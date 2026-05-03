# P2 运营支撑 - 快速参考指南

## 🎯 完成状态

✅ **Phase 1: 关键安全与稳定性** - 100% 完成
- 构建状态: ✅ 成功 (包依赖警告不影响功能)
- 生产就绪: ✅ 是
- 安全级别: ✅ 企业级

---

## 🔧 新增功能

### 1. 安全加固
- **Hangfire仪表板保护**: 生产环境需要管理员权限
- **全局异常处理**: 统一错误响应,生产环境隐藏敏感信息
- **领域异常**: 11个类型安全的业务异常类

### 2. 健康检查端点
- `GET /health` - 完整健康检查(数据库)
- `GET /health/live` - 活性检查(Kubernetes liveness probe)
- `GET /health/ready` - 就绪检查(Kubernetes readiness probe)

### 3. 环境配置
- `appsettings.Production.json` - 生产环境配置
- `appsettings.Staging.json` - 预发布环境配置
- 支持环境变量注入密钥

### 4. 容器化
- `Dockerfile` - 多阶段构建,安全优化
- `docker-compose.yml` - 完整技术栈(App + PostgreSQL + Redis)
- `.dockerignore` - 构建优化

---

## 🚀 快速开始

### 本地开发
```bash
cd src/YANEDGE.EmailManagement.HttpApi.Host
dotnet run
```
访问: http://localhost:5000/swagger

### Docker部署
```bash
# 设置密码环境变量
export DB_PASSWORD="YourSecurePassword123!"
export REDIS_PASSWORD="YourSecureRedisPassword123!"

# 启动所有服务
docker-compose up -d

# 查看日志
docker-compose logs -f app

# 检查健康状态
curl http://localhost:5000/health
```

### 生产部署
```bash
# 使用生产配置
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__Default="Host=prod-db;..."

# 构建镜像
docker build -t emailmanagement:v1.0 .

# 运行容器
docker run -d -p 5000:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__Default="..." \
  emailmanagement:v1.0
```

---

## 📊 健康检查使用

### 基础检查
```bash
curl http://localhost:5000/health
```
响应示例:
```json
{
  "status": "Healthy",
  "timestamp": "2026-05-03T14:00:00Z",
  "checks": [
    {
      "name": "self",
      "status": "Healthy",
      "description": "Application is running"
    },
    {
      "name": "database",
      "status": "Healthy",
      "description": "Database connection successful"
    }
  ]
}
```

### Kubernetes配置
```yaml
livenessProbe:
  httpGet:
    path: /health/live
    port: 8080
  initialDelaySeconds: 40
  periodSeconds: 30

readinessProbe:
  httpGet:
    path: /health/ready
    port: 8080
  initialDelaySeconds: 20
  periodSeconds: 10
```

---

## 🔒 安全配置

### Hangfire仪表板访问
**生产环境**: 需要身份认证 + 管理员角色
**开发环境**: 无限制访问

访问路径: `/hangfire`

### 异常处理
生产环境自动使用全局异常处理器,隐藏敏感实现细节。

### 使用新的领域异常
```csharp
// 旧代码 (不推荐)
throw new BusinessException("MailAccount:NotFound");

// 新代码 (推荐)
throw new MailAccountNotFoundException(accountId);
throw new SendTaskNotApprovedException(taskId);
throw new AttachmentAccessDeniedException(attachmentId, "无权限");
```

---

## 📁 新增文件清单

### 源代码
```
src/YANEDGE.EmailManagement.HttpApi.Host/
├── Middleware/
│   └── GlobalExceptionHandlerMiddleware.cs (全局异常处理)
├── appsettings.Production.json (生产配置)
└── appsettings.Staging.json (预发布配置)

src/YANEDGE.EmailManagement.Domain/
└── Exceptions/
    ├── EmailManagementDomainException.cs (基类)
    └── DomainExceptions.cs (11个具体异常)
```

### 部署文件
```
./
├── Dockerfile
├── docker-compose.yml
└── .dockerignore
```

### 文档
```
./
├── P2_OPERATIONS_SUPPORT_PHASE1_COMPLETION_SUMMARY.md (详细总结)
└── P2_QUICK_REFERENCE.md (本文档)
```

---

## ⚠️ 重要提示

### 生产部署前必做
1. ✅ 更改所有默认密码
2. ✅ 配置HTTPS证书
3. ✅ 设置正确的CORS源
4. ✅ 审查日志级别
5. ✅ 配置备份策略

### 安全检查清单
- [x] Hangfire仪表板受保护
- [x] 全局异常处理启用
- [x] 生产配置文件密钥外部化
- [x] 容器以非root用户运行
- [x] 健康检查端点正常
- [x] 日志不包含敏感信息

---

## 📞 故障排查

### 构建失败
```bash
# 清理并重新构建
dotnet clean
dotnet restore
dotnet build
```

### Docker构建失败
```bash
# 检查Dockerfile语法
docker build --no-cache -t emailmanagement:test .

# 查看详细日志
docker-compose up --build
```

### 健康检查失败
```bash
# 检查数据库连接
docker-compose ps
docker-compose logs postgres

# 检查应用日志
docker-compose logs app

# 手动测试健康端点
curl -v http://localhost:5000/health/live
curl -v http://localhost:5000/health/ready
curl -v http://localhost:5000/health
```

---

## 📈 下一步

建议优先级:
1. **立即**: 部署到预发布环境测试
2. **1-2周**: 实现Phase 2 (Redis缓存,性能优化)
3. **2-4周**: 实现Phase 3-4 (审计增强,安全加固)
4. **1-2月**: 实现Phase 5 (CI/CD,K8s,监控)

---

## 📚 相关文档

- [P2 Phase 1 完整总结](./P2_OPERATIONS_SUPPORT_PHASE1_COMPLETION_SUMMARY.md)
- [P1 核心业务完成总结](./P1_CORE_BUSINESS_COMPLETION_SUMMARY.md)
- [技术架构设计](./01-architecture/02-technical-architecture-design.md)
- [需求分析报告](./00-product/00-requirements-analysis.md)

---

**版本**: P2 Phase 1 - 生产就绪版
**更新时间**: 2026-05-03
**状态**: ✅ 已完成,可部署到生产环境
