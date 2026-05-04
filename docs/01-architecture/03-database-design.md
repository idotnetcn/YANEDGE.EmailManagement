
# 邮件管理系统数据库设计文档

> 项目名称：邮件管理系统
> 技术路线：.NET 10.x / ABP 10.x / PostgreSQL
> 文档版本：V1.0
> 文档属性：数据库设计文档
> 适用阶段：架构设计 / 数据建模 / 开发实施 / SQL 审核 / 测试与运维

---

## 修订记录

| 版本 | 日期 | 作者 | 说明 |
|---|---|---|---|
| V1.0 | 2026-04-25 | 项目组 | 初始版本 |

---

## 目录

- [1. 文档目标](#1-文档目标)
- [2. 设计依据与范围](#2-设计依据与范围)
- [3. 数据库总体设计](#3-数据库总体设计)
- [4. 命名规范与字段约定](#4-命名规范与字段约定)
- [5. ABP 标准表复用说明](#5-abp-标准表复用说明)
- [6. 逻辑数据模型总览](#6-逻辑数据模型总览)
- [7. 详细表设计](#7-详细表设计)
  - [7.1 公共字段约定](#71-公共字段约定)
  - [7.2 邮箱账号与同步域](#72-邮箱账号与同步域)
  - [7.3 邮件与线程域](#73-邮件与线程域)
  - [7.4 协同与审批域](#74-协同与审批域)
  - [7.5 模板与规则域](#75-模板与规则域)
  - [7.6 联系人与业务关联域](#76-联系人与业务关联域)
  - [7.7 集成开放平台域](#77-集成开放平台域)
  - [7.8 统计分析域](#78-统计分析域)
- [8. 关键索引设计](#8-关键索引设计)
- [9. 约束与一致性设计](#9-约束与一致性设计)
- [10. 分区、归档与生命周期设计](#10-分区归档与生命周期设计)
- [11. 安全与审计设计](#11-安全与审计设计)
- [12. EF Core 与迁移实施建议](#12-ef-core-与迁移实施建议)
- [13. 结论](#13-结论)

---

# 1. 文档目标

本文档用于明确邮件管理系统在 PostgreSQL 中的数据库设计方案，重点说明：

1. 数据库设计原则与命名规范
2. 核心业务对象的表结构设计
3. 主要实体关系、约束与索引策略
4. 大文本、附件、日志、统计数据的存储方式
5. 与 ABP 标准表的衔接方式
6. 后续扩展、分区、归档与安全控制建议

本文档是以下工作的直接依据：

- EF Core 实体与映射实现
- 数据库 Migration 设计
- SQL Review
- 性能优化
- 接口字段与查询模型设计
- 测试数据准备与环境部署

---

# 2. 设计依据与范围

## 2.1 设计依据

本数据库设计基于以下文档和前置约束：

- 00《邮件管理系统需求分析报告》
- 01《邮件管理系统功能设计与业务流程说明》
- 02《邮件管理系统技术架构设计文档》

## 2.2 数据库选型

本系统关系型数据库采用：

- **PostgreSQL 16+**（建议版本）
- ORM：**EF Core**
- 框架：**ABP 10.x**

选择 PostgreSQL 的原因：

- 支持 `jsonb`
- 支持 GIN / BTREE / Full Text Search
- 支持良好的事务一致性
- 支持分区表
- 适合结构化 + 半结构化混合数据存储场景
- 适合中大型企业业务系统长期演进

## 2.3 本文档范围

本文档覆盖：

- 自定义业务表设计
- 与 ABP 标准表的引用关系
- 关键索引、唯一约束、外键设计
- 日志、统计、集成类表设计
- 数据安全与存储策略

本文档不覆盖：

- PostgreSQL 集群部署脚本
- 具体 SQL Migration 文件
- 存储过程 / 函数详细实现
- 物理机或云数据库参数调优清单

---

# 3. 数据库总体设计

## 3.1 设计原则

数据库设计遵循以下原则：

### 3.1.1 事务数据优先结构化
核心业务数据尽量结构化存储，避免过度依赖 `jsonb`，保证可检索性、约束性和可维护性。

### 3.1.2 元数据与大内容分离
将邮件元数据、正文、附件二进制内容拆分：

- 邮件元数据存 PostgreSQL
- 邮件正文存 PostgreSQL 独立表
- 附件元数据存 PostgreSQL
- 附件实际文件存对象存储

### 3.1.3 审计与业务分离
业务表负责主流程数据，审计日志、访问日志、集成日志、任务日志独立设计，避免主业务表过度膨胀。

### 3.1.4 面向扩展设计
一期满足基础业务需要，同时为以下能力预留扩展空间：

- 多组织 / 多租户
- 搜索增强
- SLA
- AI 能力
- 第三方 Provider 接入
- 统计仓库和 BI

### 3.1.5 避免过度范式化
邮件系统天然带有复杂半结构化信息，不追求极端三范式。对于规则、模板变量、外部映射上下文等，允许合理使用 `jsonb`。

## 3.2 数据分层

数据库中的数据大致分为以下层次：

| 数据层 | 内容 | 存储方式 |
|---|---|---|
| 主业务层 | 邮箱、邮件、线程、发件、协同、审批 | PostgreSQL |
| 扩展配置层 | 模板、签名、规则、集成配置、参数 | PostgreSQL |
| 日志与审计层 | 同步日志、发件日志、Webhook 日志、附件访问日志 | PostgreSQL |
| 检索增强层 | `tsvector`、搜索摘要、归并辅助字段 | PostgreSQL |
| 文件对象层 | 附件文件、预览文件、原始 MIME（可选） | 对象存储 |

## 3.3 逻辑分域

数据库按领域划分为以下子域：

1. 邮箱账号与同步域
2. 邮件与线程域
3. 协同与审批域
4. 模板与规则域
5. 联系人与业务关联域
6. 集成开放平台域
7. 统计分析域

---

# 4. 命名规范与字段约定

## 4.1 表命名规范

自定义业务表统一采用：

- 小写
- 下划线分隔
- 业务前缀 `mail_` / `integration_`

例如：

- `mail_account`
- `mail_message`
- `mail_thread`
- `mail_send_task`
- `integration_app`

## 4.2 字段命名规范

统一采用：

- 小写
- 下划线分隔
- 主键统一为 `id`
- 外键统一为 `{table_singular}_id` 或 `{domain}_id`

例如：

- `mail_account_id`
- `mail_thread_id`
- `creator_id`
- `tenant_id`

## 4.3 主键策略

所有核心业务表主键统一使用：

- `uuid`

理由：

- 便于分布式和离线生成
- 避免暴露连续自增信息
- 与 ABP / EF Core 生态一致

## 4.4 时间字段规范

所有时间字段统一使用：

- `timestamptz`

原则：

- 存储 UTC 时间
- 展示时由应用层按用户时区转换

## 4.5 布尔、枚举与状态字段规范

- 布尔字段：`boolean`
- 状态字段：`smallint`
- 类型字段：`smallint`

说明：

- 状态枚举由应用层统一管理
- 数据库中保留 `smallint`，避免字符串存储开销与不一致风险

## 4.6 文本字段规范

| 类型 | 使用场景 |
|---|---|
| `varchar(n)` | 短文本、编码、名称、邮箱地址、状态码 |
| `text` | 正文、错误信息、备注、大文本内容 |
| `jsonb` | 扩展属性、规则 JSON、上下文快照、外部载荷 |

## 4.7 标准邮件字段规范

涉及邮箱地址的字段统一建议保留两类：

- 原始展示值，如 `email_address`
- 标准化值，如 `normalized_email_address`

标准化规则建议：

- 去前后空格
- 转小写
- 域名统一小写

---

# 5. ABP 标准表复用说明

本系统不重复设计 ABP 已提供的标准基础表，直接复用以下模块数据：

## 5.1 身份与组织相关

- 用户表
- 角色表
- 用户角色关联表
- 组织机构表
- 组织成员表
- 权限授权表

这些表由 ABP Identity / Permission Management / Organization Units 提供。

## 5.2 审计与日志相关

如启用 ABP 审计模块，可复用其标准审计表存储：

- 登录日志
- 接口调用日志
- 实体变更日志
- 权限变更相关日志

## 5.3 数据关联方式

本系统业务表中涉及用户、角色、组织时，统一通过 `uuid` 关联，不在业务表中冗余存储完整用户信息。

例如：

- `creator_id` → 用户表
- `approver_id` → 用户表
- `owner_org_id` → 组织表
- `current_assignee_id` → 用户 / 角色 / 组织，需配合 `current_assignee_type`

---

# 6. 逻辑数据模型总览

## 6.1 核心关系概览

```text
mail_account
 ├─ mail_account_permission
 ├─ mail_account_folder_state
 ├─ mail_sync_job_log
 ├─ mail_message
 │   ├─ mail_message_body
 │   ├─ mail_message_address
 │   ├─ mail_attachment
 │   └─ mail_message_user_state
 └─ mail_send_task
     └─ mail_send_task_recipient

mail_thread
 ├─ mail_message
 ├─ mail_thread_tag_rel
 ├─ mail_thread_assignment
 ├─ mail_internal_note
 ├─ mail_thread_activity
 ├─ mail_approval
 └─ mail_business_relation

mail_template
 ├─ mail_template_version
 └─ mail_signature

mail_rule
 └─ mail_rule_execution_log

mail_contact
 └─ mail_contact_email

integration_app
 ├─ integration_idempotency_record
 ├─ integration_api_call_log
 ├─ integration_webhook_subscription
 └─ integration_webhook_delivery
```

## 6.2 主实体说明

| 实体 | 说明 |
|---|---|
| `mail_account` | 纳管的邮箱账号 |
| `mail_thread` | 线程 / 会话主实体 |
| `mail_message` | 单封邮件主实体 |
| `mail_message_body` | 邮件正文与搜索内容 |
| `mail_attachment` | 邮件附件元数据 |
| `mail_send_task` | 发件任务 / 草稿 / 定时发送实体 |
| `mail_thread_assignment` | 分派、认领、转派记录 |
| `mail_approval` | 发件审批主表 |
| `mail_template` | 模板主表 |
| `mail_rule` | 规则主表 |
| `mail_business_relation` | 邮件 / 线程与业务对象的关联 |
| `integration_app` | 外部系统应用定义 |

---

# 7. 详细表设计

# 7.1 公共字段约定

## 7.1.1 审计型业务表公共字段

对于大多数核心业务表，统一包含以下公共字段：

| 字段 | 类型 | 非空 | 默认值 | 说明 |
|---|---|---|---|---|
| `id` | uuid | 是 | - | 主键 |
| `tenant_id` | uuid | 否 | null | 预留多租户 |
| `extra_properties` | jsonb | 是 | `'{}'` | ABP 扩展字段 |
| `concurrency_stamp` | varchar(40) | 否 | null | 并发控制 |
| `creation_time` | timestamptz | 是 | now() | 创建时间 |
| `creator_id` | uuid | 否 | null | 创建人 |
| `last_modification_time` | timestamptz | 否 | null | 最后修改时间 |
| `last_modifier_id` | uuid | 否 | null | 最后修改人 |
| `is_deleted` | boolean | 是 | false | 软删除标记 |
| `deleter_id` | uuid | 否 | null | 删除人 |
| `deletion_time` | timestamptz | 否 | null | 删除时间 |

> 说明：日志类表和统计类表可不包含完整软删除字段，按实际需要裁剪。

## 7.1.2 状态枚举统一约定

数据库中所有状态字段统一采用 `smallint`。
具体枚举建议如下：

### 邮箱类型 `account_type`
- 1：个人邮箱
- 2：共享邮箱
- 3：部门邮箱
- 4：系统邮箱

### 线程状态 `thread_status`
- 1：待分派
- 2：待处理
- 3：处理中
- 4：待审批
- 5：已完成
- 6：已归档
- 7：已关闭

### 邮件方向 `message_direction`
- 1：收件
- 2：发件

### 发件任务状态 `send_task_status`
- 1：草稿
- 2：待审批
- 3：待发送
- 4：发送中
- 5：发送成功
- 6：发送失败
- 7：已取消

### 审批状态 `approval_status`
- 1：待审批
- 2：已通过
- 3：已拒绝
- 4：已撤回

### 分派动作 `assignment_action_type`
- 1：自动分派
- 2：人工分派
- 3：认领
- 4：转派
- 5：退回共享池
- 6：关闭处理

---

# 7.2 邮箱账号与同步域

## 7.2.1 `mail_account`

### 表用途
存储系统纳管的邮箱账号信息、协议连接信息、授权与运行状态。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `account_name` | varchar(128) | 是 | 账号名称 |
| `email_address` | varchar(256) | 是 | 邮箱地址 |
| `normalized_email_address` | varchar(256) | 是 | 标准化邮箱地址 |
| `display_name` | varchar(128) | 否 | 发件显示名 |
| `account_type` | smallint | 是 | 邮箱类型 |
| `provider_name` | varchar(64) | 否 | 服务商标识 |
| `incoming_protocol` | smallint | 是 | 收件协议，1=IMAP |
| `outgoing_protocol` | smallint | 是 | 发件协议，1=SMTP |
| `incoming_host` | varchar(256) | 否 | 收件服务器地址 |
| `incoming_port` | int | 否 | 收件端口 |
| `incoming_ssl_enabled` | boolean | 是 | 收件 SSL 开关 |
| `outgoing_host` | varchar(256) | 否 | 发件服务器地址 |
| `outgoing_port` | int | 否 | 发件端口 |
| `outgoing_ssl_enabled` | boolean | 是 | 发件 SSL 开关 |
| `auth_type` | smallint | 是 | 认证类型 |
| `encrypted_password` | text | 否 | 加密后的密码 |
| `encrypted_access_token` | text | 否 | 加密后的访问令牌 |
| `encrypted_refresh_token` | text | 否 | 加密后的刷新令牌 |
| `token_expire_at` | timestamptz | 否 | 令牌过期时间 |
| `sync_enabled` | boolean | 是 | 是否启用同步 |
| `send_enabled` | boolean | 是 | 是否允许发件 |
| `health_status` | smallint | 是 | 健康状态 |
| `last_sync_at` | timestamptz | 否 | 最后同步时间 |
| `last_send_at` | timestamptz | 否 | 最后发件时间 |
| `owner_user_id` | uuid | 否 | 所属用户 |
| `owner_org_id` | uuid | 否 | 所属组织 |
| `description` | varchar(512) | 否 | 备注说明 |

### 索引与约束

- 主键：`pk_mail_account(id)`
- 唯一索引：`uk_mail_account_tenant_normalized_email(tenant_id, normalized_email_address)`
- 普通索引：
  - `idx_mail_account_owner_user_id`
  - `idx_mail_account_owner_org_id`
  - `idx_mail_account_sync_enabled`
  - `idx_mail_account_send_enabled`
  - `idx_mail_account_health_status`

### 设计说明

1. 邮箱凭据只存加密结果，不存明文
2. `normalized_email_address` 用于去重和快速匹配
3. `owner_user_id` 与 `owner_org_id` 支持个人邮箱、部门邮箱与共享邮箱场景

---

## 7.2.2 `mail_account_permission`

### 表用途
定义邮箱账号的数据授权范围，用于控制谁可查看、发件、下载附件、审批等。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_account_id` | uuid | 是 | 邮箱账号 ID |
| `subject_type` | smallint | 是 | 授权主体类型：1用户 2角色 3组织 |
| `subject_id` | uuid | 是 | 主体 ID |
| `can_view` | boolean | 是 | 是否可查看邮件 |
| `can_send` | boolean | 是 | 是否可发件 |
| `can_manage` | boolean | 是 | 是否可管理邮箱配置 |
| `can_download_attachment` | boolean | 是 | 是否可下载附件 |
| `can_approve` | boolean | 是 | 是否可审批 |
| `can_view_audit` | boolean | 是 | 是否可查看相关审计 |
| `effective_from` | timestamptz | 否 | 生效开始时间 |
| `effective_to` | timestamptz | 否 | 生效结束时间 |
| `remark` | varchar(256) | 否 | 授权备注 |

### 索引与约束

- 唯一索引：`uk_mail_account_permission(mail_account_id, subject_type, subject_id)`
- 普通索引：
  - `idx_mail_account_permission_subject(subject_type, subject_id)`
  - `idx_mail_account_permission_account(mail_account_id)`

---

## 7.2.3 `mail_account_folder_state`

### 表用途
记录邮箱下每个同步文件夹的增量同步状态。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_account_id` | uuid | 是 | 邮箱账号 ID |
| `folder_code` | varchar(64) | 是 | 系统文件夹编码，如 inbox/sent |
| `provider_folder_id` | varchar(256) | 否 | 服务端文件夹唯一标识 |
| `provider_folder_name` | varchar(256) | 否 | 服务端文件夹名称 |
| `last_uid` | bigint | 否 | 最近处理 UID |
| `last_success_uid` | bigint | 否 | 最近成功同步 UID |
| `last_sync_at` | timestamptz | 否 | 最近同步时间 |
| `sync_status` | smallint | 是 | 同步状态 |
| `last_error_message` | text | 否 | 最近错误信息 |
| `metadata_json` | jsonb | 是 | 额外状态 |

### 索引与约束

- 唯一索引：`uk_mail_account_folder_state(mail_account_id, folder_code)`
- 普通索引：
  - `idx_mail_account_folder_state_sync_status`
  - `idx_mail_account_folder_state_last_sync_at`

---

## 7.2.4 `mail_sync_job_log`

### 表用途
记录每次同步任务执行情况，用于监控、排障、补偿。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_account_id` | uuid | 是 | 邮箱账号 ID |
| `folder_code` | varchar(64) | 否 | 文件夹编码 |
| `trigger_source` | smallint | 是 | 触发来源：定时/手动/补偿 |
| `job_status` | smallint | 是 | 执行状态 |
| `started_at` | timestamptz | 是 | 开始时间 |
| `finished_at` | timestamptz | 否 | 结束时间 |
| `fetched_count` | int | 是 | 拉取总数 |
| `new_count` | int | 是 | 新增邮件数 |
| `updated_count` | int | 是 | 更新邮件数 |
| `skipped_count` | int | 是 | 跳过数量 |
| `failed_count` | int | 是 | 失败数量 |
| `lock_key` | varchar(128) | 否 | 分布式锁键 |
| `error_message` | text | 否 | 错误信息 |
| `context_json` | jsonb | 是 | 上下文快照 |

### 索引与约束

- 索引：
  - `idx_mail_sync_job_log_account_started(mail_account_id, started_at desc)`
  - `idx_mail_sync_job_log_status(job_status)`
  - `idx_mail_sync_job_log_started_at(started_at desc)`

---

# 7.3 邮件与线程域

## 7.3.1 `mail_thread`

### 表用途
线程主表，用于聚合同一会话中的多封邮件，并承载协同状态。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `subject` | varchar(512) | 是 | 线程主题 |
| `normalized_subject` | varchar(512) | 是 | 标准化主题 |
| `primary_mail_account_id` | uuid | 否 | 主要归属邮箱 |
| `first_message_time` | timestamptz | 否 | 首封邮件时间 |
| `latest_message_time` | timestamptz | 否 | 最后活动时间 |
| `latest_message_id` | uuid | 否 | 最近邮件 ID |
| `message_count` | int | 是 | 线程内邮件数 |
| `has_attachment` | boolean | 是 | 是否包含附件 |
| `thread_status` | smallint | 是 | 线程状态 |
| `priority` | smallint | 是 | 优先级 |
| `current_assignee_type` | smallint | 否 | 当前负责人类型：用户/角色/组织 |
| `current_assignee_id` | uuid | 否 | 当前负责人 ID |
| `last_replied_at` | timestamptz | 否 | 最后回复时间 |
| `due_time` | timestamptz | 否 | 预期处理截止时间 |
| `completed_time` | timestamptz | 否 | 完成时间 |
| `archived_time` | timestamptz | 否 | 归档时间 |
| `closed_time` | timestamptz | 否 | 关闭时间 |
| `business_summary_json` | jsonb | 是 | 业务对象摘要快照 |

### 索引与约束

- 索引：
  - `idx_mail_thread_status_latest(thread_status, latest_message_time desc)`
  - `idx_mail_thread_assignee(current_assignee_type, current_assignee_id)`
  - `idx_mail_thread_primary_account(primary_mail_account_id)`
  - `idx_mail_thread_due_time(due_time)`
  - `idx_mail_thread_normalized_subject(normalized_subject)`

### 设计说明

1. 线程是协同处理的核心主体
2. 当前负责人直接冗余在主表，便于工作台高频查询
3. `business_summary_json` 用于快速展示客户/订单/合同摘要，详细关系仍以关系表为准

---

## 7.3.2 `mail_message`

### 表用途
存储单封邮件元数据，是收件与发件的基础实体。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 是 | 所属线程 ID |
| `mail_account_id` | uuid | 是 | 所属邮箱账号 ID |
| `direction` | smallint | 是 | 邮件方向：收件/发件 |
| `source_type` | smallint | 是 | 来源：同步/系统发件/API创建 |
| `folder_code` | varchar(64) | 是 | 所属逻辑文件夹 |
| `provider_uid` | bigint | 否 | IMAP/Provider 唯一 UID |
| `provider_message_id` | varchar(256) | 否 | 服务端消息 ID |
| `internet_message_id` | varchar(512) | 否 | RFC Message-Id |
| `parent_message_id` | uuid | 否 | 内部父邮件 ID |
| `subject` | varchar(512) | 是 | 邮件主题 |
| `normalized_subject` | varchar(512) | 是 | 标准化主题 |
| `sent_time` | timestamptz | 否 | 发件时间 |
| `received_time` | timestamptz | 否 | 收件时间 |
| `size_bytes` | bigint | 是 | 邮件大小 |
| `importance` | smallint | 是 | 重要级别 |
| `sensitivity` | smallint | 是 | 敏感级别 |
| `has_attachment` | boolean | 是 | 是否有附件 |
| `is_html` | boolean | 是 | 正文是否 HTML |
| `is_draft` | boolean | 是 | 是否草稿 |
| `is_deleted_in_provider` | boolean | 是 | 服务端是否已删除 |
| `send_task_id` | uuid | 否 | 来源发件任务 ID |
| `raw_headers_json` | jsonb | 是 | 标准化邮件头 |
| `references_header` | text | 否 | References 原始内容 |
| `message_hash` | varchar(64) | 否 | 内容哈希摘要 |

### 索引与约束

- 唯一索引：`uk_mail_message_account_folder_uid(mail_account_id, folder_code, provider_uid)`
  > 说明：`provider_uid` 为空时不参与唯一性判断
- 普通索引：
  - `idx_mail_message_thread_received(mail_thread_id, received_time desc)`
  - `idx_mail_message_account_received(mail_account_id, received_time desc)`
  - `idx_mail_message_internet_message_id(internet_message_id)`
  - `idx_mail_message_parent_message_id(parent_message_id)`
  - `idx_mail_message_send_task_id(send_task_id)`
  - `idx_mail_message_folder_code(folder_code)`
  - `idx_mail_message_direction(direction)`

### 设计说明

1. 单封邮件可来源于同步，也可来源于系统发件
2. 邮件正文不直接放主表，避免列表查询负担
3. `folder_code` 用于统一映射收件箱、已发送、草稿箱、归档箱、垃圾箱

---

## 7.3.3 `mail_message_body`

### 表用途
存储邮件正文、摘要、全文检索向量和原始 MIME 路径等大字段内容。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_message_id` | uuid | 是 | 邮件 ID，建议一对一 |
| `text_body` | text | 否 | 纯文本正文 |
| `html_body` | text | 否 | 原始 HTML 正文 |
| `sanitized_html_body` | text | 否 | 安全清洗后的 HTML |
| `body_summary` | varchar(1000) | 否 | 摘要内容 |
| `raw_mime_object_key` | varchar(512) | 否 | 原始 MIME 对象存储路径 |
| `search_vector` | tsvector | 否 | 全文检索向量 |
| `html_sanitized_at` | timestamptz | 否 | HTML 清洗时间 |

### 索引与约束

- 主键 / 唯一：`pk_mail_message_body(mail_message_id)`
- GIN 索引：`idx_mail_message_body_search_vector`
- 普通索引：
  - `idx_mail_message_body_sanitized_at(html_sanitized_at)`

### 设计说明

1. `html_body` 保留原始内容，`sanitized_html_body` 用于安全展示
2. `search_vector` 可由应用层或数据库触发器维护
3. 原始 MIME 可选落对象存储，避免数据库暴涨

---

## 7.3.4 `mail_message_address`

### 表用途
存储发件人、收件人、抄送、密送、回复地址等邮件地址信息。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_message_id` | uuid | 是 | 邮件 ID |
| `address_type` | smallint | 是 | 1From 2To 3Cc 4Bcc 5ReplyTo 6Sender |
| `display_name` | varchar(256) | 否 | 显示名 |
| `email_address` | varchar(256) | 是 | 邮箱地址 |
| `normalized_email_address` | varchar(256) | 是 | 标准化邮箱地址 |
| `sort_order` | int | 是 | 顺序 |

### 索引与约束

- 索引：
  - `idx_mail_message_address_message(mail_message_id)`
  - `idx_mail_message_address_email(normalized_email_address)`
  - `idx_mail_message_address_type(address_type)`

### 设计说明

使用独立地址表而非固定列，便于：

- 多收件人
- 多抄送/密送
- 后续匹配联系人与业务对象
- 保持统一结构

---

## 7.3.5 `mail_tag`

### 表用途
定义标签主数据，可用于系统级、组织级、个人级标签。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `tag_name` | varchar(64) | 是 | 标签名称 |
| `tag_color` | varchar(16) | 否 | 标签颜色 |
| `scope_type` | smallint | 是 | 1系统 2组织 3个人 |
| `owner_org_id` | uuid | 否 | 所属组织 |
| `owner_user_id` | uuid | 否 | 所属用户 |
| `is_enabled` | boolean | 是 | 是否启用 |
| `sort_order` | int | 是 | 排序值 |

### 索引与约束

- 唯一索引建议：`uk_mail_tag_scope_name(scope_type, owner_org_id, owner_user_id, tag_name)`

---

## 7.3.6 `mail_thread_tag_rel`

### 表用途
线程与标签的关联表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 是 | 线程 ID |
| `mail_tag_id` | uuid | 是 | 标签 ID |
| `source_type` | smallint | 是 | 1系统规则 2人工打标 |
| `operator_id` | uuid | 否 | 操作人 |

### 索引与约束

- 唯一索引：`uk_mail_thread_tag_rel(mail_thread_id, mail_tag_id)`
- 普通索引：
  - `idx_mail_thread_tag_rel_tag(mail_tag_id)`

---

## 7.3.7 `mail_message_user_state`

### 表用途
记录用户维度的阅读、星标、隐藏等个性化状态。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_message_id` | uuid | 是 | 邮件 ID |
| `user_id` | uuid | 是 | 用户 ID |
| `is_read` | boolean | 是 | 是否已读 |
| `read_time` | timestamptz | 否 | 阅读时间 |
| `is_starred` | boolean | 是 | 是否星标 |
| `is_hidden` | boolean | 是 | 是否隐藏 |
| `personal_archived_time` | timestamptz | 否 | 个人归档时间 |

### 索引与约束

- 唯一索引：`uk_mail_message_user_state(mail_message_id, user_id)`
- 普通索引：
  - `idx_mail_message_user_state_user(user_id, is_read, is_starred)`

---

## 7.3.8 `mail_attachment`

### 表用途
存储附件元数据，文件内容存对象存储。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_message_id` | uuid | 是 | 所属邮件 ID |
| `file_name` | varchar(512) | 是 | 文件名 |
| `file_extension` | varchar(32) | 否 | 扩展名 |
| `content_type` | varchar(128) | 否 | MIME 类型 |
| `size_bytes` | bigint | 是 | 文件大小 |
| `sha256` | varchar(64) | 否 | 文件哈希 |
| `object_key` | varchar(512) | 是 | 对象存储路径 |
| `storage_provider` | varchar(32) | 是 | 存储提供方 |
| `is_inline` | boolean | 是 | 是否内嵌资源 |
| `content_id` | varchar(256) | 否 | HTML 内嵌 Content-Id |
| `access_level` | smallint | 是 | 访问级别 |
| `preview_status` | smallint | 是 | 预览状态 |
| `preview_object_key` | varchar(512) | 否 | 预览文件对象路径 |
| `virus_scan_status` | smallint | 是 | 病毒扫描状态 |
| `download_count` | bigint | 是 | 下载次数 |

### 索引与约束

- 索引：
  - `idx_mail_attachment_message(mail_message_id)`
  - `idx_mail_attachment_sha256(sha256)`
  - `idx_mail_attachment_access_level(access_level)`
  - `idx_mail_attachment_virus_scan_status(virus_scan_status)`

### 设计说明

1. 附件二进制内容不入库
2. `sha256` 用于后续去重、核验、审计
3. `access_level` 支持普通附件 / 敏感附件分级控制

---

## 7.3.9 `mail_attachment_access_log`

### 表用途
记录附件查看、下载等敏感行为。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_attachment_id` | uuid | 是 | 附件 ID |
| `mail_message_id` | uuid | 是 | 邮件 ID |
| `user_id` | uuid | 否 | 操作用户 |
| `access_type` | smallint | 是 | 1预览 2下载 |
| `client_ip` | varchar(64) | 否 | 客户端 IP |
| `user_agent` | varchar(1024) | 否 | 浏览器标识 |
| `access_time` | timestamptz | 是 | 访问时间 |
| `is_success` | boolean | 是 | 是否成功 |
| `failure_reason` | varchar(512) | 否 | 失败原因 |

### 索引与约束

- 索引：
  - `idx_mail_attachment_access_log_attachment(mail_attachment_id, access_time desc)`
  - `idx_mail_attachment_access_log_user(user_id, access_time desc)`
  - `idx_mail_attachment_access_log_time(access_time desc)`

---

## 7.3.10 `mail_send_task`

### 表用途
存储草稿、待发邮件、定时发件和重试状态，是主动发件流程主表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 否 | 归属线程 ID |
| `mail_account_id` | uuid | 是 | 发件邮箱账号 ID |
| `created_by_user_id` | uuid | 否 | 发起人 |
| `subject` | varchar(512) | 是 | 原始主题 |
| `rendered_subject` | varchar(512) | 否 | 渲染后主题 |
| `body_text` | text | 否 | 纯文本正文 |
| `body_html` | text | 否 | HTML 正文 |
| `rendered_body_text` | text | 否 | 渲染后纯文本 |
| `rendered_body_html` | text | 否 | 渲染后 HTML |
| `status` | smallint | 是 | 发件任务状态 |
| `priority` | smallint | 是 | 优先级 |
| `template_id` | uuid | 否 | 模板 ID |
| `template_version_id` | uuid | 否 | 模板版本 ID |
| `signature_id` | uuid | 否 | 签名 ID |
| `variables_json` | jsonb | 是 | 模板变量快照 |
| `scheduled_send_time` | timestamptz | 否 | 定时发送时间 |
| `submitted_at` | timestamptz | 否 | 提交审批时间 |
| `approved_at` | timestamptz | 否 | 审批通过时间 |
| `send_started_at` | timestamptz | 否 | 开始发件时间 |
| `sent_at` | timestamptz | 否 | 发件成功时间 |
| `retry_count` | int | 是 | 已重试次数 |
| `max_retry_count` | int | 是 | 最大重试次数 |
| `error_code` | varchar(64) | 否 | 错误码 |
| `error_message` | text | 否 | 错误信息 |
| `need_approval` | boolean | 是 | 是否需要审批 |
| `approval_policy_code` | varchar(64) | 否 | 审批策略编码 |
| `idempotency_key` | varchar(128) | 否 | 幂等键 |
| `external_biz_ref` | varchar(128) | 否 | 外部业务引用 |

### 索引与约束

- 唯一索引建议：`uk_mail_send_task_idempotency_key(tenant_id, idempotency_key)`
  > 当 `idempotency_key` 不为空时生效
- 普通索引：
  - `idx_mail_send_task_account_status(mail_account_id, status)`
  - `idx_mail_send_task_status_scheduled(status, scheduled_send_time)`
  - `idx_mail_send_task_thread(mail_thread_id)`
  - `idx_mail_send_task_created_by(created_by_user_id)`
  - `idx_mail_send_task_sent_at(sent_at desc)`

### 设计说明

1. 草稿与待发任务统一建模为 `mail_send_task`
2. 发件成功后可同步生成对应 `mail_message` 记录
3. 外部系统创建邮件时可通过 `idempotency_key` 保证幂等

---

## 7.3.11 `mail_send_task_recipient`

### 表用途
存储待发邮件的收件人、抄送、密送信息。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_send_task_id` | uuid | 是 | 发件任务 ID |
| `recipient_type` | smallint | 是 | 1To 2Cc 3Bcc |
| `display_name` | varchar(256) | 否 | 显示名 |
| `email_address` | varchar(256) | 是 | 邮箱地址 |
| `normalized_email_address` | varchar(256) | 是 | 标准化邮箱地址 |
| `sort_order` | int | 是 | 顺序 |

### 索引与约束

- 索引：
  - `idx_mail_send_task_recipient_task(mail_send_task_id)`
  - `idx_mail_send_task_recipient_email(normalized_email_address)`

---

# 7.4 协同与审批域

## 7.4.1 `mail_thread_assignment`

### 表用途
记录线程责任流转过程，包括自动分派、认领、转派、退回共享池等。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 是 | 线程 ID |
| `from_assignee_type` | smallint | 否 | 原负责人类型 |
| `from_assignee_id` | uuid | 否 | 原负责人 ID |
| `to_assignee_type` | smallint | 否 | 新负责人类型 |
| `to_assignee_id` | uuid | 否 | 新负责人 ID |
| `action_type` | smallint | 是 | 分派动作类型 |
| `reason` | varchar(512) | 否 | 说明 |
| `is_current` | boolean | 是 | 是否当前有效记录 |
| `assigned_at` | timestamptz | 是 | 分派时间 |
| `completed_at` | timestamptz | 否 | 失效 / 完成时间 |

### 索引与约束

- 索引：
  - `idx_mail_thread_assignment_thread(mail_thread_id, assigned_at desc)`
  - `idx_mail_thread_assignment_current(is_current, to_assignee_type, to_assignee_id)`
  - `idx_mail_thread_assignment_to_assignee(to_assignee_type, to_assignee_id)`

---

## 7.4.2 `mail_internal_note`

### 表用途
记录内部备注、协同建议、风险说明、@提醒内容，不对外发送。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 是 | 线程 ID |
| `mail_message_id` | uuid | 否 | 关联邮件 ID |
| `note_type` | smallint | 是 | 备注类型 |
| `content` | text | 是 | 备注内容 |
| `mention_user_ids_json` | jsonb | 是 | 被提醒用户列表 |
| `is_pinned` | boolean | 是 | 是否置顶 |

### 索引与约束

- 索引：
  - `idx_mail_internal_note_thread(mail_thread_id, creation_time desc)`
  - `idx_mail_internal_note_message(mail_message_id)`

---

## 7.4.3 `mail_thread_activity`

### 表用途
记录线程生命周期中的关键事件，作为协同时间线数据源。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_thread_id` | uuid | 是 | 线程 ID |
| `event_type` | smallint | 是 | 事件类型 |
| `event_time` | timestamptz | 是 | 事件时间 |
| `operator_id` | uuid | 否 | 操作人 |
| `related_entity_type` | varchar(64) | 否 | 关联实体类型 |
| `related_entity_id` | uuid | 否 | 关联实体 ID |
| `summary` | varchar(512) | 否 | 摘要 |
| `payload_json` | jsonb | 是 | 事件快照 |

### 索引与约束

- 索引：
  - `idx_mail_thread_activity_thread(mail_thread_id, event_time desc)`
  - `idx_mail_thread_activity_operator(operator_id, event_time desc)`
  - `idx_mail_thread_activity_type(event_type, event_time desc)`

---

## 7.4.4 `mail_approval`

### 表用途
发件审批主表，负责记录审批主状态。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `business_type` | varchar(64) | 是 | 业务类型，默认 SEND_TASK |
| `business_id` | uuid | 是 | 业务对象 ID |
| `mail_thread_id` | uuid | 否 | 归属线程 ID |
| `applicant_id` | uuid | 否 | 发起人 |
| `current_status` | smallint | 是 | 审批状态 |
| `current_step_no` | int | 是 | 当前步骤号 |
| `policy_code` | varchar(64) | 否 | 审批策略编码 |
| `submitted_at` | timestamptz | 是 | 提交时间 |
| `decided_at` | timestamptz | 否 | 最终决策时间 |
| `decision_note` | text | 否 | 结论备注 |
| `snapshot_json` | jsonb | 是 | 提交时快照 |

### 索引与约束

- 索引：
  - `idx_mail_approval_business(business_type, business_id)`
  - `idx_mail_approval_status(current_status, submitted_at desc)`
  - `idx_mail_approval_applicant(applicant_id, submitted_at desc)`

---

## 7.4.5 `mail_approval_record`

### 表用途
审批步骤记录表，用于记录每个审批节点和处理意见。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_approval_id` | uuid | 是 | 审批主表 ID |
| `step_no` | int | 是 | 步骤号 |
| `approver_type` | smallint | 是 | 审批人类型：用户/角色/组织 |
| `approver_id` | uuid | 是 | 审批主体 ID |
| `action_status` | smallint | 是 | 节点状态 |
| `action_time` | timestamptz | 否 | 处理时间 |
| `comment` | text | 否 | 审批意见 |

### 索引与约束

- 索引：
  - `idx_mail_approval_record_approval(mail_approval_id, step_no)`
  - `idx_mail_approval_record_approver(approver_type, approver_id, action_status)`

---

# 7.5 模板与规则域

## 7.5.1 `mail_template_category`

### 表用途
邮件模板分类表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `category_name` | varchar(128) | 是 | 分类名称 |
| `category_code` | varchar(64) | 是 | 分类编码 |
| `parent_id` | uuid | 否 | 上级分类 |
| `sort_order` | int | 是 | 排序 |
| `is_enabled` | boolean | 是 | 是否启用 |

### 索引与约束

- 唯一索引：`uk_mail_template_category_code(tenant_id, category_code)`

---

## 7.5.2 `mail_template`

### 表用途
模板主表，存放当前有效模板定义。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `template_code` | varchar(64) | 是 | 模板编码 |
| `template_name` | varchar(128) | 是 | 模板名称 |
| `mail_template_category_id` | uuid | 否 | 分类 ID |
| `language_code` | varchar(16) | 否 | 语言 |
| `current_version_no` | int | 是 | 当前版本号 |
| `subject_template` | text | 是 | 主题模板 |
| `body_template` | text | 是 | 正文模板 |
| `is_html` | boolean | 是 | 是否 HTML 模板 |
| `is_enabled` | boolean | 是 | 是否启用 |
| `need_publish_approval` | boolean | 是 | 是否需要发布审批 |
| `description` | varchar(512) | 否 | 描述 |

### 索引与约束

- 唯一索引：`uk_mail_template_code(tenant_id, template_code)`
- 普通索引：
  - `idx_mail_template_category(mail_template_category_id)`
  - `idx_mail_template_enabled(is_enabled)`

---

## 7.5.3 `mail_template_version`

### 表用途
模板版本历史表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_template_id` | uuid | 是 | 模板 ID |
| `version_no` | int | 是 | 版本号 |
| `subject_template` | text | 是 | 主题模板 |
| `body_template` | text | 是 | 正文模板 |
| `variables_schema_json` | jsonb | 是 | 变量定义快照 |
| `change_summary` | varchar(512) | 否 | 变更说明 |
| `published_at` | timestamptz | 否 | 发布时间 |
| `published_by` | uuid | 否 | 发布人 |

### 索引与约束

- 唯一索引：`uk_mail_template_version(mail_template_id, version_no)`

---

## 7.5.4 `mail_signature`

### 表用途
邮件签名配置表，支持个人、组织、系统级签名。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `signature_name` | varchar(128) | 是 | 签名名称 |
| `scope_type` | smallint | 是 | 1系统 2组织 3个人 |
| `owner_org_id` | uuid | 否 | 所属组织 |
| `owner_user_id` | uuid | 否 | 所属用户 |
| `content_html` | text | 否 | HTML 签名 |
| `content_text` | text | 否 | 文本签名 |
| `is_default` | boolean | 是 | 是否默认 |
| `is_enabled` | boolean | 是 | 是否启用 |

### 索引与约束

- 索引：
  - `idx_mail_signature_scope(scope_type, owner_org_id, owner_user_id)`
  - `idx_mail_signature_default(is_default)`

---

## 7.5.5 `mail_rule`

### 表用途
自动分类、自动分派、黑白名单、自动归档等规则主表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `rule_code` | varchar(64) | 是 | 规则编码 |
| `rule_name` | varchar(128) | 是 | 规则名称 |
| `rule_type` | smallint | 是 | 规则类型 |
| `priority` | int | 是 | 优先级，越小越先执行 |
| `scope_type` | smallint | 是 | 规则适用范围 |
| `owner_org_id` | uuid | 否 | 所属组织 |
| `mail_account_id` | uuid | 否 | 绑定邮箱账号 |
| `condition_json` | jsonb | 是 | 规则条件 |
| `action_json` | jsonb | 是 | 规则动作 |
| `stop_on_match` | boolean | 是 | 命中后是否停止后续规则 |
| `is_enabled` | boolean | 是 | 是否启用 |
| `effective_from` | timestamptz | 否 | 生效开始时间 |
| `effective_to` | timestamptz | 否 | 生效结束时间 |
| `description` | varchar(512) | 否 | 描述 |

### 索引与约束

- 唯一索引：`uk_mail_rule_code(tenant_id, rule_code)`
- 普通索引：
  - `idx_mail_rule_type_enabled(rule_type, is_enabled)`
  - `idx_mail_rule_account(mail_account_id)`
  - `idx_mail_rule_priority(priority)`

### 设计说明

规则条件和动作使用 `jsonb` 存储，原因：

- 规则结构灵活
- 便于前端动态配置
- 便于后续新增规则条件与动作类型
- 减少大量规则子表带来的复杂 Join

---

## 7.5.6 `mail_rule_execution_log`

### 表用途
记录规则命中与执行情况。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_rule_id` | uuid | 是 | 规则 ID |
| `mail_message_id` | uuid | 否 | 命中邮件 ID |
| `mail_thread_id` | uuid | 否 | 命中线程 ID |
| `executed_at` | timestamptz | 是 | 执行时间 |
| `is_matched` | boolean | 是 | 是否命中 |
| `is_success` | boolean | 是 | 是否执行成功 |
| `matched_condition_json` | jsonb | 是 | 命中条件快照 |
| `executed_action_json` | jsonb | 是 | 执行动作快照 |
| `error_message` | text | 否 | 错误信息 |

### 索引与约束

- 索引：
  - `idx_mail_rule_execution_log_rule(mail_rule_id, executed_at desc)`
  - `idx_mail_rule_execution_log_message(mail_message_id)`
  - `idx_mail_rule_execution_log_thread(mail_thread_id)`
  - `idx_mail_rule_execution_log_executed_at(executed_at desc)`

---

# 7.6 联系人与业务关联域

## 7.6.1 `mail_contact`

### 表用途
联系人主表，用于客户、供应商等邮件联系人基础信息管理。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `contact_name` | varchar(128) | 是 | 联系人姓名 |
| `mobile_phone` | varchar(32) | 否 | 手机号 |
| `company_name` | varchar(256) | 否 | 公司名称 |
| `source_system` | varchar(64) | 否 | 来源系统 |
| `source_object_type` | varchar(64) | 否 | 外部对象类型 |
| `source_object_id` | varchar(128) | 否 | 外部对象 ID |
| `owner_org_id` | uuid | 否 | 所属组织 |
| `last_contact_time` | timestamptz | 否 | 最近联系时间 |
| `relation_count` | int | 是 | 关联邮件数 |

### 索引与约束

- 索引：
  - `idx_mail_contact_name(contact_name)`
  - `idx_mail_contact_source(source_system, source_object_type, source_object_id)`
  - `idx_mail_contact_last_contact_time(last_contact_time desc)`

---

## 7.6.2 `mail_contact_email`

### 表用途
联系人邮箱表，支持一个联系人多个邮箱地址。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `mail_contact_id` | uuid | 是 | 联系人 ID |
| `email_address` | varchar(256) | 是 | 邮箱地址 |
| `normalized_email_address` | varchar(256) | 是 | 标准化邮箱地址 |
| `is_primary` | boolean | 是 | 是否主邮箱 |
| `is_verified` | boolean | 是 | 是否已验证 |

### 索引与约束

- 唯一索引：`uk_mail_contact_email(normalized_email_address)`
- 普通索引：
  - `idx_mail_contact_email_contact(mail_contact_id)`

---

## 7.6.3 `mail_business_relation`

### 表用途
通用业务关联表，支持将线程、邮件、发件任务与客户、订单、合同、工单等对象关联。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `owner_type` | varchar(32) | 是 | THREAD / MESSAGE / SEND_TASK |
| `owner_id` | uuid | 是 | 归属实体 ID |
| `business_type` | varchar(64) | 是 | Customer / Order / Contract / Ticket 等 |
| `business_id` | varchar(128) | 是 | 外部业务对象主键 |
| `business_code` | varchar(128) | 否 | 外部业务编码 |
| `display_name` | varchar(256) | 否 | 展示名称 |
| `is_primary` | boolean | 是 | 是否主关联 |
| `source_type` | smallint | 是 | 1自动 2人工 3外部系统 |
| `source_ref` | varchar(128) | 否 | 来源引用 |
| `relation_metadata_json` | jsonb | 是 | 关联补充信息 |

### 索引与约束

- 唯一索引建议：`uk_mail_business_relation(owner_type, owner_id, business_type, business_id)`
- 普通索引：
  - `idx_mail_business_relation_business(business_type, business_id)`
  - `idx_mail_business_relation_owner(owner_type, owner_id)`
  - `idx_mail_business_relation_primary(is_primary)`

### 设计说明

该表采用通用关联模型，避免为每种业务对象单独建一张关系表，便于后续扩展自定义业务对象。

---

# 7.7 集成开放平台域

## 7.7.1 `integration_app`

### 表用途
外部系统接入应用表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `app_name` | varchar(128) | 是 | 应用名称 |
| `app_key` | varchar(64) | 是 | 应用标识 |
| `encrypted_app_secret` | text | 是 | 加密密钥 |
| `status` | smallint | 是 | 应用状态 |
| `allowed_scopes_json` | jsonb | 是 | 权限范围 |
| `callback_base_url` | varchar(512) | 否 | 回调基础地址 |
| `last_call_time` | timestamptz | 否 | 最近调用时间 |
| `description` | varchar(512) | 否 | 描述 |

### 索引与约束

- 唯一索引：`uk_integration_app_app_key(app_key)`
- 普通索引：
  - `idx_integration_app_status(status)`

---

## 7.7.2 `integration_idempotency_record`

### 表用途
记录外部系统写接口幂等键，防止重复创建数据。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `integration_app_id` | uuid | 是 | 应用 ID |
| `idempotency_key` | varchar(128) | 是 | 幂等键 |
| `request_hash` | varchar(64) | 否 | 请求摘要 |
| `business_type` | varchar(64) | 是 | 业务类型 |
| `business_id` | uuid | 否 | 业务主键 |
| `created_at` | timestamptz | 是 | 创建时间 |
| `expired_at` | timestamptz | 否 | 过期时间 |

### 索引与约束

- 唯一索引：`uk_integration_idempotency_record(integration_app_id, idempotency_key)`
- 普通索引：
  - `idx_integration_idempotency_expired_at(expired_at)`

---

## 7.7.3 `integration_api_call_log`

### 表用途
记录外部应用调用 API 情况，便于审计与排障。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `integration_app_id` | uuid | 否 | 调用应用 ID |
| `request_method` | varchar(16) | 是 | HTTP 方法 |
| `request_path` | varchar(512) | 是 | 请求路径 |
| `trace_id` | varchar(64) | 否 | 追踪 ID |
| `request_time` | timestamptz | 是 | 请求时间 |
| `response_time` | timestamptz | 否 | 响应时间 |
| `status_code` | int | 否 | 响应码 |
| `is_success` | boolean | 是 | 是否成功 |
| `client_ip` | varchar(64) | 否 | 客户端 IP |
| `error_message` | varchar(1024) | 否 | 错误信息 |
| `request_body_json` | jsonb | 是 | 请求快照 |
| `response_body_json` | jsonb | 是 | 响应快照 |

### 索引与约束

- 索引：
  - `idx_integration_api_call_log_app_time(integration_app_id, request_time desc)`
  - `idx_integration_api_call_log_trace_id(trace_id)`
  - `idx_integration_api_call_log_status_code(status_code)`

---

## 7.7.4 `integration_webhook_subscription`

### 表用途
Webhook 订阅配置表。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `integration_app_id` | uuid | 是 | 应用 ID |
| `event_code` | varchar(64) | 是 | 事件编码 |
| `target_url` | varchar(1024) | 是 | 回调地址 |
| `signing_secret` | text | 否 | 签名密钥 |
| `is_enabled` | boolean | 是 | 是否启用 |
| `timeout_seconds` | int | 是 | 超时时间 |
| `retry_policy_json` | jsonb | 是 | 重试策略 |

### 索引与约束

- 唯一索引：`uk_integration_webhook_subscription(integration_app_id, event_code, target_url)`

---

## 7.7.5 `integration_webhook_delivery`

### 表用途
记录每一次 Webhook 投递结果。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `integration_webhook_subscription_id` | uuid | 是 | 订阅 ID |
| `event_code` | varchar(64) | 是 | 事件编码 |
| `event_id` | varchar(128) | 是 | 事件唯一 ID |
| `payload_json` | jsonb | 是 | 推送内容 |
| `delivery_status` | smallint | 是 | 投递状态 |
| `attempt_count` | int | 是 | 重试次数 |
| `last_attempt_at` | timestamptz | 否 | 最后尝试时间 |
| `next_retry_at` | timestamptz | 否 | 下次重试时间 |
| `response_status_code` | int | 否 | 响应码 |
| `response_body` | text | 否 | 响应体 |
| `error_message` | text | 否 | 错误信息 |

### 索引与约束

- 唯一索引：`uk_integration_webhook_delivery_event(event_id, integration_webhook_subscription_id)`
- 普通索引：
  - `idx_integration_webhook_delivery_status(delivery_status, next_retry_at)`
  - `idx_integration_webhook_delivery_event_code(event_code, last_attempt_at desc)`

---

## 7.7.6 `integration_external_object_map`

### 表用途
维护本系统对象与外部系统对象映射关系。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `source_system` | varchar(64) | 是 | 来源系统 |
| `external_object_type` | varchar(64) | 是 | 外部对象类型 |
| `external_object_id` | varchar(128) | 是 | 外部对象 ID |
| `local_object_type` | varchar(64) | 是 | 本地对象类型 |
| `local_object_id` | uuid | 是 | 本地对象 ID |
| `mapping_status` | smallint | 是 | 映射状态 |
| `last_sync_at` | timestamptz | 否 | 最后同步时间 |
| `metadata_json` | jsonb | 是 | 扩展信息 |

### 索引与约束

- 唯一索引：`uk_integration_external_object_map(source_system, external_object_type, external_object_id, local_object_type, local_object_id)`
- 普通索引：
  - `idx_integration_external_object_map_local(local_object_type, local_object_id)`

---

# 7.8 统计分析域

## 7.8.1 `mail_stat_daily_account`

### 表用途
按天汇总邮箱级统计数据。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `stat_date` | date | 是 | 统计日期 |
| `mail_account_id` | uuid | 是 | 邮箱账号 ID |
| `received_count` | int | 是 | 收件数 |
| `sent_count` | int | 是 | 发件数 |
| `failed_send_count` | int | 是 | 发件失败数 |
| `new_thread_count` | int | 是 | 新线程数 |
| `archived_thread_count` | int | 是 | 归档线程数 |
| `avg_first_response_seconds` | int | 否 | 平均首次响应秒数 |
| `avg_process_seconds` | int | 否 | 平均处理时长秒数 |
| `generated_at` | timestamptz | 是 | 生成时间 |

### 索引与约束

- 唯一索引：`uk_mail_stat_daily_account(stat_date, mail_account_id)`

---

## 7.8.2 `mail_stat_daily_user`

### 表用途
按天汇总用户级处理统计数据。

### 字段设计

| 字段 | 类型 | 非空 | 说明 |
|---|---|---|---|
| `stat_date` | date | 是 | 统计日期 |
| `user_id` | uuid | 是 | 用户 ID |
| `assigned_count` | int | 是 | 被分派数 |
| `processed_count` | int | 是 | 已处理线程数 |
| `replied_count` | int | 是 | 回复数 |
| `approved_count` | int | 是 | 审批通过数 |
| `rejected_count` | int | 是 | 审批拒绝数 |
| `avg_response_seconds` | int | 否 | 平均响应时长秒数 |
| `avg_process_seconds` | int | 否 | 平均处理时长秒数 |
| `generated_at` | timestamptz | 是 | 生成时间 |

### 索引与约束

- 唯一索引：`uk_mail_stat_daily_user(stat_date, user_id)`

---

# 8. 关键索引设计

## 8.1 索引总体原则

索引设计目标：

1. 支撑高频查询
2. 控制写入成本
3. 避免无效索引
4. 重点保障收件箱、线程列表、工作台、审批中心、搜索和同步任务性能

## 8.2 高频查询对应索引

### 8.2.1 收件箱 / 线程列表
常见过滤条件：

- `mail_account_id`
- `thread_status`
- `current_assignee_type + current_assignee_id`
- `latest_message_time`
- `due_time`
- 标签
- 是否有附件

重点索引：

- `idx_mail_thread_status_latest`
- `idx_mail_thread_assignee`
- `idx_mail_thread_due_time`

### 8.2.2 邮件详情
依赖索引：

- `idx_mail_message_thread_received`
- `pk_mail_message_body`
- `idx_mail_attachment_message`
- `idx_mail_message_address_message`

### 8.2.3 同步去重
依赖索引：

- `uk_mail_message_account_folder_uid`
- `idx_mail_message_internet_message_id`

### 8.2.4 联系人匹配
依赖索引：

- `uk_mail_contact_email(normalized_email_address)`
- `idx_mail_message_address_email(normalized_email_address)`

### 8.2.5 审批工作台
依赖索引：

- `idx_mail_approval_status`
- `idx_mail_approval_record_approver`

### 8.2.6 Webhook 重试
依赖索引：

- `idx_integration_webhook_delivery_status(delivery_status, next_retry_at)`

## 8.3 全文检索设计

建议在 `mail_message_body.search_vector` 上建立 GIN 索引：

- 支持主题、摘要、纯文本正文基础全文搜索
- 搜索词来源建议包含：
  - 主题
  - 纯文本正文
  - 摘要

如需更高能力，后续接入 Elasticsearch / OpenSearch。

## 8.4 索引控制建议

不建议为以下字段盲目建索引：

- 长文本正文原文
- 大量低选择性布尔字段的单列索引
- 低频查询字段
- 很少作为筛选条件的描述字段

---

# 9. 约束与一致性设计

## 9.1 一致性原则

数据库一致性分为两类：

1. **强一致事务内保证**
   例如发件任务状态更新、审批状态切换、线程负责人更新等

2. **最终一致异步保证**
   例如统计汇总、搜索向量更新、Webhook 推送、业务摘要快照更新等

## 9.2 外键设计原则

- 核心主从表使用外键保证引用完整性
- 高吞吐日志表可视情况弱化外键，改为应用层保证
- 避免大范围级联删除
- 业务表以软删除为主，不依赖物理删除级联

## 9.3 唯一约束重点

建议重点配置以下唯一约束：

- 邮箱地址唯一：`mail_account`
- 同一邮箱同一文件夹同一 UID 唯一：`mail_message`
- 用户个性化状态唯一：`mail_message_user_state`
- 模板编码唯一：`mail_template`
- 规则编码唯一：`mail_rule`
- 外部应用 AppKey 唯一：`integration_app`
- 幂等键唯一：`integration_idempotency_record`

## 9.4 幂等与重复控制

### 收件同步幂等
通过以下维度控制重复入库：

- `(mail_account_id, folder_code, provider_uid)`
- 辅助参考：`internet_message_id`

### 发件幂等
通过以下方式控制：

- `mail_send_task.idempotency_key`
- 任务执行分布式锁
- 发送前状态二次校验

### Webhook 幂等
通过以下方式控制：

- `event_id + subscription_id` 唯一
- 重试记录复用同一 delivery 记录

---

# 10. 分区、归档与生命周期设计

## 10.1 一期策略

一期建议默认不强制所有表分区，但要为高增长表预留分区能力。

## 10.2 高增长候选分区表

后续数据量增长后，优先考虑对以下表按月分区：

- `mail_message`
- `mail_message_body`
- `mail_sync_job_log`
- `mail_thread_activity`
- `mail_attachment_access_log`
- `integration_api_call_log`
- `integration_webhook_delivery`

## 10.3 分区键建议

| 表 | 分区字段 |
|---|---|
| `mail_message` | `received_time` 或 `creation_time` |
| `mail_message_body` | `creation_time` |
| `mail_sync_job_log` | `started_at` |
| `mail_thread_activity` | `event_time` |
| `mail_attachment_access_log` | `access_time` |
| `integration_api_call_log` | `request_time` |
| `integration_webhook_delivery` | `last_attempt_at` |

## 10.4 生命周期建议

| 数据类型 | 建议保留策略 |
|---|---|
| 同步日志 | 180 天 |
| API 调用日志 | 180 天 |
| Webhook 投递日志 | 180 天 |
| 附件访问日志 | 365 天 |
| 协同活动日志 | 365 天或长期 |
| 核心邮件与线程 | 按业务要求长期保留 |
| 原始 MIME | 可按配置选择保留或定期归档 |

## 10.5 归档建议

对于长期不活跃的历史数据，可采用：

- 冷热分层
- 历史表迁移
- 对象存储归档
- BI 或数据仓库离线统计保留

---

# 11. 安全与审计设计

## 11.1 凭据安全

`mail_account` 中的以下字段必须加密存储：

- `encrypted_password`
- `encrypted_access_token`
- `encrypted_refresh_token`

要求：

- 禁止明文日志输出
- 禁止数据库管理员通过应用日志轻易还原
- 建议使用应用层统一加密服务
- 密钥与业务数据库分离保管

## 11.2 正文安全

`mail_message_body` 中：

- 原始 HTML 存 `html_body`
- 展示用 HTML 存 `sanitized_html_body`

要求：

- 展示必须使用清洗后的 HTML
- 原始 HTML 仅在审计或技术排障场景受限使用

## 11.3 附件安全

`mail_attachment` 与 `mail_attachment_access_log` 应支持：

- 按权限下载
- 敏感级别控制
- 下载审计
- 病毒扫描状态追踪
- 预览文件隔离存储

## 11.4 审计建议

以下关键数据变更应有审计记录：

- 邮箱账号配置修改
- 邮箱授权修改
- 规则启停
- 模板发布
- 审批决策
- 线程分派
- 附件访问
- API 调用
- Webhook 推送结果

审计可部分复用 ABP 标准审计表，业务专项访问日志仍建议单独建表。

---

# 12. EF Core 与迁移实施建议

## 12.1 实体映射建议

建议按模块拆分实体和配置：

- `MailAccountDbContextModelCreatingExtensions`
- `MailMessageDbContextModelCreatingExtensions`
- `CollaborationDbContextModelCreatingExtensions`
- `TemplateDbContextModelCreatingExtensions`
- `IntegrationDbContextModelCreatingExtensions`

## 12.2 PostgreSQL 字段映射建议

| C# 类型 | PostgreSQL 类型 |
|---|---|
| `Guid` | `uuid` |
| `string` | `varchar(n)` / `text` |
| `DateTime` / `DateTimeOffset` | `timestamptz` |
| `bool` | `boolean` |
| `int` | `integer` |
| `long` | `bigint` |
| `short` / enum | `smallint` |
| `Dictionary<string, object>` / JSON | `jsonb` |

## 12.3 JSONB 映射建议

以下字段建议统一映射为 `jsonb`：

- `extra_properties`
- `raw_headers_json`
- `variables_json`
- `condition_json`
- `action_json`
- `context_json`
- `payload_json`
- `relation_metadata_json`

## 12.4 Migration 原则

- 每次结构变更必须使用 Migration
- 不允许手工改生产表结构后不回写代码
- 生产数据修复脚本与结构迁移脚本应分离
- 大字段和索引变更要评估锁表风险
- 大索引建议采用分批上线策略

## 12.5 初始化数据建议

建议初始化以下基础数据：

- 系统标签
- 系统模板分类
- 基础规则类型
- 默认文件夹编码
- 默认统计任务配置
- 默认审批策略代码
- 默认事件编码

---

# 13. 结论

本数据库设计围绕“**邮箱纳管、邮件线程化、协同闭环、业务关联、模板规则驱动、开放集成与审计追踪**”六个核心目标展开，采用如下关键策略：

1. **核心事务数据结构化存储于 PostgreSQL**
2. **邮件正文与主元数据拆分**
3. **附件元数据入库、文件内容进入对象存储**
4. **线程作为协同与业务处理核心实体**
5. **规则、模板、外部映射合理使用 JSONB 保持扩展性**
6. **通过索引、唯一约束和幂等设计保障性能与一致性**
7. **为后续分区、归档、多租户、搜索增强保留演进空间**

该数据库设计能够满足一期邮件管理系统的交付要求，也能够为后续系统平台化、规模化和智能化升级提供稳定的数据底座。
