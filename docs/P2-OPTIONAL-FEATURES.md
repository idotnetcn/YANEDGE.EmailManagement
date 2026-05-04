# P2可选功能配置指南 (P2 Optional Features Configuration Guide)

本文档说明如何配置和启用P2阶段的可选功能。

## 功能概述 (Features Overview)

### 1. Webhook事件总线 (Webhook Event Bus)
自动向外部系统推送邮件管理系统中的重要事件。

### 2. Elasticsearch高级搜索 (Elasticsearch Advanced Search)
提供全文搜索、多字段搜索和高级过滤功能。

---

## Webhook配置 (Webhook Configuration)

### 功能说明
Webhook功能默认启用，通过事件总线自动触发。系统支持以下9种核心事件类型:

1. **EmailManagement.MailMessage.Received** - 邮件接收事件
2. **EmailManagement.MailMessage.Sent** - 邮件发送事件
3. **EmailManagement.MailSendTask.Created** - 发送任务创建事件
4. **EmailManagement.MailSendTask.Failed** - 发送任务失败事件
5. **EmailManagement.MailThread.StatusChanged** - 会话状态变更事件
6. **EmailManagement.MailApproval.Requested** - 审批请求事件
7. **EmailManagement.MailApproval.Completed** - 审批完成事件
8. **EmailManagement.MailAttachment.Accessed** - 附件访问事件
9. **EmailManagement.MailRule.Executed** - 规则执行事件

### 使用方式

#### 1. 创建Webhook订阅

```http
POST /api/mail-management/v1/webhooks
Content-Type: application/json

{
  "name": "My Webhook",
  "url": "https://your-domain.com/webhook-endpoint",
  "secret": "your-secret-key",
  "subscribedEvents": [
    "EmailManagement.MailMessage.Received",
    "EmailManagement.MailMessage.Sent"
  ],
  "headers": {
    "X-Custom-Header": "custom-value"
  },
  "maxRetryCount": 5,
  "timeoutSeconds": 30,
  "description": "Webhook for mail notifications"
}
```

#### 2. 验证Webhook签名

Webhook请求会包含以下HTTP头:
- `X-EmailManagement-Signature`: HMAC-SHA256签名 (Base64编码)
- `X-EmailManagement-Event-Type`: 事件类型
- `X-EmailManagement-Event-Id`: 事件ID (GUID)

验证签名示例 (C#):
```csharp
using System.Security.Cryptography;
using System.Text;

public bool ValidateWebhookSignature(string payload, string signature, string secret)
{
    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
    var computedSignature = Convert.ToBase64String(hash);
    return signature == computedSignature;
}
```

#### 3. Webhook管理API

```http
# 获取订阅列表
GET /api/mail-management/v1/webhooks

# 获取指定订阅
GET /api/mail-management/v1/webhooks/{id}

# 更新订阅
PUT /api/mail-management/v1/webhooks/{id}

# 删除订阅
DELETE /api/mail-management/v1/webhooks/{id}

# 激活订阅
POST /api/mail-management/v1/webhooks/{id}/activate

# 停用订阅
POST /api/mail-management/v1/webhooks/{id}/deactivate

# 获取可用事件类型
GET /api/mail-management/v1/webhooks/available-event-types
```

### 重试策略

Webhook投递失败时会自动重试，采用指数退避策略:
- 第1次重试: 60秒后
- 第2次重试: 5分钟后
- 第3次重试: 15分钟后
- 第4次重试: 1小时后
- 第5次重试: 2小时后

超过最大重试次数后，投递将停止。

### 后台任务

系统包含两个后台任务处理Webhook投递:
- **WebhookDeliveryJob**: 处理待投递的Webhook (每批最多50个)
- **WebhookRetryJob**: 处理失败重试 (每批最多20个)

建议配置:
- WebhookDeliveryJob: 每1分钟执行一次
- WebhookRetryJob: 每5分钟执行一次

---

## Elasticsearch配置 (Elasticsearch Configuration)

### 功能说明
Elasticsearch搜索功能**默认禁用**，需要手动启用。提供全文搜索、多字段搜索和高级过滤功能。

### 配置步骤

#### 1. 安装Elasticsearch

```bash
# 使用Docker安装 (推荐)
docker run -d \
  --name elasticsearch \
  -p 9200:9200 \
  -p 9300:9300 \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  docker.elastic.co/elasticsearch/elasticsearch:8.11.0
```

#### 2. 配置appsettings.json

在 `appsettings.json` 中添加或修改 Elasticsearch 配置:

```json
{
  "Elasticsearch": {
    "Enabled": true,
    "Url": "http://localhost:9200",
    "Username": "",
    "Password": ""
  }
}
```

配置参数说明:
- **Enabled**: 是否启用Elasticsearch (默认: `false`)
- **Url**: Elasticsearch服务地址
- **Username**: 用户名 (可选，如果未启用安全认证可留空)
- **Password**: 密码 (可选，如果未启用安全认证可留空)

#### 3. 初始化索引

首次启用时需要初始化索引:

```http
POST /api/mail-management/v1/search/sync
Content-Type: application/json

{
  "syncType": 0  // 0=全量同步, 1=增量同步
}
```

### 使用方式

#### 1. 搜索邮件

```http
GET /api/mail-management/v1/search?query=your-search-term&skipCount=0&maxResultCount=20
```

高级搜索参数:
```http
GET /api/mail-management/v1/search?
  query=keyword&
  mailAccountId=guid&
  fromAddress=sender@example.com&
  hasAttachments=true&
  startDate=2024-01-01&
  endDate=2024-12-31&
  skipCount=0&
  maxResultCount=20
```

#### 2. 检查服务状态

```http
GET /api/mail-management/v1/search/health
```

响应示例:
```json
{
  "isAvailable": true,
  "isEnabled": true,
  "status": "Healthy"
}
```

#### 3. 手动触发索引同步

```http
# 增量同步 (最近24小时)
POST /api/mail-management/v1/search/sync
Content-Type: application/json
{
  "syncType": 1
}

# 全量同步
POST /api/mail-management/v1/search/sync
Content-Type: application/json
{
  "syncType": 0
}
```

### 搜索功能特性

1. **全文搜索**: 搜索邮件主题、正文、发件人、附件名称
2. **字段权重**:
   - 主题权重: x3
   - 正文权重: x2
   - 其他字段: x1
3. **多条件过滤**:
   - 邮件账户
   - 会话ID
   - 发件人/收件人
   - 日期范围
   - 是否有附件
   - 标签
4. **排序**: 支持按主题、发件人、接收时间排序
5. **分页**: 支持分页查询

### 后台任务

**SearchIndexSyncJob**: 搜索索引同步任务
- 支持全量同步和增量同步
- 批量处理 (每批100条消息)
- 自动处理分页

建议配置:
- 增量同步: 每30分钟执行一次
- 全量同步: 每天凌晨执行一次

### 索引结构

Elasticsearch索引名称: `mail-messages`

索引字段:
```
id               (Guid)      - 消息ID
threadId         (Guid)      - 会话ID
mailAccountId    (Guid)      - 邮件账户ID
subject          (Text)      - 主题 (全文搜索)
fromAddress      (Keyword)   - 发件人地址
fromName         (Text)      - 发件人名称
toAddresses      (Keyword[]) - 收件人地址列表
ccAddresses      (Keyword[]) - 抄送地址列表
bodyPreview      (Text)      - 正文预览
bodyText         (Text)      - 正文内容 (全文搜索)
hasAttachments   (Boolean)   - 是否有附件
attachmentNames  (Keyword[]) - 附件名称列表
labels           (Keyword[]) - 标签列表
receivedTime     (Date)      - 接收时间
indexedAt        (Date)      - 索引时间
```

### 性能优化建议

1. **Elasticsearch硬件配置**:
   - 建议最少2GB内存
   - 生产环境建议4GB+内存
   - SSD存储以提升搜索性能

2. **索引策略**:
   - 使用增量同步减少系统负载
   - 非高峰时段执行全量同步
   - 考虑使用索引生命周期管理(ILM)清理旧数据

3. **搜索优化**:
   - 使用分页避免一次性加载过多结果
   - 使用过滤条件缩小搜索范围
   - 避免使用通配符开头的搜索 (如 `*term`)

---

## 故障排查 (Troubleshooting)

### Webhook问题

**问题**: Webhook投递失败
- 检查目标URL是否可访问
- 验证防火墙配置
- 检查日志中的错误信息
- 确认订阅状态为"激活"

**问题**: 未收到Webhook通知
- 检查订阅的事件类型是否正确
- 确认订阅状态为"激活" (`isActive: true`)
- 查看WebhookDeliveryLogs表中的投递记录
- 检查后台任务是否正常运行

### Elasticsearch问题

**问题**: 搜索返回空结果
- 检查Elasticsearch服务是否运行: `curl http://localhost:9200`
- 确认配置中 `Enabled` 设置为 `true`
- 检查索引是否存在: `curl http://localhost:9200/mail-messages`
- 执行一次全量索引同步

**问题**: 搜索性能慢
- 检查Elasticsearch内存配置
- 优化搜索查询，使用更多过滤条件
- 考虑使用SSD存储
- 检查索引分片配置

**问题**: 索引同步失败
- 检查应用日志中的错误信息
- 验证Elasticsearch连接配置
- 确认Elasticsearch磁盘空间充足
- 检查数据库连接是否正常

---

## 监控指标 (Monitoring)

### Webhook监控

建议监控的指标:
- Webhook投递成功率
- 平均投递延迟
- 失败重试次数
- 队列积压数量

查询示例 (SQL):
```sql
-- 投递成功率
SELECT
  DeliveryStatus,
  COUNT(*) as Count,
  COUNT(*) * 100.0 / SUM(COUNT(*)) OVER() as Percentage
FROM WebhookDeliveryLogs
WHERE CreationTime > NOW() - INTERVAL '24 hours'
GROUP BY DeliveryStatus;

-- 最近失败的投递
SELECT * FROM WebhookDeliveryLogs
WHERE DeliveryStatus = 2  -- Failed
ORDER BY CreationTime DESC
LIMIT 100;
```

### Elasticsearch监控

建议监控的指标:
- 索引文档数量
- 搜索请求QPS
- 平均搜索延迟
- 索引同步延迟

查询Elasticsearch状态:
```bash
# 集群健康状态
curl http://localhost:9200/_cluster/health

# 索引统计
curl http://localhost:9200/mail-messages/_stats

# 索引文档数
curl http://localhost:9200/mail-messages/_count
```

---

## 安全建议 (Security Recommendations)

### Webhook安全

1. **使用HTTPS**: 生产环境必须使用HTTPS URLs
2. **验证签名**: 始终验证 `X-EmailManagement-Signature` 头
3. **保护Secret**: 使用强随机字符串作为secret
4. **限制重试**: 避免无限重试导致的资源消耗
5. **日志审计**: 定期审查webhook投递日志

### Elasticsearch安全

1. **启用认证**: 生产环境启用xpack.security
2. **网络隔离**: 限制Elasticsearch访问端口
3. **使用TLS**: 启用节点间和客户端通信加密
4. **访问控制**: 配置适当的用户角色和权限
5. **定期备份**: 配置快照备份策略

---

## 附录 (Appendix)

### 相关表结构

**WebhookSubscriptions**:
```sql
Id                Guid
Name              varchar(200)
Url               varchar(2000)
Secret            varchar(500)
IsActive          boolean
SubscribedEvents  jsonb
Headers           jsonb
MaxRetryCount     int
TimeoutSeconds    int
Description       text
TenantId          Guid (nullable)
```

**WebhookDeliveryLogs**:
```sql
Id                Guid
SubscriptionId    Guid
EventId           Guid
EventType         varchar(200)
Payload           text
TargetUrl         varchar(2000)
DeliveryStatus    int (0=Pending, 1=Successful, 2=Failed)
HttpStatusCode    int (nullable)
ResponseBody      text (nullable)
ErrorMessage      text (nullable)
RetryCount        int
NextRetryTime     datetime (nullable)
DeliveredAt       datetime (nullable)
TenantId          Guid (nullable)
```

### 依赖包

Webhook功能所需NuGet包:
- Volo.Abp.EventBus
- Volo.Abp.BackgroundJobs (Hangfire)

Elasticsearch功能所需NuGet包:
- Elastic.Clients.Elasticsearch (>= 8.0.0)

### 参考文档

- [ABP Framework Documentation](https://docs.abp.io/)
- [Elasticsearch Guide](https://www.elastic.co/guide/en/elasticsearch/reference/current/index.html)
- [Webhook Best Practices](https://webhooks.dev/)
