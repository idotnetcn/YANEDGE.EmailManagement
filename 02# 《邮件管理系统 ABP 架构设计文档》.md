## 🤖 Assistant

下面我直接继续输出：

# 《邮件管理系统 ABP 架构设计文档》

---

# 邮件管理系统 ABP 架构设计文档

> 适用技术栈：
>
> - `.NET 10.x`
> - `ABP 10.x`
> - `PostgreSQL`
>
> 适用目标：
>
> - 企业内部邮件管理
> - 独立部署
> - 可与 ERP/CRM 集成
> - 模块化、可维护、可扩展
> - 可作为未来 ERP/CRM 平台基础模块演进

---

# 1. 文档目标

## 1.1 编写目的
本文档用于定义邮件管理系统的整体技术架构、模块边界、ABP 分层设计、项目结构、集成方式与扩展策略，作为后续详细设计、开发实施、代码规范和部署落地的依据。

## 1.2 架构目标
本系统架构设计需满足以下核心目标：

- 支持企业级邮件统一管理
- 支持与 ERP/CRM 低耦合集成
- 支持模块化演进与业务扩展
- 支持长期维护与团队协作开发
- 支持后续平台化、服务化拆分
- 满足权限、安全、审计、可观测要求

---

# 2. 总体架构原则

## 2.1 设计原则

### 2.1.1 模块化优先
系统必须按业务领域拆分模块，避免“大一统业务类”与“全局数据库耦合”问题。

### 2.1.2 单体优先，预留服务化
一期建议采用 **模块化单体架构**，在 ABP 框架下通过模块隔离实现清晰边界，后续可逐步拆分为独立服务。

### 2.1.3 领域驱动
围绕邮件、线程、账号、规则、模板、协同、业务关联、审计等核心领域建立模型，而不是围绕页面堆砌代码。

### 2.1.4 应用与集成分离
系统内部业务逻辑与外部 ERP/CRM 集成适配逻辑必须分离，避免外部系统耦合污染核心领域。

### 2.1.5 异步优先
邮件同步、发送、回调、规则处理、统计聚合等高耗时操作应采用后台任务或事件驱动机制。

### 2.1.6 安全与审计前置
不是“开发完成后再补安全”，而是在架构层就引入权限、审计、日志、脱敏、访问控制能力。

---

# 3. 总体架构选型

## 3.1 推荐架构模式
建议采用：

**ABP 模块化单体 + 分层架构 + 后台任务 + 事件驱动扩展**

即：

- 前台/后台管理 UI
- HttpApi 对外接口层
- Application 应用服务层
- Domain 领域层
- EF Core 持久化层
- Background Workers/Jobs 异步任务层
- Integration 集成适配层

---

## 3.2 架构选择理由

### 适合一期快速落地
相比直接微服务化，模块化单体能够更快交付，降低部署、网络通信、分布式事务和运维复杂度。

### 更符合 ABP 最佳实践
ABP 天然适合分层 + 模块化设计，能够较好支撑权限、审计、多租户、后台任务、Blob 存储等能力。

### 后续拆分空间大
只要模块边界设计合理，未来可以把如下模块拆出为独立服务：

- 邮件同步服务
- 邮件发送服务
- 搜索服务
- 集成服务
- 统计分析服务

---

# 4. 系统逻辑架构

## 4.1 逻辑分层图

```text
表现层
├─ Web 管理后台
├─ API 接口
└─ 第三方系统调用入口

应用层
├─ AppService
├─ 查询服务
├─ 权限校验
├─ DTO 转换
└─ 事务编排

领域层
├─ 聚合根
├─ 实体
├─ 值对象
├─ 领域服务
├─ 领域事件
└─ 业务规则

基础设施层
├─ EF Core
├─ PostgreSQL
├─ Blob/File Storage
├─ 缓存
├─ 日志
├─ 邮件协议适配
└─ 外部系统集成适配

异步处理层
├─ 邮件收件同步任务
├─ 发件任务
├─ 审批提醒任务
├─ 规则处理任务
├─ Webhook 回调任务
└─ 统计聚合任务
```

---

## 4.2 外部交互架构图

```text
企业邮箱/外部邮件服务器
   ├─ IMAP/POP3 收件
   └─ SMTP/API 发件

邮件管理系统
   ├─ 核心业务模块
   ├─ 协同与审批
   ├─ 模板规则
   ├─ 业务关联
   ├─ 审计日志
   └─ 开放接口

ERP / CRM / OA / BI
   ├─ API 调用
   ├─ Webhook 回调
   └─ 事件订阅
```

---

# 5. ABP 解决方案结构设计

## 5.1 推荐解决方案结构

建议采用主应用 + 领域模块目录结构。

```text
src
├─ MailManagement.Domain.Shared
├─ MailManagement.Domain
├─ MailManagement.Application.Contracts
├─ MailManagement.Application
├─ MailManagement.EntityFrameworkCore
├─ MailManagement.HttpApi
├─ MailManagement.HttpApi.Client
├─ MailManagement.Web
├─ MailManagement.DbMigrator
└─ MailManagement.TestBase
```

如果希望边界更清晰，建议进一步拆分模块：

```text
modules
├─ MailManagement.MailAccounts
├─ MailManagement.MailCore
├─ MailManagement.MailWorkflow
├─ MailManagement.MailTemplates
├─ MailManagement.MailRules
├─ MailManagement.MailRelations
├─ MailManagement.MailAttachments
├─ MailManagement.MailIntegration
├─ MailManagement.MailAudit
└─ MailManagement.MailReporting
```

---

## 5.2 推荐项目组织方式

### 方案 A：单解决方案多层结构
适合一期团队规模较小、快速交付。

### 方案 B：单解决方案 + 内部模块包结构
适合中长期维护，推荐采用。

如果项目生命周期较长，建议从一开始按模块拆分项目，即使先仍然部署为一个应用。

---

# 6. 模块划分设计

---

## 6.1 模块总览

| 模块 | 职责 | 是否一期核心 |
|---|---|---|
| Identity / Organization | 用户、角色、组织、权限 | 是 |
| MailAccounts | 邮箱账号接入与配置 | 是 |
| MailCore | 邮件收发、线程、状态 | 是 |
| MailWorkflow | 分派、协同、审批、备注 | 是 |
| MailTemplates | 模板、签名、变量 | 是 |
| MailRules | 自动分类、分派、黑白名单 | 是 |
| MailRelations | 联系人、业务对象关联 | 是 |
| MailAttachments | 附件存储与访问控制 | 是 |
| MailIntegration | ERP/CRM 对接、Webhook、幂等 | 是 |
| MailAudit | 审计与合规 | 是 |
| MailReporting | 统计分析 | 建议一期基础版 |
| Notification | 系统消息与提醒 | 建议一期基础版 |
| Search | 搜索增强 | 一期可轻量实现 |

---

## 6.2 各模块设计说明

### 6.2.1 Identity / Organization 模块
#### 主要职责
- 用户、角色、权限管理
- 组织、部门、岗位
- 数据权限范围
- 邮箱访问授权
- 共享邮箱可见性控制

#### 技术建议
直接复用 ABP：
- Identity
- Permission Management
- Setting Management
- Tenant Management（预留）

---

### 6.2.2 MailAccounts 模块
#### 主要职责
- 邮箱账号配置
- IMAP/POP3/SMTP 参数管理
- OAuth2/密码/授权码管理
- 连接测试
- 收件策略
- 发件策略
- 健康检查

#### 核心对象
- `MailAccount`
- `MailAccountCredential`
- `MailSyncPolicy`
- `MailAccountHealthLog`

#### 模块边界
只负责邮箱接入和账号配置，不直接承担具体邮件业务流转。

---

### 6.2.3 MailCore 模块
#### 主要职责
- 邮件主数据管理
- 收件、发件、草稿、归档
- 邮件线程聚合
- 邮件状态控制
- 邮件正文解析
- 收件人管理

#### 核心对象
- `MailMessage`
- `MailThread`
- `MailRecipient`
- `MailFolder`
- `MailHeader`
- `MailSendTask`
- `MailSyncTask`

#### 关键原则
MailCore 是整个系统最核心领域，不应引入外部 ERP/CRM 业务概念，只保留通用邮件能力。

---

### 6.2.4 MailWorkflow 模块
#### 主要职责
- 邮件分派
- 协同处理
- 内部备注
- 状态流转
- 审批机制
- SLA 提醒

#### 核心对象
- `MailAssignment`
- `MailInternalComment`
- `MailApproval`
- `MailProcessingRecord`
- `MailTodo`

#### 边界说明
只关心“邮件如何被协同处理”，不关心外部业务对象本体。

---

### 6.2.5 MailTemplates 模块
#### 主要职责
- 邮件模板
- 模板分类
- 模板变量
- 邮件签名
- 模板渲染
- 模板版本管理

#### 核心对象
- `MailTemplate`
- `MailTemplateVersion`
- `MailTemplateCategory`
- `MailTemplateVariable`
- `MailSignature`

#### 扩展原则
模板变量来源可由外部系统注入，但变量解析规则必须由本模块统一管理。

---

### 6.2.6 MailRules 模块
#### 主要职责
- 自动分类
- 自动标签
- 自动分派
- 黑白名单
- 可信域
- 规则执行日志

#### 核心对象
- `MailRule`
- `MailRuleCondition`
- `MailRuleAction`
- `MailRuleExecutionLog`
- `MailDomainPolicy`

#### 设计建议
一期不建议做复杂规则引擎 DSL，可使用“条件 + 动作”配置模型，后续再增强。

---

### 6.2.7 MailRelations 模块
#### 主要职责
- 联系人管理
- 邮件与客户/订单/合同/工单等对象关联
- 自动匹配联系人
- 业务关联查询

#### 核心对象
- `Contact`
- `BusinessObjectRelation`
- `BusinessObjectType`

#### 边界说明
不管理 ERP/CRM 数据本体，只管理“邮件与业务对象关系”。

---

### 6.2.8 MailAttachments 模块
#### 主要职责
- 附件元数据管理
- 附件访问控制
- 附件下载审计
- 存储适配抽象

#### 核心对象
- `MailAttachment`
- `AttachmentAccessLog`

#### 存储建议
- 元数据：PostgreSQL
- 文件内容：ABP Blob Storing / 本地文件 / MinIO / OSS / S3

---

### 6.2.9 MailIntegration 模块
#### 主要职责
- 对外开放 API
- Webhook 回调
- 幂等控制
- 外部系统映射
- 事件日志

#### 核心对象
- `IntegrationApp`
- `IntegrationWebhook`
- `ExternalMapping`
- `IdempotencyRecord`
- `IntegrationEventLog`

#### 关键原则
所有与 ERP/CRM 的对接入口统一通过该模块暴露，避免各模块随意开放接口。

---

### 6.2.10 MailAudit 模块
#### 主要职责
- 操作审计
- 配置变更审计
- 敏感访问留痕
- 附件下载审计
- API 调用审计

#### 核心对象
- `MailAuditLog`
- `ConfigChangeLog`
- `SensitiveOperationLog`

---

### 6.2.11 MailReporting 模块
#### 主要职责
- 收发量统计
- 处理量统计
- 响应时效分析
- 失败任务统计
- 规则命中统计

#### 建议
一期先做基础统计聚合；后续再增加报表快照与 BI 输出能力。

---

# 7. 分层设计

## 7.1 ABP 标准分层

| 层 | 责任 |
|---|---|
| Domain.Shared | 共享常量、枚举、错误码、权限名 |
| Domain | 聚合根、实体、值对象、领域服务、仓储接口 |
| Application.Contracts | DTO、应用服务接口、权限声明 |
| Application | 应用服务实现、事务编排、用例处理 |
| EntityFrameworkCore | EF Core 映射、仓储实现、数据库迁移 |
| HttpApi | 控制器、API 入口、Swagger |
| Web | 后台管理 UI |
| DbMigrator | 数据迁移初始化 |

---

## 7.2 各层详细职责

### 7.2.1 Domain.Shared
#### 存放内容
- 枚举定义
- 常量
- 权限名称常量
- 本地化资源键
- 错误码

#### 示例
```csharp
public static class MailPermissions
{
    public const string GroupName = "MailManagement";
    public const string InboxView = GroupName + ".Inbox.View";
    public const string SendCreate = GroupName + ".Send.Create";
}
```

---

### 7.2.2 Domain
#### 存放内容
- 聚合根
- 业务规则
- 领域服务
- 仓储接口
- 领域事件

#### 注意事项
- 不应依赖 UI
- 不应依赖 DTO
- 不应包含具体数据库访问实现
- 不直接处理第三方系统 HTTP 调用

---

### 7.2.3 Application.Contracts
#### 存放内容
- 输入输出 DTO
- 查询 DTO
- 应用服务接口
- 远程服务契约

---

### 7.2.4 Application
#### 存放内容
- AppService 实现
- 权限检查
- DTO 转换
- 业务流程编排
- 调用领域服务
- 发布集成事件

#### 示例职责
- 创建待发邮件任务
- 查询线程详情
- 为邮件绑定业务对象
- 发起审批流

---

### 7.2.5 EntityFrameworkCore
#### 存放内容
- `DbContext`
- Entity 映射
- Repository 实现
- 索引配置
- 数据迁移

#### 建议
每个模块单独配置实体映射，避免 `DbContext` 代码膨胀。

---

### 7.2.6 HttpApi
#### 存放内容
- REST API 控制器
- 路由版本化
- 输入验证
- Swagger/OpenAPI 暴露

---

### 7.2.7 Web
#### 存放内容
- 后台 UI
- 管理台页面
- 工作台
- 列表查询页
- 审批与处理页

---

# 8. 领域模型设计建议

## 8.1 核心聚合建议

### 8.1.1 MailAccount 聚合
负责邮箱账号与认证配置。

**聚合根：**
- `MailAccount`

**子实体/值对象：**
- `CredentialInfo`
- `SyncPolicy`
- `ConnectionSettings`

---

### 8.1.2 MailThread 聚合
负责线程视角下的状态、责任人、处理概况。

**聚合根：**
- `MailThread`

**包含：**
- 线程状态
- 负责人
- 未读数
- 最新消息时间
- 优先级

---

### 8.1.3 MailMessage 聚合
负责单封邮件的核心数据与状态。

**聚合根：**
- `MailMessage`

**子实体：**
- `MailRecipient`
- `MailHeader`
- `MailAttachmentRef`

---

### 8.1.4 MailRule 聚合
负责规则定义与执行配置。

**聚合根：**
- `MailRule`

**子实体：**
- `MailRuleCondition`
- `MailRuleAction`

---

### 8.1.5 MailTemplate 聚合
负责模板定义与版本管理。

**聚合根：**
- `MailTemplate`

**子实体：**
- `MailTemplateVersion`

---

### 8.1.6 BusinessObjectRelation 聚合
负责业务对象关联关系。

**聚合根：**
- `BusinessObjectRelation`

---

## 8.2 领域服务建议

以下逻辑建议用领域服务实现，而不是塞进 AppService：

- 邮件线程归并服务
- 规则命中判定服务
- 邮件自动关联服务
- 模板渲染服务
- 发件审批判定服务
- SLA 计算服务

---

## 8.3 领域事件建议

可定义如下领域事件：

- `MailReceivedEvent`
- `MailSentEvent`
- `MailSendFailedEvent`
- `MailAssignedEvent`
- `MailApprovalSubmittedEvent`
- `MailRuleMatchedEvent`
- `BusinessRelationBoundEvent`

作用：
- 解耦模块逻辑
- 便于未来对接分布式事件总线
- 提升扩展能力

---

# 9. 模块依赖关系设计

## 9.1 推荐依赖方向

```text
Identity / Organization
    ↑
MailAccounts
    ↑
MailCore
    ↑
MailWorkflow
    ↑
MailTemplates
    ↑
MailRules
    ↑
MailRelations
    ↑
MailAttachments
    ↑
MailIntegration
    ↑
MailAudit / MailReporting
```

更准确地说，依赖应尽量单向、收敛到核心。

---

## 9.2 依赖原则
- `MailCore` 不依赖 ERP/CRM 集成模块
- `MailIntegration` 可以依赖 `MailCore` / `MailRelations`
- `MailAudit` 可监听其他模块事件，但其他模块不依赖 `MailAudit`
- `MailReporting` 只读聚合，不应反向影响核心业务

---

# 10. 应用服务设计建议

## 10.1 AppService 分类

建议分为三类：

### 10.1.1 Command AppService
处理写操作：
- 创建发件任务
- 更新规则
- 分派邮件
- 提交审批
- 归档邮件

### 10.1.2 Query AppService
处理读操作：
- 查询邮件列表
- 查询线程详情
- 查询附件列表
- 查询统计数据

### 10.1.3 Integration AppService
处理外部系统调用：
- ERP/CRM 创建待发邮件
- 按业务对象查询邮件
- 接收外部业务对象同步

---

## 10.2 应用服务命名建议

| 类型 | 命名示例 |
|---|---|
| 邮件管理 | `MailMessageAppService` |
| 线程查询 | `MailThreadQueryAppService` |
| 规则管理 | `MailRuleAppService` |
| 模板管理 | `MailTemplateAppService` |
| 集成接口 | `MailIntegrationAppService` |

---

# 11. 邮件收发架构设计

## 11.1 收件流程架构

```text
后台同步任务
→ 连接 IMAP
→ 获取未同步邮件
→ 解析邮件头/正文/附件
→ 持久化邮件
→ 线程归并
→ 执行规则
→ 自动关联业务对象
→ 生成待办/提醒/审计
```

### 技术建议
- 使用独立同步服务类
- 每个邮箱账号独立同步上下文
- 使用 UID 或 Message-ID 防重
- 同步结果写入任务日志

---

## 11.2 发件流程架构

```text
用户/系统提交发件请求
→ 创建 MailMessage + MailSendTask
→ 审批判定
→ 发件任务入队
→ SMTP/API 发送
→ 更新状态
→ 记录审计日志
→ 回调 ERP/CRM
```

### 技术建议
- 发件应异步化
- 允许失败重试
- 外部系统调用必须幂等
- 发件成功后回写 `internet_message_id`

---

# 12. 后台任务与异步处理设计

## 12.1 建议任务类型

| 任务 | 说明 |
|---|---|
| 邮箱同步任务 | 周期性拉取邮件 |
| 发件任务 | 处理待发邮件 |
| 失败重试任务 | 重发失败邮件 |
| Webhook 回调任务 | 异步通知外部系统 |
| 规则重算任务 | 特定场景批量重跑规则 |
| 审批提醒任务 | 超时审批通知 |
| SLA 检查任务 | 超时待办提醒 |
| 归档清理任务 | 清理历史任务日志 |

---

## 12.2 ABP 实现建议
可使用：

- `IBackgroundJobManager`
- `BackgroundWorkerBase`
- 定时调度组件（如 Hangfire/Quartz，视项目规范）

### 建议
- 简单任务：ABP Background Jobs
- 定时复杂任务：Quartz/Hangfire 更适合

---

# 13. 数据访问架构设计

## 13.1 持久化方案
- 结构化数据：PostgreSQL
- 附件文件：Blob/Object Storage
- 配置：ABP Setting Management
- 缓存：Redis（建议）
- 搜索：一期 PG 全文检索，后续可升级

---

## 13.2 EF Core 设计建议

### 建议一：模块化映射
每个模块独立 EntityTypeConfiguration。

### 建议二：只在需要时使用导航属性
避免大面积复杂导航导致查询不可控。

### 建议三：读写分离思维
虽然一期未必物理分离，但代码层应区分“写模型”和“读模型查询”。

---

## 13.3 数据一致性策略
- 单模块内：数据库事务
- 跨模块：领域事件 + 最终一致性
- 对外系统：幂等 + 重试 + 补偿

---

# 14. 权限与数据权限架构

## 14.1 权限模型
建议采用：

**RBAC + 数据范围控制**

即：
- 功能权限：菜单/按钮/API 权限
- 数据权限：用户可以看到哪些邮箱、哪些邮件、哪些附件

---

## 14.2 权限层级建议

### 一级：模块权限
- 邮件中心
- 模板中心
- 规则中心
- 集成中心

### 二级：操作权限
- 查看
- 创建
- 编辑
- 删除
- 发送
- 审批
- 分派
- 下载附件

### 三级：数据权限
- 仅本人
- 本部门
- 授权邮箱
- 指定共享邮箱
- 全部

---

## 14.3 数据权限控制点
以下场景必须做数据权限校验：

- 查询邮件列表
- 查看邮件详情
- 下载附件
- 查看共享邮箱
- 查看线程协同记录
- 查询统计报表

---

# 15. 审计与日志架构

## 15.1 审计范围
必须记录的操作：

- 邮件查看
- 邮件发送
- 邮件删除
- 邮件归档
- 邮件分派
- 审批操作
- 模板修改
- 规则修改
- 邮箱账号配置修改
- 附件下载
- API 外部调用

---

## 15.2 日志分类

| 日志类型 | 用途 |
|---|---|
| 运行日志 | 程序错误与调试 |
| 业务日志 | 邮件收发与规则执行 |
| 审计日志 | 用户操作留痕 |
| 集成日志 | ERP/CRM 接口交互 |
| 任务日志 | 后台任务执行情况 |

---

## 15.3 日志建议
- 统一 `TraceId`
- API 请求响应日志脱敏
- 关键异常保留上下文
- 审计日志不可随意物理删除

---

# 16. 集成架构设计

## 16.1 集成方式
推荐三种方式并存：

### 1）REST API
适合同步查询、创建待发任务、读取状态。

### 2）Webhook
适合结果通知、邮件发送状态回调、审批状态通知。

### 3）事件驱动
适合同步量大、解耦需求强的系统联动。

---

## 16.2 集成原则
- 接口版本化
- 请求幂等
- 安全鉴权
- 超时控制
- 重试机制
- 回放能力
- 失败可补偿

---

## 16.3 外部主数据处理原则
对于客户、订单、合同、工单等对象：

**邮件系统只保存引用与关系，不保存完整业务主数据。**

原因：
- 避免双主数据系统
- 降低一致性维护成本
- 便于集成不同 ERP/CRM

---

# 17. 搜索架构设计

## 17.1 一期建议
基于 PostgreSQL 实现以下搜索：

- 主题搜索
- 发件人搜索
- 收件人搜索
- 正文全文搜索
- 标签筛选
- 时间范围筛选
- 业务对象筛选

---

## 17.2 二期建议
当出现以下情况时引入独立搜索引擎：

- 正文检索量大
- 附件全文检索需求强
- 多维筛选复杂
- 响应时间明显变差

可选：
- Elasticsearch
- OpenSearch

---

# 18. 文件与附件存储架构

## 18.1 设计原则
- 数据库只存附件元数据
- 文件内容不建议大规模直接入库
- 支持对象存储抽象
- 下载必须鉴权
- 敏感附件必须记录下载日志

---

## 18.2 推荐方案
一期推荐：
- 本地文件系统 / MinIO
- ABP BlobStoring 抽象统一封装

后续可切换：
- 阿里云 OSS
- 腾讯云 COS
- AWS S3

---

# 19. 可扩展性设计

## 19.1 为未来 ERP/CRM 演进预留的扩展点

### 业务对象类型扩展
支持新增：
- 商机
- 报价单
- 项目
- 结算单
- 发票
- 自定义对象

### 流程扩展
支持从邮件协同扩展到：
- 工单
- 审批流
- 客户服务流程
- 合同审批通知

### 平台扩展
未来可在同平台上增加：
- CRM 客户管理
- ERP 订单管理
- 项目管理
- 文档中心

---

## 19.2 微服务演进预留
后续可优先拆分以下模块：

1. `MailIntegration Service`
2. `MailSync Service`
3. `MailSend Service`
4. `MailSearch Service`
5. `MailReporting Service`

拆分前提：
- 模块边界清晰
- 接口契约稳定
- 事件消息设计成熟

---

# 20. 部署架构建议

## 20.1 一期部署建议
建议采用标准三层部署：

```text
Nginx / 反向代理
    ↓
ABP Web / HttpApi 应用
    ↓
PostgreSQL + Redis + Blob Storage
```

---

## 20.2 生产环境建议组件

| 组件 | 建议 |
|---|---|
| Web/API | 2 实例起步，支持水平扩展 |
| PostgreSQL | 主从/备份 |
| Redis | 会话、缓存、分布式锁 |
| Blob Storage | 本地/NAS/MinIO |
| 日志中心 | ELK / Loki / Seq |
| 监控 | Prometheus + Grafana |
| 任务调度 | Hangfire / Quartz |

---

# 21. 开发规范建议

## 21.1 代码组织规范
- 每个模块独立目录
- DTO 与 Entity 分离
- AppService 不直接写复杂业务规则
- Repository 不写业务逻辑
- 领域服务负责核心规则

---

## 21.2 命名规范
- 模块名统一前缀 `Mail`
- 表名统一 `snake_case`
- API 路径统一 `/api/mail-management/v1/...`
- 权限名称统一层级化

---

## 21.3 异常规范
统一业务异常码：
- 邮箱配置错误
- 邮件发送失败
- 权限不足
- 附件不存在
- 规则配置非法
- 幂等冲突

---

# 22. 风险点与架构应对

| 风险 | 说明 | 架构应对 |
|---|---|---|
| 邮件协议兼容复杂 | 各邮箱厂商细节不同 | 增加适配层，不污染领域层 |
| 邮件量增长快 | 查询和存储压力大 | 索引、归档、分区、搜索引擎预留 |
| 附件膨胀 | 存储成本高 | 附件外置存储 |
| 外部集成差异大 | ERP/CRM 不统一 | Integration 模块统一抽象 |
| 权限越权 | 共享邮箱复杂 | RBAC + 数据范围双重控制 |
| 线程归并误判 | 邮件主题变化导致错误归并 | 优先 Message-ID 链路归并 |
| 异步任务失败 | 发件/回调丢失风险 | 重试、死信、补偿日志 |

---

# 23. 推荐实施顺序

## 23.1 优先落地模块
一期建议按照以下顺序开发：

1. Identity / Organization
2. MailAccounts
3. MailCore
4. MailAttachments
5. MailWorkflow
6. MailTemplates
7. MailRules
8. MailRelations
9. MailIntegration
10. MailAudit
11. MailReporting

---

## 23.2 原因
这样可以先打通系统最核心的“收、发、查、管”闭环，再逐步叠加协同、规则、集成与审计能力。

---

# 24. 架构结论

本系统最适合的架构路线为：

## 核心结论
- **采用 ABP 模块化单体架构**
- **围绕邮件核心领域进行模块划分**
- **以异步任务支撑同步、发件、回调、统计**
- **以 Integration 模块统一 ERP/CRM 对接**
- **以 MailRelations 模块管理业务对象关联**
- **以 Audit + Permission 作为底座能力前置设计**
- **为未来平台化与微服务演进保留清晰边界**

这条路线兼顾了：

- 当前快速落地
- 中期维护成本
- 后续 ERP/CRM 演进空间
- 多系统集成灵活性

---

如果你愿意，我下一步可以继续直接输出以下其中一个：

1. **《邮件管理系统数据库设计文档（详细版）》**
2. **《邮件管理系统领域模型设计文档》**
3. **《邮件管理系统 API 详细设计文档》**
4. **《邮件管理系统开发任务分解 WBS + 人天估算》**
5. **《邮件管理系统菜单、权限点、角色矩阵设计文档》**

如果你不想再选，我也可以直接按最合理顺序，下一步继续输出：

> **《邮件管理系统数据库设计文档（详细版）》**
