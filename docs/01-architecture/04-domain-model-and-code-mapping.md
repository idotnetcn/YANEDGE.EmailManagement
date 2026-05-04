
# 邮件管理系统领域模型与模块设计文档

> 项目名称：邮件管理系统
> 技术路线：.NET 10.x / ABP 10.x / PostgreSQL
> 文档版本：V1.0
> 文档属性：领域模型与模块设计文档
> 适用阶段：领域建模 / 模块拆分 / 应用服务设计 / 代码结构设计 / 开发实施

---

## 修订记录

| 版本 | 日期 | 作者 | 说明 |
|---|---|---|---|
| V1.0 | 2026-04-25 | 项目组 | 初始版本 |

---

## 目录

- [1. 文档目标](#1-文档目标)
- [2. 设计原则](#2-设计原则)
- [3. 领域划分总览](#3-领域划分总览)
- [4. 上下文边界设计](#4-上下文边界设计)
- [5. 模块拆分设计](#5-模块拆分设计)
- [6. 核心领域模型设计](#6-核心领域模型设计)
- [7. 聚合设计](#7-聚合设计)
- [8. 实体与值对象设计](#8-实体与值对象设计)
- [9. 领域服务设计](#9-领域服务设计)
- [10. 领域事件设计](#10-领域事件设计)
- [11. 仓储接口设计](#11-仓储接口设计)
- [12. 应用层用例设计](#12-应用层用例设计)
- [13. 模块协作关系设计](#13-模块协作关系设计)
- [14. 状态流转领域约束](#14-状态流转领域约束)
- [15. 权限与数据可见性落点](#15-权限与数据可见性落点)
- [16. 代码组织设计建议](#16-代码组织设计建议)
- [17. 开发实施建议](#17-开发实施建议)
- [18. 演进规划](#18-演进规划)
- [19. 结论](#19-结论)

---

# 1. 文档目标

本文档用于在技术架构和数据库设计基础上，进一步明确邮件管理系统的领域模型与模块设计，回答以下关键问题：

1. 系统应划分为哪些核心领域模块
2. 每个模块的职责边界是什么
3. 哪些对象应设计为聚合根、实体、值对象
4. 模块之间如何协作，如何避免强耦合
5. 哪些规则应放在领域层，哪些应放在应用层
6. 如何为后续代码实现、测试和服务拆分打下稳定基础

本文档是以下工作的直接输入：

- Domain 层实体建模
- Application 层用例设计
- Repository 接口设计
- 事件模型设计
- 代码目录结构设计
- 单元测试边界划分

---

# 2. 设计原则

## 2.1 先领域后技术

模块设计优先围绕业务概念，而不是围绕页面、数据库表或接口 URL 进行拆分。
即使最终落地为单体项目，也应先形成稳定的业务边界。

## 2.2 核心域与支撑域分离

本系统的核心竞争力不在“简单收发邮件”，而在：

- 线程化处理
- 共享邮箱协同
- 审批与责任流转
- 业务对象关联
- 模板与规则自动化

因此需要明确：

- **核心域**：直接决定系统业务价值的部分
- **支撑域**：为核心域提供支撑能力的部分
- **通用域**：权限、审计、配置、集成等平台能力

## 2.3 聚合边界清晰

聚合设计遵循以下原则：

- 聚合根负责维护内部一致性
- 聚合之间通过 ID 引用，不直接持有彼此内部实体
- 尽量缩小事务边界，避免超大聚合
- 高频查询对象与高频变更对象适度解耦

## 2.4 应用层编排，领域层约束

- **领域层**负责表达业务规则、不变量、状态机和业务语义
- **应用层**负责协调流程、权限校验、事务管理、外部调用和 DTO 转换

## 2.5 事件驱动解耦优先

对于“邮件入库后触发规则执行、业务关联、统计更新、Webhook 回调”等场景，不宜让一个模块同步直接调用所有其他模块，而应通过领域事件或集成事件解耦。

## 2.6 先模块化单体，后服务化演进

一期项目以模块化单体为主，但每个模块应具备未来拆分为独立服务的潜力。

---

# 3. 领域划分总览

## 3.1 领域分类

系统领域建议划分为三层：

### 3.1.1 核心域
- 邮件域（MailMessage / MailThread / MailCompose）
- 协同域（Collaboration）
- 审批域（Approval）
- 业务关联域（BusinessRelation）

### 3.1.2 支撑域
- 邮箱账号域（MailAccount）
- 模板域（Template）
- 规则域（RuleEngine）
- 附件域（Attachment）
- 统计域（Statistics）

### 3.1.3 通用域
- 身份与组织域（Identity / Organization）
- 集成域（Integration）
- 审计安全域（AuditSecurity）
- 系统配置域（SystemSetting）
- 通知域（Notification，可后续增强）

## 3.2 核心域价值说明

| 领域 | 核心价值 |
|---|---|
| MailThread | 把单封邮件提升为可协同处理的业务线程 |
| Collaboration | 解决共享邮箱“看得到但没人负责”的问题 |
| Approval | 解决企业敏感外发与合规控制 |
| BusinessRelation | 让邮件进入客户、订单、合同、工单上下文 |

---

# 4. 上下文边界设计

## 4.1 边界上下文划分

建议形成以下边界上下文（Bounded Context）：

```text
[Identity / Organization]
[Mail Account Context]
[Mail Messaging Context]
[Mail Collaboration Context]
[Mail Approval Context]
[Mail Template Context]
[Mail Rule Context]
[Business Relation Context]
[Attachment Context]
[Integration Context]
[Statistics Context]
[Audit Security Context]
[System Setting Context]
```

## 4.2 边界划分说明

### 4.2.1 Mail Account Context
负责邮箱账号纳管、连接信息、同步配置、授权摘要。

### 4.2.2 Mail Messaging Context
负责邮件、正文、地址、线程、发件任务等核心对象。

### 4.2.3 Mail Collaboration Context
负责认领、分派、转派、备注、待办、协同活动日志。

### 4.2.4 Mail Approval Context
负责待审批邮件、审批流程、审批记录、审批策略命中。

### 4.2.5 Mail Template Context
负责模板、模板版本、签名、变量定义。

### 4.2.6 Mail Rule Context
负责自动分类、自动分派、黑白名单、规则执行日志。

### 4.2.7 Business Relation Context
负责联系人、外部业务对象映射、邮件与业务对象关联。

### 4.2.8 Attachment Context
负责附件元数据、访问控制、下载日志、预览状态。

### 4.2.9 Integration Context
负责外部系统接入、AppKey、Webhook、幂等、回调日志。

### 4.2.10 Statistics Context
负责统计口径、聚合计算、日报表。

### 4.2.11 Audit Security Context
负责审计记录、安全策略、敏感操作留痕。

---

# 5. 模块拆分设计

## 5.1 模块清单

一期建议使用以下模块：

```text
MailAccount
MailMessage
MailThread
MailCompose
Collaboration
Approval
Template
RuleEngine
Attachment
BusinessRelation
Integration
Statistics
AuditSecurity
SystemSetting
```

## 5.2 模块职责说明

### 5.2.1 MailAccount 模块
负责：

- 邮箱账号建档
- 连接参数管理
- 启停同步/发件
- 账号授权摘要
- 同步文件夹状态
- 连接测试

### 5.2.2 MailMessage 模块
负责：

- 单封邮件元数据
- 正文内容
- 邮件地址
- 收件/发件的基础邮件对象
- 邮件入库标准化

### 5.2.3 MailThread 模块
负责：

- 线程归并
- 线程状态
- 线程上下文
- 线程摘要
- 最近活动聚合信息

### 5.2.4 MailCompose 模块
负责：

- 草稿
- 待发邮件
- 定时发件
- 发件重试
- 发件执行状态

### 5.2.5 Collaboration 模块
负责：

- 认领
- 分派
- 转派
- 内部备注
- 协同时间线
- 待办生成

### 5.2.6 Approval 模块
负责：

- 审批单
- 审批记录
- 审批流转
- 外发审批约束

### 5.2.7 Template 模块
负责：

- 模板
- 版本
- 签名
- 变量与渲染上下文定义

### 5.2.8 RuleEngine 模块
负责：

- 规则定义
- 条件匹配
- 规则动作输出
- 执行日志

### 5.2.9 Attachment 模块
负责：

- 附件元数据
- 访问控制
- 下载日志
- 预览状态
- 敏感等级

### 5.2.10 BusinessRelation 模块
负责：

- 联系人
- 邮件/线程/发件任务与业务对象关联
- 自动匹配与人工修正

### 5.2.11 Integration 模块
负责：

- 外部应用接入
- API 鉴权
- 幂等记录
- Webhook 订阅与投递
- 外部对象映射

### 5.2.12 Statistics 模块
负责：

- 日报聚合
- 用户/邮箱维度指标
- 协同效率指标

### 5.2.13 AuditSecurity 模块
负责：

- 敏感行为审计
- 安全事件记录
- 凭据操作审计
- 附件访问留痕

### 5.2.14 SystemSetting 模块
负责：

- 系统参数
- 特性开关
- 默认策略配置
- 健康检查配置

---

# 6. 核心领域模型设计

## 6.1 核心模型总览

```text
MailAccount
  └─ MailAccountFolderState

MailThread
  ├─ MailMessage
  │   ├─ MailMessageBody
  │   ├─ MailAddress
  │   └─ MailAttachmentRef
  ├─ ThreadAssignment
  ├─ InternalNote
  └─ ThreadActivity

MailSendTask
  ├─ SendRecipient
  └─ SendAttachmentRef

MailApproval
  └─ MailApprovalRecord

MailTemplate
  ├─ MailTemplateVersion
  └─ Signature

MailRule
  └─ RuleExecutionLog

MailContact
  └─ ContactEmail

BusinessRelation
IntegrationApp
WebhookSubscription
```

## 6.2 核心业务主线

邮件系统的核心主线可以抽象为：

```text
邮箱账号
→ 邮件接入
→ 线程归并
→ 协同处理
→ 审批控制
→ 外发或归档
→ 业务沉淀
→ 审计与统计
```

因此从领域视角看，核心处理对象不是单表，而是以下几个关键聚合：

- `MailThread`
- `MailMessage`
- `MailSendTask`
- `MailApproval`
- `MailAccount`

---

# 7. 聚合设计

## 7.1 聚合设计原则

本系统聚合设计遵循：

1. 一个聚合只维护一个核心业务一致性边界
2. 不把查询方便性误认为聚合边界
3. 高频写入对象优先独立聚合
4. 跨模块只传 ID 和事件，不共享内部状态修改权

## 7.2 聚合根清单

| 聚合根 | 模块 | 说明 |
|---|---|---|
| `MailAccount` | MailAccount | 邮箱账号聚合 |
| `MailThread` | MailThread | 线程聚合 |
| `MailMessage` | MailMessage | 单封邮件聚合 |
| `MailSendTask` | MailCompose | 发件任务聚合 |
| `ThreadAssignmentRecord` | Collaboration | 分派记录聚合根或独立实体根 |
| `MailApproval` | Approval | 审批聚合 |
| `MailTemplate` | Template | 模板聚合 |
| `MailRule` | RuleEngine | 规则聚合 |
| `MailContact` | BusinessRelation | 联系人聚合 |
| `BusinessRelationLink` | BusinessRelation | 通用业务关联聚合根 |
| `IntegrationApp` | Integration | 应用接入聚合 |
| `WebhookSubscription` | Integration | Webhook 订阅聚合 |

> 说明：`ThreadAssignmentRecord` 是否设计为聚合根，取决于实现方式。若责任流转主要从 `MailThread` 发起，也可将“当前负责人”作为 `MailThread` 的状态，而把历史记录作为协同模块中的独立实体根维护。

## 7.3 `MailThread` 聚合

### 7.3.1 聚合职责
负责维护：

- 线程主题与归属摘要
- 当前处理状态
- 当前负责人
- 最近活动时间
- 处理完成、归档、关闭等核心状态流转

### 7.3.2 聚合边界内规则
- 线程状态流转合法性
- 当前负责人变更规则
- 完成/归档/关闭前置条件
- 线程基础摘要维护

### 7.3.3 不放入聚合内部的对象
以下对象不建议强绑定进 `MailThread` 聚合内部，以避免超大聚合：

- 全量邮件正文
- 所有附件
- 审批单详情
- 统计数据
- Webhook 投递记录

## 7.4 `MailMessage` 聚合

### 7.4.1 聚合职责
负责维护：

- 单封邮件基础元数据
- 发件人/收件人地址
- 正文关联
- 附件关联
- 来源信息和 RFC 头信息摘要

### 7.4.2 适用场景
- 邮件入库
- 邮件详情查看
- 单封邮件上下文识别
- 收件同步去重

### 7.4.3 设计说明
`MailMessage` 与 `MailThread` 是强关联但独立聚合：

- `MailThread` 负责“会话”
- `MailMessage` 负责“单封消息”

## 7.5 `MailSendTask` 聚合

### 7.5.1 聚合职责
负责维护：

- 草稿内容
- 发件收件人
- 模板快照
- 发件状态
- 重试次数
- 是否需要审批

### 7.5.2 聚合内规则
- 草稿转待审批
- 待审批转待发送
- 发件中、成功、失败状态流转
- 重试次数上限控制
- 幂等键一致性检查

## 7.6 `MailApproval` 聚合

### 7.6.1 聚合职责
负责维护：

- 审批主状态
- 当前审批步骤
- 审批记录
- 审批通过/拒绝/撤回规则

### 7.6.2 聚合边界内规则
- 已通过审批不可再次审批
- 已拒绝审批不可直接转通过
- 已撤回审批不可继续流转
- 审批记录必须与主状态一致

## 7.7 `MailTemplate` 聚合

### 7.7.1 聚合职责
负责维护：

- 模板主信息
- 当前版本
- 版本历史
- 启停与发布状态

### 7.7.2 聚合内规则
- 版本号递增
- 已发布版本不可直接覆盖
- 当前启用版本唯一
- 变量结构需合法

## 7.8 `MailRule` 聚合

### 7.8.1 聚合职责
负责维护：

- 规则定义
- 优先级
- 条件与动作结构
- 生效时间
- 启停状态

### 7.8.2 聚合内规则
- 同一规则编码唯一
- 动作配置合法
- 时间区间合法
- 禁用规则不可执行

---

# 8. 实体与值对象设计

## 8.1 实体清单

### 8.1.1 MailAccount 相关
- `MailAccount`
- `MailAccountFolderState`

### 8.1.2 MailThread 相关
- `MailThread`
- `ThreadParticipantSummary`（可值对象化）
- `ThreadStatusSnapshot`（可值对象化）

### 8.1.3 MailMessage 相关
- `MailMessage`
- `MailMessageBody`
- `MailAddress`
- `MailAttachmentRef`

### 8.1.4 Collaboration 相关
- `ThreadAssignment`
- `InternalNote`
- `ThreadActivity`

### 8.1.5 Approval 相关
- `MailApproval`
- `MailApprovalRecord`

### 8.1.6 Template 相关
- `MailTemplate`
- `MailTemplateVersion`
- `Signature`

### 8.1.7 RuleEngine 相关
- `MailRule`
- `RuleExecutionLog`

### 8.1.8 BusinessRelation 相关
- `MailContact`
- `ContactEmail`
- `BusinessRelationLink`

### 8.1.9 Integration 相关
- `IntegrationApp`
- `WebhookSubscription`
- `WebhookDelivery`
- `IdempotencyRecord`

## 8.2 值对象设计建议

## 8.2.1 `EmailAddressValue`
建议作为值对象封装：

- 原始邮箱地址
- 标准化邮箱地址
- 显示名

优势：

- 统一邮箱地址合法性校验
- 统一标准化逻辑
- 避免多个模块重复实现

## 8.2.2 `MailSubjectValue`
封装：

- 原始主题
- 标准化主题

用于线程归并和搜索索引。

## 8.2.3 `MailBodySummaryValue`
封装正文摘要、纯文本摘要等。

## 8.2.4 `AttachmentDescriptor`
封装：

- 文件名
- 大小
- 类型
- 哈希
- 访问级别

## 8.2.5 `AssignmentTarget`
封装：

- 负责人类型
- 负责人 ID

用于统一表示“用户 / 角色 / 组织”三类分派目标。

## 8.2.6 `BusinessRef`
封装：

- 业务对象类型
- 业务对象 ID
- 编码
- 展示名

## 8.2.7 `ApprovalDecision`
封装：

- 审批动作
- 审批意见
- 审批时间
- 审批人

## 8.2.8 `TemplateVariableBag`
封装模板渲染变量集合，负责变量存在性检查与只读访问。

---

# 9. 领域服务设计

## 9.1 领域服务使用原则

只有在规则无法自然归属单一实体或聚合时，才设计领域服务。
避免把所有逻辑都写成“Service”。

## 9.2 建议领域服务清单

## 9.2.1 `MailThreadMergeService`
职责：

- 根据 `Message-Id` / `References` / `In-Reply-To` 归并线程
- 兜底按主题、参与人、时间窗口归并
- 输出归并结果和归并原因

## 9.2.2 `MailApprovalPolicyService`
职责：

- 判断发件是否需要审批
- 根据邮箱、模板、附件、收件域名等输出审批策略

## 9.2.3 `MailRuleMatchService`
职责：

- 对邮件上下文执行规则匹配
- 返回命中的规则集合
- 控制优先级和 stop-on-match 行为

## 9.2.4 `BusinessRelationMatchService`
职责：

- 根据邮箱地址、订单号、合同号、工单号等自动匹配业务对象
- 返回候选关联集

## 9.2.5 `MailTemplateRenderService`
职责：

- 渲染模板主题和正文
- 校验变量完整性
- 组合签名

## 9.2.6 `MailPermissionDomainService`
职责：

- 对领域级操作进行权限语义判断
- 如“此用户能否认领此线程”“能否审批该发件任务”

> 注意：页面可见性仍主要由应用层和授权系统控制，领域服务只负责操作语义约束。

## 9.2.7 `MailSendPolicyService`
职责：

- 判断发件是否允许立即发送
- 校验重试上限、状态合法性、邮箱可用性

---

# 10. 领域事件设计

## 10.1 事件设计目标

领域事件用于表达“业务上已经发生的重要事实”，用于：

- 模块解耦
- 异步处理
- 审计与统计
- 集成回调

## 10.2 关键领域事件清单

| 事件 | 触发模块 | 说明 |
|---|---|---|
| `MailReceivedEvent` | MailMessage | 新邮件成功入库 |
| `MailThreadCreatedEvent` | MailThread | 创建新线程 |
| `MailThreadMergedEvent` | MailThread | 邮件归并入线程 |
| `MailThreadAssignedEvent` | Collaboration | 线程被分派/认领 |
| `MailInternalNoteAddedEvent` | Collaboration | 新增内部备注 |
| `MailSendTaskSubmittedEvent` | MailCompose | 提交发件任务 |
| `MailApprovalCreatedEvent` | Approval | 创建审批单 |
| `MailApprovalApprovedEvent` | Approval | 审批通过 |
| `MailApprovalRejectedEvent` | Approval | 审批拒绝 |
| `MailSendTaskStartedEvent` | MailCompose | 后台开始发件 |
| `MailSentEvent` | MailCompose | 邮件发送成功 |
| `MailSendFailedEvent` | MailCompose | 邮件发送失败 |
| `MailBusinessRelationCreatedEvent` | BusinessRelation | 建立业务关联 |
| `AttachmentDownloadedEvent` | Attachment | 附件下载 |
| `WebhookDeliveryFailedEvent` | Integration | Webhook 多次失败 |

## 10.3 事件使用规范

- 事件名称使用过去式，表示“已经发生”
- 事件中传递必要字段，不传整个实体
- 事件负载只包含订阅方需要的关键信息
- 事件处理器必须幂等

## 10.4 典型事件链示例

### 收件事件链
```text
MailReceivedEvent
→ RuleEngine 执行规则
→ BusinessRelation 自动关联
→ Statistics 更新统计
→ AuditSecurity 记录审计
→ Integration 推送 Webhook
```

### 审批通过事件链
```text
MailApprovalApprovedEvent
→ MailCompose 将发件任务转为待发送
→ Statistics 更新审批统计
→ Integration 推送审批结果
→ AuditSecurity 记录审计
```

---

# 11. 仓储接口设计

## 11.1 仓储设计原则

- 仓储面向聚合根
- 不暴露过细碎的 SQL 语义
- 高复杂查询可拆到 Query Service
- 仓储只负责持久化抽象，不承载业务编排

## 11.2 核心仓储接口建议

## 11.2.1 `IMailAccountRepository`
支持：

- 按邮箱地址查询
- 查询启用同步的账号
- 查询启用发件的账号
- 查询账号授权摘要

## 11.2.2 `IMailThreadRepository`
支持：

- 获取线程
- 按负责人查询线程
- 按状态查询线程
- 按主题/时间窗口查候选线程
- 保存线程状态变更

## 11.2.3 `IMailMessageRepository`
支持：

- 按账号+UID 查询
- 按 InternetMessageId 查询
- 按线程查询邮件列表
- 保存新邮件入库

## 11.2.4 `IMailSendTaskRepository`
支持：

- 查询待发送任务
- 查询失败待重试任务
- 按幂等键查询
- 按审批状态查询待处理任务

## 11.2.5 `IMailApprovalRepository`
支持：

- 获取审批单
- 查询待审批列表
- 按业务对象查询审批单
- 保存审批决策

## 11.2.6 `IMailTemplateRepository`
支持：

- 按编码查询模板
- 查询启用模板
- 保存版本切换

## 11.2.7 `IMailRuleRepository`
支持：

- 查询指定范围内启用规则
- 按优先级获取规则集
- 保存规则状态变更

## 11.2.8 `IBusinessRelationRepository`
支持：

- 查询某线程/邮件/发件任务的业务关联
- 查询某业务对象关联的线程/邮件
- 保存关联修正结果

## 11.2.9 `IAttachmentRepository`
支持：

- 查询邮件附件
- 查询附件访问级别
- 保存下载计数与日志

## 11.3 查询服务建议

以下场景建议使用 Query Service，而不是仓储：

- 工作台汇总查询
- 收件箱复杂筛选
- 全局高级搜索
- 统计报表
- 协同时间线拼装
- 邮件详情页聚合展示

---

# 12. 应用层用例设计

## 12.1 应用服务划分原则

应用服务按“业务用例”组织，而不是按“实体 CRUD”组织。

## 12.2 建议应用服务清单

## 12.2.1 `MailInboxAppService`
负责：

- 收件箱列表
- 邮件详情读取
- 已读/未读
- 星标/取消星标
- 标签操作

## 12.2.2 `MailThreadAppService`
负责：

- 线程详情
- 线程状态变更
- 归档
- 关闭
- 恢复

## 12.2.3 `MailComposeAppService`
负责：

- 创建草稿
- 保存草稿
- 选择模板
- 提交审批
- 立即发送
- 定时发送
- 重试失败发送

## 12.2.4 `CollaborationAppService`
负责：

- 认领线程
- 分派线程
- 转派线程
- 退回共享池
- 新增内部备注
- 查询协同日志

## 12.2.5 `ApprovalAppService`
负责：

- 我的待审批
- 审批通过
- 审批拒绝
- 撤回审批
- 查看审批历史

## 12.2.6 `MailAccountAppService`
负责：

- 邮箱管理
- 测试连接
- 启停同步
- 授权配置
- 手动触发同步

## 12.2.7 `TemplateAppService`
负责：

- 模板管理
- 模板发布
- 模板预览
- 签名管理

## 12.2.8 `RuleAppService`
负责：

- 规则增删改查
- 启停规则
- 规则测试
- 执行日志查看

## 12.2.9 `BusinessRelationAppService`
负责：

- 联系人管理
- 业务关联查询
- 人工关联修正
- 外部业务对象补写映射

## 12.2.10 `AttachmentAppService`
负责：

- 附件列表
- 预览
- 下载
- 访问日志查看

## 12.2.11 `IntegrationAppService`
负责：

- 应用接入管理
- Webhook 订阅
- 幂等记录查询
- 集成日志查看

## 12.2.12 `StatisticsAppService`
负责：

- 邮件收发统计
- 用户处理统计
- 处理效率统计
- 规则命中统计

---

# 13. 模块协作关系设计

## 13.1 依赖方向原则

模块间依赖建议如下：

- 核心模块尽量少依赖外围模块
- 外围模块依赖核心模块事件
- 查询型依赖弱于写入型依赖
- 能通过事件完成的，不优先同步强耦合调用

## 13.2 典型协作关系

## 13.2.1 收件同步流程
```text
MailAccount
→ MailMessage
→ MailThread
→ RuleEngine
→ BusinessRelation
→ Statistics
→ AuditSecurity
→ Integration
```

## 13.2.2 发件流程
```text
MailCompose
→ Template
→ Approval
→ MailAccount
→ MailMessage
→ MailThread
→ Statistics
→ AuditSecurity
→ Integration
```

## 13.2.3 协同流程
```text
MailThread
→ Collaboration
→ Approval（按需要）
→ AuditSecurity
→ Statistics
```

## 13.3 模块交互方式建议

| 场景 | 建议方式 |
|---|---|
| 同步操作、强事务依赖 | 应用层直接调用接口 |
| 异步补充处理 | 领域事件 / 本地事件 |
| 对外回调 | 集成事件 + Webhook |
| 报表查询 | Query Service / Read Model |
| 权限判断 | 授权服务 + 领域校验 |

## 13.4 避免的反模式

- 模块 A 直接操作模块 B 的 EF 实体
- 模块 A 直接依赖模块 B 的 DbContext
- 应用服务直接写过多跨模块 SQL
- 用 DTO 代替领域对象承载业务规则
- 把所有逻辑堆在一个 `MailService`

---

# 14. 状态流转领域约束

## 14.1 线程状态流转约束

### 允许流转
- 待分派 → 待处理 / 处理中 / 已关闭
- 待处理 → 处理中 / 待审批 / 已完成
- 处理中 → 待审批 / 已完成 / 已关闭
- 待审批 → 处理中 / 已完成 / 已关闭
- 已完成 → 已归档
- 已归档 → 恢复处理中（高权限场景）
- 已关闭 → 一般不再流转，特殊场景可恢复

### 领域约束
- 已归档线程不允许继续新增外部处理动作，除非先恢复
- 已关闭线程不可直接发起新审批
- 无负责人线程不可直接标记为已完成

## 14.2 发件任务状态流转约束

### 允许流转
- 草稿 → 待审批 / 待发送 / 已取消
- 待审批 → 待发送 / 已取消
- 待发送 → 发送中 / 已取消
- 发送中 → 发送成功 / 发送失败
- 发送失败 → 待发送 / 已取消

### 领域约束
- 已发送成功任务不可再编辑
- 已取消任务不可直接恢复为发送中
- 发送失败重试前必须检查重试上限

## 14.3 审批状态流转约束

### 允许流转
- 待审批 → 已通过 / 已拒绝 / 已撤回

### 领域约束
- 已通过不能再拒绝
- 已拒绝不能直接转通过，需重新发起审批
- 已撤回审批单不能继续审批

## 14.4 分派动作约束

- 共享池线程才允许认领
- 非当前负责人不能随意转派，除非具备管理权限
- 已归档线程不允许分派
- 自动分派产生的记录也必须可审计

---

# 15. 权限与数据可见性落点

## 15.1 权限不应全部下沉到领域层

权限设计应分三层：

### 15.1.1 功能权限
由 ABP 权限系统控制菜单、按钮、接口访问。

### 15.1.2 数据权限
由查询服务、授权服务、仓储过滤统一控制用户可见数据范围。

### 15.1.3 领域操作权限
由领域服务或应用服务控制：
- 能否认领
- 能否审批
- 能否归档
- 能否下载敏感附件

## 15.2 统一授权服务建议

建议设计：

- `IMailAuthorizationService`
- `IMailDataPermissionService`

负责判断：

- 用户是否可访问某邮箱
- 用户是否可查看某线程
- 用户是否可操作某发件任务
- 用户是否可下载某附件

## 15.3 数据可见性来源

数据可见范围可由以下因素组合而成：

- 邮箱授权
- 组织归属
- 当前负责人
- 历史参与关系
- 审批关系
- 管理员全局权限
- 业务对象映射权限

---

# 16. 代码组织设计建议

## 16.1 代码目录建议

```text
src/
├─ MailManagement.Domain.Shared
│  ├─ Localization
│  ├─ Permissions
│  ├─ Settings
│  ├─ Enums
│  ├─ Constants
│  └─ Events
│
├─ MailManagement.Domain
│  ├─ MailAccount
│  │  ├─ AggregateRoots
│  │  ├─ Entities
│  │  ├─ ValueObjects
│  │  ├─ DomainServices
│  │  └─ Repositories
│  ├─ MailMessage
│  ├─ MailThread
│  ├─ MailCompose
│  ├─ Collaboration
│  ├─ Approval
│  ├─ Template
│  ├─ RuleEngine
│  ├─ Attachment
│  ├─ BusinessRelation
│  ├─ Integration
│  ├─ Statistics
│  ├─ AuditSecurity
│  └─ SystemSetting
│
├─ MailManagement.Application.Contracts
│  ├─ Dtos
│  ├─ Permissions
│  ├─ Services
│  └─ InputsOutputs
│
├─ MailManagement.Application
│  ├─ MailAccount
│  ├─ MailMessage
│  ├─ MailThread
│  ├─ MailCompose
│  ├─ Collaboration
│  ├─ Approval
│  ├─ Template
│  ├─ RuleEngine
│  ├─ Attachment
│  ├─ BusinessRelation
│  ├─ Integration
│  ├─ Statistics
│  └─ Common
│
├─ MailManagement.EntityFrameworkCore
│  ├─ EntityFrameworkCore
│  ├─ Configurations
│  ├─ Repositories
│  ├─ Migrations
│  └─ Querying
│
├─ MailManagement.HttpApi
├─ MailManagement.Web
└─ MailManagement.DbMigrator
```

## 16.2 模块内推荐结构

以 `MailThread` 为例：

```text
MailThread/
├─ AggregateRoots
│  └─ MailThread.cs
├─ Entities
│  └─ ThreadParticipant.cs
├─ ValueObjects
│  ├─ ThreadSubject.cs
│  └─ AssignmentTarget.cs
├─ DomainServices
│  └─ MailThreadMergeService.cs
├─ Events
│  ├─ MailThreadCreatedEvent.cs
│  └─ MailThreadAssignedEvent.cs
├─ Repositories
│  └─ IMailThreadRepository.cs
└─ Specifications
   └─ ThreadCanArchiveSpecification.cs
```

## 16.3 代码命名建议

- 聚合根：名词，如 `MailThread`
- 领域服务：业务语义 + `Service`
- 仓储接口：`I{Aggregate}Repository`
- 领域事件：过去式 + `Event`
- 应用服务：用例语义 + `AppService`

---

# 17. 开发实施建议

## 17.1 实施顺序建议

建议按以下顺序推进：

### 第一步：打底模块
- Identity / Organization（复用 ABP）
- SystemSetting
- AuditSecurity
- MailAccount

### 第二步：核心邮件主线
- MailMessage
- MailThread
- Attachment
- MailCompose

### 第三步：业务闭环能力
- Collaboration
- Approval
- Template
- RuleEngine
- BusinessRelation

### 第四步：平台增强能力
- Integration
- Statistics

## 17.2 单元测试优先对象

以下领域对象建议优先写单元测试：

- `MailThread` 状态流转
- `MailSendTask` 状态流转
- `MailApproval` 决策流转
- `MailThreadMergeService` 归并规则
- `MailApprovalPolicyService` 审批命中规则
- `MailRuleMatchService` 命中逻辑
- `MailTemplateRenderService` 渲染逻辑

## 17.3 先做对，再做快

在邮件系统这类强协同、强审计场景中，最危险的问题不是“慢一点”，而是：

- 线程归并错误
- 权限越权
- 审批绕过
- 附件误下载
- 外发重复发送
- 共享池责任混乱

因此领域模型设计必须优先保证语义正确和规则稳定。

---

# 18. 演进规划

## 18.1 一期领域模型重点

一期建议重点落地以下稳定领域模型：

- `MailAccount`
- `MailMessage`
- `MailThread`
- `MailSendTask`
- `MailApproval`
- `ThreadAssignment`
- `InternalNote`
- `MailTemplate`
- `MailRule`
- `BusinessRelationLink`

## 18.2 二期增强方向

### 18.2.1 SLA 领域增强
新增：
- `SlaPolicy`
- `SlaTimer`
- `TimeoutEvent`

### 18.2.2 智能化增强
新增：
- `MailSummary`
- `IntentRecognitionResult`
- `SuggestedAssignment`
- `SuggestedBusinessRelation`

### 18.2.3 搜索增强
新增：
- 独立搜索索引模型
- 附件全文模型
- OCR 结果模型

### 18.2.4 多租户增强
为现有聚合统一强化：
- `tenant_id`
- 租户级配置
- 租户级策略作用域

## 18.3 服务化拆分优先级

若后续拆服务，建议优先级如下：

1. `Integration`
2. `Statistics`
3. `Attachment`
4. `RuleEngine`
5. `MailAccount` 的协议适配部分
6. `Search`（新增独立上下文）

---

# 19. 结论

邮件管理系统的领域模型设计，应围绕以下核心理念建立：

1. **以线程而不是单封邮件作为协同核心**
2. **以发件任务和审批作为主动发信闭环核心**
3. **以业务关联和规则自动化作为企业化能力增强核心**
4. **以模块边界清晰和事件驱动解耦作为系统可演进核心**

一期推荐采用如下稳定结构：

- `MailAccount` 负责连接与接入
- `MailMessage` 负责单封邮件事实
- `MailThread` 负责会话与状态
- `Collaboration` 负责责任流转
- `Approval` 负责合规控制
- `MailCompose` 负责主动发件
- `BusinessRelation` 负责业务上下文沉淀
- `RuleEngine`、`Template`、`Attachment`、`Integration`、`Statistics` 提供支撑能力

该设计既能满足当前阶段快速交付，也能为后续搜索增强、AI 能力、SLA、多租户和服务化拆分奠定稳定基础。

