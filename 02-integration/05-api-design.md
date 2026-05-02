
# 邮件管理系统接口设计文档（API Design）

> 项目名称：邮件管理系统
> 技术路线：.NET 10.x / ABP 10.x / PostgreSQL
> 文档版本：V1.0
> 文档属性：接口设计文档
> 适用阶段：前后端联调 / 开放接口对接 / 应用服务实现 / 测试设计 / 安全评审

---

## 修订记录

| 版本 | 日期 | 作者 | 说明 |
|---|---|---|---|
| V1.0 | 2026-04-25 | 项目组 | 初始版本 |

---

## 目录

- [1. 文档目标](#1-文档目标)
- [2. 设计范围与接口分类](#2-设计范围与接口分类)
- [3. API 设计原则](#3-api-设计原则)
- [4. 接口基础规范](#4-接口基础规范)
- [5. 认证与授权设计](#5-认证与授权设计)
- [6. 通用请求与响应模型](#6-通用请求与响应模型)
- [7. 错误码与异常响应设计](#7-错误码与异常响应设计)
- [8. 内部管理 API 设计](#8-内部管理-api-设计)
- [9. 开放平台 Open API 设计](#9-开放平台-open-api-设计)
- [10. Webhook 事件推送设计](#10-webhook-事件推送设计)
- [11. 接口安全与审计要求](#11-接口安全与审计要求)
- [12. API 版本管理与兼容策略](#12-api-版本管理与兼容策略)
- [13. 测试与联调建议](#13-测试与联调建议)
- [14. 结论](#14-结论)

---

# 1. 文档目标

本文档用于定义邮件管理系统的接口设计规范与核心 API 清单，重点解决以下问题：

1. 系统内部管理端与外部集成接口如何划分
2. API 路径、方法、版本、认证方式如何统一
3. 邮箱管理、收件箱、线程、发件、审批、协同、附件、规则、统计等接口如何设计
4. 返回结构、错误码、分页、筛选、幂等等横切规范如何落地
5. Webhook 推送模型与安全签名如何定义
6. 如何为前后端联调、第三方接入、自动化测试提供稳定契约

本文档是以下工作的直接输入：

- HttpApi 层 Controller 设计
- Application Service 入参与返回 DTO 设计
- Swagger / OpenAPI 文档输出
- 前端页面联调
- 第三方系统对接
- 安全测试与接口测试

---

# 2. 设计范围与接口分类

## 2.1 接口范围

本文档覆盖以下接口类型：

1. **内部管理 API**
   - 面向 Web 管理端、运营端、审批端、管理员使用
   - 使用用户身份认证和权限控制

2. **开放平台 Open API**
   - 面向 ERP / CRM / OA / Portal / 外部业务系统
   - 使用应用身份认证、签名和幂等控制

3. **Webhook 事件推送**
   - 面向外部订阅方
   - 由本系统主动推送业务事件

## 2.2 接口分类总览

| 接口类型 | 使用方 | 认证方式 | 典型场景 |
|---|---|---|---|
| 内部管理 API | 前端、管理员、业务人员 | JWT / OIDC | 邮件处理、审批、配置、管理 |
| Open API | 外部系统 | AppKey + Signature / OAuth2 | 创建待发邮件、查询结果、建立业务关联 |
| Webhook | 外部订阅方 | 签名验签 | 邮件接收完成、发送结果、审批结果通知 |

## 2.3 路径规划

建议统一采用以下基础路径：

### 内部管理 API
```text
/api/mail-management/v1
```

### 开放平台 Open API
```text
/api/mail-management/open-api/v1
```

### Webhook 管理 API
```text
/api/mail-management/v1/webhook-subscriptions
```

> 说明：Webhook 是“系统主动调用外部地址”，本系统只需要提供订阅管理接口，不需要提供通用入站 webhook 接收接口，除非后续接第三方邮件事件回调。

---

# 3. API 设计原则

## 3.1 REST 风格优先

接口设计遵循 REST 风格：

- 资源名使用复数
- 使用 HTTP 方法表达动作
- 路径表达资源层级
- 状态变化尽量通过动作型子资源或明确命令接口表达

示例：

- `GET /threads`
- `GET /threads/{id}`
- `POST /threads/{id}/claim`
- `POST /send-tasks/{id}/submit-approval`

## 3.2 命令与查询分离

建议在接口设计上体现 CQRS 思想：

- **查询接口**：返回读模型，强调检索效率与展示完整性
- **命令接口**：表达业务动作，强调语义和约束

例如：

- 查询线程详情：`GET /threads/{id}`
- 认领线程：`POST /threads/{id}/claim`

## 3.3 资源稳定、动作明确

对于“认领、转派、归档、审批”这类明显业务动作，不强行做成普通 `PUT` 更新，而使用动作接口：

- `POST /threads/{id}/assign`
- `POST /threads/{id}/archive`
- `POST /approvals/{id}/approve`

## 3.4 兼顾内部实现与对外契约稳定

API 设计应稳定，不暴露：

- EF Core 实体结构
- 数据库表名
- ABP 内部实现细节
- 过于底层的枚举编码语义

对外返回的字段命名与 DTO 结构应面向业务语义。

## 3.5 可扩展与可兼容

接口必须支持：

- 版本演进
- 新字段向后兼容
- OpenAPI 自动生成
- 外部系统低成本接入

---

# 4. 接口基础规范

## 4.1 协议与格式

- 协议：`HTTPS`
- 编码：`UTF-8`
- 数据格式：`application/json`
- 文件下载：`application/octet-stream` 或实际文件 MIME 类型
- 时间格式：ISO 8601 UTC，例如：
  - `2026-04-25T08:30:00Z`

## 4.2 路径命名规范

- 路径使用小写
- 单词使用中划线分隔
- 资源使用复数名词
- 避免在路径中暴露技术实现名

示例：

```text
/mail-accounts
/messages
/threads
/send-tasks
/approvals
/templates
/rules
/attachments
/business-relations
```

## 4.3 查询参数规范

通用查询参数建议：

| 参数 | 说明 |
|---|---|
| `pageNo` | 页码，从 1 开始 |
| `pageSize` | 每页条数 |
| `sorting` | 排序字段，如 `latestMessageTime desc` |
| `keyword` | 关键字 |
| `status` | 状态过滤 |
| `from` / `to` | 时间范围 |
| `include` | 可选扩展字段 |

## 4.4 分页规范

分页统一采用：

```json
{
  "code": "0",
  "message": "success",
  "data": {
    "items": [],
    "pageNo": 1,
    "pageSize": 20,
    "totalCount": 235
  },
  "traceId": "00-6f4b..."
}
```

## 4.5 幂等规范

对以下写接口要求支持幂等：

- Open API 创建待发邮件
- 发件提交
- 发件重试
- Webhook 投递记录处理
- 外部业务关联写入

建议使用请求头：

```text
Idempotency-Key: 6d6f9d3f-3c8d-4d2a-8a6d-xxxx
```

## 4.6 追踪头规范

建议支持以下请求头：

| Header | 说明 |
|---|---|
| `Authorization` | Bearer Token |
| `X-Correlation-Id` | 业务调用链关联 ID |
| `Idempotency-Key` | 幂等键 |
| `X-App-Key` | Open API 应用标识 |
| `X-Timestamp` | 签名时间戳 |
| `X-Nonce` | 随机串 |
| `X-Signature` | 签名值 |

---

# 5. 认证与授权设计

## 5.1 内部管理 API 认证

内部管理 API 建议使用：

- OIDC / OAuth2 登录
- JWT Bearer Token
- 或 ABP 默认认证集成

请求头示例：

```text
Authorization: Bearer {access_token}
```

## 5.2 Open API 认证

外部系统调用 Open API 建议采用：

- `X-App-Key`
- `X-Timestamp`
- `X-Nonce`
- `X-Signature`

签名串建议：

```text
signature = HMACSHA256(appSecret, method + "\n" + path + "\n" + timestamp + "\n" + nonce + "\n" + bodyHash)
```

## 5.3 权限控制

内部管理 API 应结合 ABP 权限系统，建议权限粒度包括：

| 权限编码 | 说明 |
|---|---|
| `MailManagement.MailAccounts.Manage` | 邮箱管理 |
| `MailManagement.MailAccounts.Sync` | 手动同步 |
| `MailManagement.Messages.View` | 查看邮件 |
| `MailManagement.Threads.View` | 查看线程 |
| `MailManagement.Threads.Assign` | 分派线程 |
| `MailManagement.Threads.Claim` | 认领线程 |
| `MailManagement.Threads.Archive` | 归档线程 |
| `MailManagement.SendTasks.Create` | 创建发件任务 |
| `MailManagement.SendTasks.Send` | 发送邮件 |
| `MailManagement.SendTasks.Approve` | 审批发件 |
| `MailManagement.Attachments.Download` | 下载附件 |
| `MailManagement.Rules.Manage` | 管理规则 |
| `MailManagement.Templates.Manage` | 管理模板 |
| `MailManagement.Integration.Manage` | 管理开放平台 |
| `MailManagement.Statistics.View` | 查看统计 |

## 5.4 数据权限

除功能权限外，还需额外校验：

- 用户是否有权限访问该邮箱
- 用户是否可查看该线程
- 用户是否可下载敏感附件
- 用户是否是当前审批人或具备审批权限
- 外部应用是否具备对应 scope

---

# 6. 通用请求与响应模型

## 6.1 统一响应结构

建议内部 API 与 Open API 尽量统一响应格式：

```json
{
  "code": "0",
  "message": "success",
  "data": {},
  "traceId": "00-6f4b8b8c4d8f6f3f4a1a...",
  "timestamp": "2026-04-25T09:20:00Z"
}
```

字段说明：

| 字段 | 类型 | 说明 |
|---|---|---|
| `code` | string | 业务码，`0` 表示成功 |
| `message` | string | 响应消息 |
| `data` | object | 返回数据 |
| `traceId` | string | 调用链追踪 ID |
| `timestamp` | string | 服务端时间 |

## 6.2 列表响应结构

```json
{
  "code": "0",
  "message": "success",
  "data": {
    "items": [
      {
        "id": "7a4e4d73-35d1-4b77-9b1e-1dc5b8d8c001",
        "subject": "报价确认",
        "threadStatus": 2
      }
    ],
    "pageNo": 1,
    "pageSize": 20,
    "totalCount": 120
  },
  "traceId": "00-xxxx",
  "timestamp": "2026-04-25T09:20:00Z"
}
```

## 6.3 文件下载响应

文件下载接口成功时直接返回二进制流，并设置：

- `Content-Type`
- `Content-Disposition`
- `Content-Length`
- `X-Trace-Id`

失败时返回标准 JSON 错误体。

## 6.4 通用 DTO 约定

### 6.4.1 ID 字段
- 类型：`string(uuid)`

### 6.4.2 枚举字段
- 对外返回建议同时提供编码和值说明，例如：

```json
{
  "threadStatus": 2,
  "threadStatusName": "待处理"
}
```

### 6.4.3 用户字段
涉及用户信息的返回建议采用：

```json
{
  "user": {
    "id": "uuid",
    "name": "张三"
  }
}
```

而不是直接散落多个平级字段。

---

# 7. 错误码与异常响应设计

## 7.1 HTTP 状态码使用建议

| HTTP 状态码 | 场景 |
|---|---|
| `200` | 成功查询、成功命令 |
| `201` | 成功创建资源 |
| `202` | 已接受，异步处理中 |
| `400` | 参数错误、业务校验失败 |
| `401` | 未认证 |
| `403` | 无权限 |
| `404` | 资源不存在 |
| `409` | 状态冲突、幂等冲突、重复提交 |
| `422` | 业务规则不满足 |
| `429` | 频率超限 |
| `500` | 服务器内部错误 |

## 7.2 错误响应结构

```json
{
  "code": "MAIL.THREAD.INVALID_STATUS",
  "message": "当前线程状态不允许归档",
  "details": "仅已完成线程允许归档",
  "validationErrors": [],
  "traceId": "00-xxxx",
  "timestamp": "2026-04-25T09:20:00Z"
}
```

## 7.3 业务错误码建议

| 错误码 | 说明 |
|---|---|
| `MAIL.ACCOUNT.NOT_FOUND` | 邮箱账号不存在 |
| `MAIL.ACCOUNT.DISABLED` | 邮箱账号已停用 |
| `MAIL.ACCOUNT.SEND_FORBIDDEN` | 当前邮箱不允许发件 |
| `MAIL.MESSAGE.NOT_FOUND` | 邮件不存在 |
| `MAIL.THREAD.NOT_FOUND` | 线程不存在 |
| `MAIL.THREAD.INVALID_STATUS` | 线程状态非法 |
| `MAIL.THREAD.CLAIM_FORBIDDEN` | 当前线程不可认领 |
| `MAIL.SENDTASK.NOT_FOUND` | 发件任务不存在 |
| `MAIL.SENDTASK.INVALID_STATUS` | 发件任务状态非法 |
| `MAIL.SENDTASK.RETRY_LIMIT_EXCEEDED` | 超出最大重试次数 |
| `MAIL.APPROVAL.NOT_FOUND` | 审批单不存在 |
| `MAIL.APPROVAL.NOT_CURRENT_APPROVER` | 当前用户不是审批人 |
| `MAIL.APPROVAL.INVALID_STATUS` | 审批状态非法 |
| `MAIL.ATTACHMENT.NOT_FOUND` | 附件不存在 |
| `MAIL.ATTACHMENT.DOWNLOAD_FORBIDDEN` | 无权下载附件 |
| `MAIL.RULE.INVALID_CONFIG` | 规则配置非法 |
| `MAIL.TEMPLATE.VARIABLE_MISSING` | 模板变量缺失 |
| `INTEGRATION.AUTH.INVALID_SIGNATURE` | 签名无效 |
| `INTEGRATION.AUTH.APP_DISABLED` | 接入应用已禁用 |
| `INTEGRATION.IDEMPOTENCY.CONFLICT` | 幂等键冲突 |
| `COMMON.VALIDATION_ERROR` | 参数校验失败 |

---

# 8. 内部管理 API 设计

# 8.1 邮箱账号 API

## 8.1.1 查询邮箱账号列表

**GET** `/api/mail-management/v1/mail-accounts`

### 查询参数

| 参数 | 类型 | 必填 | 说明 |
|---|---|---|---|
| `keyword` | string | 否 | 账号名称/邮箱关键字 |
| `accountType` | int | 否 | 邮箱类型 |
| `syncEnabled` | bool | 否 | 是否启用同步 |
| `sendEnabled` | bool | 否 | 是否允许发件 |
| `pageNo` | int | 否 | 页码 |
| `pageSize` | int | 否 | 每页条数 |

### 返回字段
- `id`
- `accountName`
- `emailAddress`
- `displayName`
- `accountType`
- `accountTypeName`
- `syncEnabled`
- `sendEnabled`
- `healthStatus`
- `healthStatusName`
- `lastSyncAt`
- `lastSendAt`

### 权限
`MailManagement.MailAccounts.Manage`

---

## 8.1.2 获取邮箱账号详情

**GET** `/api/mail-management/v1/mail-accounts/{id}`

### 返回字段
- 基础信息
- 收/发件服务器配置摘要
- 所属用户/组织
- 当前健康状态
- 最近同步状态
- 权限授权摘要

---

## 8.1.3 创建邮箱账号

**POST** `/api/mail-management/v1/mail-accounts`

### 请求体示例

```json
{
  "accountName": "客服共享邮箱",
  "emailAddress": "service@example.com",
  "displayName": "客服中心",
  "accountType": 2,
  "providerName": "default",
  "incomingProtocol": 1,
  "incomingHost": "imap.example.com",
  "incomingPort": 993,
  "incomingSslEnabled": true,
  "outgoingProtocol": 1,
  "outgoingHost": "smtp.example.com",
  "outgoingPort": 465,
  "outgoingSslEnabled": true,
  "authType": 1,
  "password": "******",
  "syncEnabled": true,
  "sendEnabled": true,
  "ownerOrgId": "uuid"
}
```

### 返回
创建后的邮箱账号详情。

---

## 8.1.4 更新邮箱账号

**PUT** `/api/mail-management/v1/mail-accounts/{id}`

---

## 8.1.5 启用/停用同步

**POST** `/api/mail-management/v1/mail-accounts/{id}/sync-toggle`

### 请求体
```json
{
  "enabled": true
}
```

---

## 8.1.6 测试邮箱连接

**POST** `/api/mail-management/v1/mail-accounts/{id}/test-connection`

### 返回示例

```json
{
  "code": "0",
  "message": "success",
  "data": {
    "incomingSuccess": true,
    "outgoingSuccess": true,
    "detail": "IMAP/SMTP connection test passed"
  }
}
```

---

## 8.1.7 手动触发邮箱同步

**POST** `/api/mail-management/v1/mail-accounts/{id}/sync`

### 请求体
```json
{
  "folderCodes": ["inbox"],
  "forceFullScan": false
}
```

### 返回
- `jobId`
- `accepted`
- `message`

### 说明
该接口建议返回 `202 Accepted`，表示后台任务已接收。

---

## 8.1.8 配置邮箱授权

**PUT** `/api/mail-management/v1/mail-accounts/{id}/permissions`

### 请求体示例

```json
{
  "items": [
    {
      "subjectType": 1,
      "subjectId": "uuid-user-1",
      "canView": true,
      "canSend": true,
      "canManage": false,
      "canDownloadAttachment": true,
      "canApprove": false
    }
  ]
}
```

---

# 8.2 收件箱与邮件 API

## 8.2.1 查询邮件列表

**GET** `/api/mail-management/v1/messages`

### 查询参数

| 参数 | 说明 |
|---|---|
| `mailAccountId` | 邮箱账号 ID |
| `threadId` | 线程 ID |
| `folderCode` | 文件夹编码 |
| `direction` | 收件/发件 |
| `keyword` | 主题/地址关键字 |
| `from` `to` | 时间范围 |
| `hasAttachment` | 是否有附件 |
| `isRead` | 是否已读 |
| `pageNo` `pageSize` | 分页 |

### 返回字段
- `id`
- `threadId`
- `mailAccountId`
- `subject`
- `direction`
- `receivedTime`
- `sentTime`
- `from`
- `toSummary`
- `hasAttachment`
- `isRead`
- `importance`

---

## 8.2.2 获取邮件详情

**GET** `/api/mail-management/v1/messages/{id}`

### 返回字段
- 基本元数据
- 发件人 / 收件人 / 抄送 / 密送
- 正文摘要
- `sanitizedHtmlBody`
- `textBody`
- 附件列表
- 所属线程信息
- 用户已读状态

---

## 8.2.3 标记已读/未读

**POST** `/api/mail-management/v1/messages/{id}/read-status`

### 请求体
```json
{
  "isRead": true
}
```

---

## 8.2.4 星标/取消星标

**POST** `/api/mail-management/v1/messages/{id}/star`

### 请求体
```json
{
  "isStarred": true
}
```

---

## 8.2.5 获取邮件原始头信息

**GET** `/api/mail-management/v1/messages/{id}/headers`

### 说明
仅管理员或具备调试权限用户可访问。

---

# 8.3 线程 API

## 8.3.1 查询线程列表

**GET** `/api/mail-management/v1/threads`

### 查询参数

| 参数 | 说明 |
|---|---|
| `mailAccountId` | 归属邮箱 |
| `threadStatus` | 线程状态 |
| `assigneeType` | 当前负责人类型 |
| `assigneeId` | 当前负责人 |
| `tagId` | 标签 |
| `hasAttachment` | 是否有附件 |
| `keyword` | 主题/参与人关键字 |
| `from` `to` | 时间范围 |
| `pageNo` `pageSize` | 分页 |
| `sorting` | 排序 |

### 返回字段
- `id`
- `subject`
- `threadStatus`
- `threadStatusName`
- `latestMessageTime`
- `messageCount`
- `currentAssignee`
- `hasAttachment`
- `priority`
- `businessSummary`
- `unreadCount`

---

## 8.3.2 获取线程详情

**GET** `/api/mail-management/v1/threads/{id}`

### 返回内容
- 线程摘要
- 当前状态
- 当前负责人
- 业务关联摘要
- 邮件列表
- 标签列表
- 最近活动
- 协同信息摘要
- 审批信息摘要

---

## 8.3.3 认领线程

**POST** `/api/mail-management/v1/threads/{id}/claim`

### 请求体
```json
{
  "reason": "由我负责跟进"
}
```

### 业务约束
- 线程必须在共享池或允许认领状态
- 当前用户必须具备认领权限

---

## 8.3.4 分派线程

**POST** `/api/mail-management/v1/threads/{id}/assign`

### 请求体
```json
{
  "toAssigneeType": 1,
  "toAssigneeId": "uuid-user-2",
  "reason": "转交客户成功团队处理"
}
```

---

## 8.3.5 归档线程

**POST** `/api/mail-management/v1/threads/{id}/archive`

### 请求体
```json
{
  "remark": "已处理完成，归档"
}
```

---

## 8.3.6 关闭线程

**POST** `/api/mail-management/v1/threads/{id}/close`

---

## 8.3.7 恢复线程

**POST** `/api/mail-management/v1/threads/{id}/reopen`

---

## 8.3.8 给线程打标签

**POST** `/api/mail-management/v1/threads/{id}/tags`

### 请求体
```json
{
  "tagIds": ["uuid-tag-1", "uuid-tag-2"]
}
```

---

## 8.3.9 获取线程活动时间线

**GET** `/api/mail-management/v1/threads/{id}/activities`

---

# 8.4 发件与草稿 API

## 8.4.1 创建发件任务（草稿）

**POST** `/api/mail-management/v1/send-tasks`

### 请求体示例

```json
{
  "mailAccountId": "uuid-account",
  "threadId": "uuid-thread",
  "subject": "关于报价确认",
  "bodyHtml": "<p>您好，附件为最新报价单。</p>",
  "bodyText": "您好，附件为最新报价单。",
  "templateId": null,
  "signatureId": "uuid-signature",
  "scheduledSendTime": null,
  "recipients": [
    {
      "recipientType": 1,
      "emailAddress": "customer@example.com",
      "displayName": "李经理"
    }
  ],
  "attachmentIds": [
    "uuid-attachment-temp-1"
  ]
}
```

### 返回
- `id`
- `status`
- `needApproval`

---

## 8.4.2 获取发件任务详情

**GET** `/api/mail-management/v1/send-tasks/{id}`

---

## 8.4.3 更新草稿

**PUT** `/api/mail-management/v1/send-tasks/{id}`

### 约束
仅 `草稿`、`待审批退回后可编辑` 状态允许修改。

---

## 8.4.4 预览模板渲染结果

**POST** `/api/mail-management/v1/send-tasks/preview`

### 请求体
```json
{
  "templateId": "uuid-template",
  "signatureId": "uuid-signature",
  "variables": {
    "customerName": "张先生",
    "orderNo": "SO20260425001"
  }
}
```

### 返回
- `renderedSubject`
- `renderedBodyHtml`
- `renderedBodyText`

---

## 8.4.5 提交审批

**POST** `/api/mail-management/v1/send-tasks/{id}/submit-approval`

### 返回
- `approvalId`
- `status`

---

## 8.4.6 立即发送

**POST** `/api/mail-management/v1/send-tasks/{id}/send`

### 说明
- 若命中审批策略，则返回业务错误或自动转换为待审批
- 建议支持 `Idempotency-Key`

---

## 8.4.7 取消发送任务

**POST** `/api/mail-management/v1/send-tasks/{id}/cancel`

---

## 8.4.8 重试发送

**POST** `/api/mail-management/v1/send-tasks/{id}/retry`

### 业务约束
- 仅失败状态允许重试
- 不得超过最大重试次数

---

## 8.4.9 查询发件任务列表

**GET** `/api/mail-management/v1/send-tasks`

### 常用过滤
- `status`
- `mailAccountId`
- `createdByUserId`
- `needApproval`
- `from` `to`

---

# 8.5 协同 API

## 8.5.1 添加内部备注

**POST** `/api/mail-management/v1/threads/{id}/internal-notes`

### 请求体
```json
{
  "content": "客户要求本周内回复最终价格。",
  "mentionUserIds": ["uuid-user-1", "uuid-user-2"],
  "isPinned": false
}
```

---

## 8.5.2 查询内部备注列表

**GET** `/api/mail-management/v1/threads/{id}/internal-notes`

---

## 8.5.3 转派线程

**POST** `/api/mail-management/v1/threads/{id}/transfer`

### 请求体
```json
{
  "toAssigneeType": 1,
  "toAssigneeId": "uuid-user-2",
  "reason": "该客户由华东区负责人继续跟进"
}
```

---

## 8.5.4 退回共享池

**POST** `/api/mail-management/v1/threads/{id}/return-to-pool`

---

## 8.5.5 查询我的待办线程

**GET** `/api/mail-management/v1/workbench/my-todos`

### 常用返回字段
- `threadId`
- `subject`
- `todoType`
- `priority`
- `dueTime`
- `latestMessageTime`

---

# 8.6 审批 API

## 8.6.1 查询待审批列表

**GET** `/api/mail-management/v1/approvals/pending`

### 查询参数
- `businessType`
- `applicantId`
- `mailAccountId`
- `submittedFrom`
- `submittedTo`
- `pageNo`
- `pageSize`

---

## 8.6.2 获取审批详情

**GET** `/api/mail-management/v1/approvals/{id}`

### 返回内容
- 审批单主信息
- 业务快照
- 发件任务摘要
- 当前审批步骤
- 审批记录列表

---

## 8.6.3 审批通过

**POST** `/api/mail-management/v1/approvals/{id}/approve`

### 请求体
```json
{
  "comment": "内容合规，可以发送"
}
```

---

## 8.6.4 审批拒绝

**POST** `/api/mail-management/v1/approvals/{id}/reject`

### 请求体
```json
{
  "comment": "附件内容需补充合同编号"
}
```

---

## 8.6.5 撤回审批

**POST** `/api/mail-management/v1/approvals/{id}/withdraw`

---

## 8.6.6 查询审批历史

**GET** `/api/mail-management/v1/approvals/history`

---

# 8.7 模板与签名 API

## 8.7.1 查询模板列表

**GET** `/api/mail-management/v1/templates`

---

## 8.7.2 获取模板详情

**GET** `/api/mail-management/v1/templates/{id}`

---

## 8.7.3 创建模板

**POST** `/api/mail-management/v1/templates`

### 请求体示例
```json
{
  "templateCode": "QUOTE_REPLY",
  "templateName": "报价回复模板",
  "categoryId": "uuid-category",
  "subjectTemplate": "关于 ${customerName} 的报价回复",
  "bodyTemplate": "<p>您好 ${customerName}，请查收附件。</p>",
  "isHtml": true,
  "description": "用于报价邮件回复"
}
```

---

## 8.7.4 更新模板

**PUT** `/api/mail-management/v1/templates/{id}`

---

## 8.7.5 发布模板版本

**POST** `/api/mail-management/v1/templates/{id}/publish`

### 请求体
```json
{
  "changeSummary": "修正变量名称并优化措辞"
}
```

---

## 8.7.6 查询模板版本历史

**GET** `/api/mail-management/v1/templates/{id}/versions`

---

## 8.7.7 查询签名列表

**GET** `/api/mail-management/v1/signatures`

---

## 8.7.8 创建签名

**POST** `/api/mail-management/v1/signatures`

---

# 8.8 规则 API

## 8.8.1 查询规则列表

**GET** `/api/mail-management/v1/rules`

### 查询参数
- `ruleType`
- `mailAccountId`
- `isEnabled`
- `keyword`

---

## 8.8.2 获取规则详情

**GET** `/api/mail-management/v1/rules/{id}`

---

## 8.8.3 创建规则

**POST** `/api/mail-management/v1/rules`

### 请求体示例

```json
{
  "ruleCode": "AUTO_ASSIGN_VIP",
  "ruleName": "VIP客户自动分派",
  "ruleType": 2,
  "priority": 10,
  "scopeType": 2,
  "mailAccountId": "uuid-account",
  "condition": {
    "fromDomains": ["vip.example.com"],
    "subjectContains": ["紧急", "VIP"]
  },
  "actions": [
    {
      "type": "AssignToUser",
      "userId": "uuid-user-vip"
    },
    {
      "type": "AddTag",
      "tagId": "uuid-tag-vip"
    }
  ],
  "stopOnMatch": true,
  "isEnabled": true
}
```

---

## 8.8.4 更新规则

**PUT** `/api/mail-management/v1/rules/{id}`

---

## 8.8.5 启停规则

**POST** `/api/mail-management/v1/rules/{id}/toggle`

### 请求体
```json
{
  "enabled": true
}
```

---

## 8.8.6 测试规则

**POST** `/api/mail-management/v1/rules/{id}/test`

### 请求体
```json
{
  "mailMessageId": "uuid-message"
}
```

### 返回
- `matched`
- `matchedConditions`
- `plannedActions`

---

## 8.8.7 查询规则执行日志

**GET** `/api/mail-management/v1/rules/{id}/execution-logs`

---

# 8.9 附件 API

## 8.9.1 查询邮件附件列表

**GET** `/api/mail-management/v1/messages/{id}/attachments`

---

## 8.9.2 获取附件元数据

**GET** `/api/mail-management/v1/attachments/{id}`

---

## 8.9.3 下载附件

**GET** `/api/mail-management/v1/attachments/{id}/download`

### 说明
- 需进行权限校验
- 记录下载审计日志
- 对敏感附件可增加二次确认或短期下载令牌

---

## 8.9.4 获取附件预览地址

**GET** `/api/mail-management/v1/attachments/{id}/preview`

### 返回
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "previewUrl": "https://...",
    "expiresAt": "2026-04-25T10:20:00Z"
  }
}
```

---

## 8.9.5 查询附件访问日志

**GET** `/api/mail-management/v1/attachments/{id}/access-logs`

---

# 8.10 联系人与业务关联 API

## 8.10.1 查询联系人列表

**GET** `/api/mail-management/v1/contacts`

---

## 8.10.2 获取联系人详情

**GET** `/api/mail-management/v1/contacts/{id}`

---

## 8.10.3 查询业务关联列表

**GET** `/api/mail-management/v1/business-relations`

### 查询参数
- `ownerType`
- `ownerId`
- `businessType`
- `businessId`

---

## 8.10.4 为线程建立业务关联

**POST** `/api/mail-management/v1/threads/{id}/business-relations`

### 请求体
```json
{
  "businessType": "Customer",
  "businessId": "CUST-10001",
  "businessCode": "KH2026001",
  "displayName": "上海某某科技有限公司",
  "isPrimary": true
}
```

---

## 8.10.5 删除业务关联

**DELETE** `/api/mail-management/v1/business-relations/{id}`

---

## 8.10.6 获取自动关联候选项

**GET** `/api/mail-management/v1/threads/{id}/relation-candidates`

---

# 8.11 集成平台管理 API

## 8.11.1 查询接入应用列表

**GET** `/api/mail-management/v1/integration/apps`

---

## 8.11.2 创建接入应用

**POST** `/api/mail-management/v1/integration/apps`

### 请求体
```json
{
  "appName": "CRM系统",
  "allowedScopes": ["mail.send", "mail.read", "mail.relation.write"],
  "callbackBaseUrl": "https://crm.example.com/callback"
}
```

---

## 8.11.3 轮换应用密钥

**POST** `/api/mail-management/v1/integration/apps/{id}/rotate-secret`

---

## 8.11.4 查询 Webhook 订阅列表

**GET** `/api/mail-management/v1/webhook-subscriptions`

---

## 8.11.5 创建 Webhook 订阅

**POST** `/api/mail-management/v1/webhook-subscriptions`

### 请求体
```json
{
  "integrationAppId": "uuid-app",
  "eventCode": "mail.sent",
  "targetUrl": "https://crm.example.com/webhooks/mail-sent",
  "timeoutSeconds": 10,
  "retryPolicy": {
    "maxAttempts": 6,
    "strategy": "exponential"
  }
}
```

---

## 8.11.6 查询 Webhook 投递日志

**GET** `/api/mail-management/v1/webhook-deliveries`

---

## 8.11.7 手工重推 Webhook

**POST** `/api/mail-management/v1/webhook-deliveries/{id}/redeliver`

---

# 8.12 统计 API

## 8.12.1 邮箱维度日报

**GET** `/api/mail-management/v1/statistics/daily-accounts`

### 查询参数
- `statDateFrom`
- `statDateTo`
- `mailAccountId`

---

## 8.12.2 用户维度日报

**GET** `/api/mail-management/v1/statistics/daily-users`

---

## 8.12.3 工作台概览

**GET** `/api/mail-management/v1/statistics/dashboard`

### 返回示例
```json
{
  "code": "0",
  "message": "success",
  "data": {
    "pendingThreads": 23,
    "processingThreads": 18,
    "pendingApprovals": 5,
    "failedSendTasks": 2,
    "todayReceivedCount": 156,
    "todaySentCount": 47
  }
}
```

---

## 8.12.4 规则命中统计

**GET** `/api/mail-management/v1/statistics/rule-execution`

---

# 9. 开放平台 Open API 设计

## 9.1 Open API 设计原则

开放平台 API 应满足：

- 面向外部系统，契约清晰
- 不暴露内部复杂协同状态细节
- 强制幂等和签名
- scope 控制
- 错误码稳定

## 9.2 Open API Scope 建议

| Scope | 说明 |
|---|---|
| `mail.send` | 创建待发邮件/触发发送 |
| `mail.read` | 查询邮件/线程摘要 |
| `mail.approval.read` | 查询审批状态 |
| `mail.relation.write` | 写入业务关联 |
| `mail.webhook.manage` | 管理 webhook 订阅 |

---

## 9.3 创建待发邮件

**POST** `/api/mail-management/open-api/v1/send-tasks`

### Header
- `X-App-Key`
- `X-Timestamp`
- `X-Nonce`
- `X-Signature`
- `Idempotency-Key`

### 请求体示例

```json
{
  "mailAccountCode": "SERVICE_MAIL",
  "externalBizRef": "CRM-OUT-20260425-0001",
  "subject": "客户回访通知",
  "bodyHtml": "<p>您好，这是系统发送的客户回访通知。</p>",
  "recipients": [
    {
      "recipientType": "To",
      "emailAddress": "customer@example.com",
      "displayName": "王女士"
    }
  ],
  "attachments": [
    {
      "fileName": "notice.pdf",
      "objectKey": "temp/2026/04/notice.pdf"
    }
  ],
  "variables": {
    "customerName": "王女士"
  }
}
```

### 返回示例

```json
{
  "code": "0",
  "message": "success",
  "data": {
    "sendTaskId": "uuid-send-task",
    "status": 2,
    "statusName": "待审批",
    "needApproval": true
  },
  "traceId": "00-xxxx"
}
```

---

## 9.4 查询发件任务状态

**GET** `/api/mail-management/open-api/v1/send-tasks/{id}`

### 返回字段
- `id`
- `status`
- `statusName`
- `needApproval`
- `sentAt`
- `errorCode`
- `errorMessage`
- `approvalStatus`

---

## 9.5 按外部业务引用查询发件任务

**GET** `/api/mail-management/open-api/v1/send-tasks/by-external-ref/{externalBizRef}`

---

## 9.6 查询线程摘要

**GET** `/api/mail-management/open-api/v1/threads/{id}`

### 返回建议
仅返回开放场景需要的摘要字段：
- `id`
- `subject`
- `threadStatus`
- `latestMessageTime`
- `messageCount`
- `businessRelations`

---

## 9.7 查询邮件摘要列表

**GET** `/api/mail-management/open-api/v1/messages`

### 常用过滤
- `threadId`
- `mailAccountCode`
- `from`
- `to`
- `keyword`

---

## 9.8 建立业务关联

**POST** `/api/mail-management/open-api/v1/business-relations`

### 请求体
```json
{
  "ownerType": "THREAD",
  "ownerId": "uuid-thread",
  "businessType": "Order",
  "businessId": "SO20260425001",
  "businessCode": "SO20260425001",
  "displayName": "销售订单 SO20260425001",
  "isPrimary": true
}
```

---

## 9.9 查询审批状态

**GET** `/api/mail-management/open-api/v1/approvals/by-business/{businessId}`

---

## 9.10 查询 Webhook 投递结果

**GET** `/api/mail-management/open-api/v1/webhook-deliveries/{eventId}`

---

# 10. Webhook 事件推送设计

## 10.1 推送原则

Webhook 用于将关键业务事件通知外部系统。必须满足：

- 至少一次投递
- 签名校验
- 重试机制
- 事件唯一 ID
- 可追踪、可补发

## 10.2 事件清单

| 事件编码 | 说明 |
|---|---|
| `mail.received` | 新邮件接收完成 |
| `mail.sent` | 邮件发送成功 |
| `mail.send_failed` | 邮件发送失败 |
| `mail.approval.approved` | 审批通过 |
| `mail.approval.rejected` | 审批拒绝 |
| `mail.thread.assigned` | 线程分派变更 |
| `mail.business_relation.created` | 建立业务关联 |

## 10.3 Webhook 请求头

| Header | 说明 |
|---|---|
| `X-Event-Id` | 事件唯一 ID |
| `X-Event-Code` | 事件编码 |
| `X-Event-Time` | 事件时间 |
| `X-Signature` | 签名值 |
| `X-App-Key` | 应用标识 |
| `Content-Type` | application/json |

## 10.4 Webhook 负载结构

```json
{
  "eventId": "evt_20260425_000001",
  "eventCode": "mail.sent",
  "eventTime": "2026-04-25T09:30:00Z",
  "traceId": "00-xxxx",
  "data": {
    "sendTaskId": "uuid-send-task",
    "mailMessageId": "uuid-message",
    "threadId": "uuid-thread",
    "mailAccountId": "uuid-account",
    "subject": "客户回访通知",
    "sentAt": "2026-04-25T09:29:58Z",
    "externalBizRef": "CRM-OUT-20260425-0001"
  }
}
```

## 10.5 响应要求

外部系统收到 Webhook 后建议返回：

```json
{
  "code": "0",
  "message": "received"
}
```

HTTP 状态码：
- `2xx` 视为成功
- 非 `2xx` 视为失败并重试

## 10.6 重试策略

建议：

- 首次失败后 1 分钟重试
- 后续指数退避
- 最大 6 次
- 超过阈值进入死信并告警

---

# 11. 接口安全与审计要求

## 11.1 输入校验要求

所有写接口必须进行：

- 必填校验
- 长度校验
- 枚举合法性校验
- 权限校验
- 状态机校验
- 幂等校验

## 11.2 敏感接口要求

以下接口必须重点审计：

- 邮箱账号创建/修改
- 邮箱密钥轮换
- 发件
- 审批通过/拒绝
- 下载附件
- 创建/修改规则
- 创建/发布模板
- 创建/修改 webhook 订阅
- Open API 调用

## 11.3 限流建议

建议限流对象：

- Open API 写接口
- 附件下载接口
- 搜索接口
- 手工同步接口
- Webhook 重推接口

## 11.4 敏感字段脱敏

以下字段不得在普通日志中明文输出：

- 邮箱密码
- AccessToken / RefreshToken
- AppSecret
- Webhook 签名密钥
- HTML 正文中的敏感个人信息（按策略）
- 附件访问令牌

---

# 12. API 版本管理与兼容策略

## 12.1 版本策略

采用 URI 版本：

```text
/api/mail-management/v1
/api/mail-management/open-api/v1
```

后续新增大版本时：

```text
/api/mail-management/v2
```

## 12.2 向后兼容原则

在同一主版本内：

- 可新增响应字段
- 可新增非必填请求字段
- 不删除已有字段
- 不改变字段语义
- 不改变错误码含义

## 12.3 不兼容变更处理

若发生以下情况，应升级主版本：

- 路径变更
- 字段重命名
- 枚举语义变化
- 认证方式重大变化
- 响应结构变化

## 12.4 废弃策略

建议接口废弃时：

1. 文档中标注 `deprecated`
2. Swagger 标注废弃
3. 至少保留一个过渡周期
4. 对外系统提前通知

---

# 13. 测试与联调建议

## 13.1 Swagger / OpenAPI

建议所有内部管理 API 与 Open API 均输出 Swagger 文档，并按标签分组：

- MailAccounts
- Messages
- Threads
- SendTasks
- Approvals
- Collaboration
- Templates
- Rules
- Attachments
- BusinessRelations
- Integration
- Statistics
- OpenApi

## 13.2 接口测试重点

### 功能测试
- 正常 CRUD / 查询
- 状态流转
- 权限边界
- 幂等处理
- Webhook 重试

### 安全测试
- 越权访问
- 参数注入
- 签名伪造
- 敏感接口未授权访问
- 附件下载绕过

### 性能测试
- 线程列表分页
- 邮件详情加载
- 附件下载
- Open API 并发创建待发邮件
- Webhook 批量投递

## 13.3 Mock 建议

前后端联调阶段建议优先 Mock 以下接口：

- 线程列表
- 线程详情
- 发件任务详情
- 审批详情
- 模板预览
- 统计 Dashboard

## 13.4 自动化测试建议

建议为以下关键接口建立自动化回归用例：

- `POST /send-tasks`
- `POST /send-tasks/{id}/send`
- `POST /send-tasks/{id}/submit-approval`
- `POST /approvals/{id}/approve`
- `POST /threads/{id}/claim`
- `GET /threads`
- `GET /messages/{id}`
- `GET /attachments/{id}/download`
- `POST /open-api/v1/send-tasks`

---

# 14. 结论

邮件管理系统接口设计应以“**内部管理稳定、外部集成清晰、动作语义明确、幂等与安全前置**”为核心原则，形成以下统一规范：

1. **内部管理 API 与 Open API 分层设计**
2. **REST 风格路径统一，命令/查询语义清晰**
3. **统一响应结构、错误码、分页和时间格式**
4. **关键写接口支持幂等**
5. **Webhook 采用标准事件模型、签名和重试机制**
6. **权限控制、数据权限与审计要求贯穿所有核心接口**
7. **通过版本化与 Swagger 保证长期可维护和可演进**

该接口设计既适用于一期前后端联调和第三方系统集成，也能够支撑后续版本扩展、平台化开放和自动化测试体系建设。
