namespace YANEDGE.EmailManagement.Enums;

/// <summary>
/// 业务对象类型
/// </summary>
public enum BusinessObjectType
{
    /// <summary>
    /// 客户
    /// </summary>
    Customer = 0,

    /// <summary>
    /// 供应商
    /// </summary>
    Supplier = 10,

    /// <summary>
    /// 订单
    /// </summary>
    Order = 20,

    /// <summary>
    /// 合同
    /// </summary>
    Contract = 30,

    /// <summary>
    /// 工单
    /// </summary>
    WorkOrder = 40,

    /// <summary>
    /// 项目
    /// </summary>
    Project = 50,

    /// <summary>
    /// 发票
    /// </summary>
    Invoice = 60,

    /// <summary>
    /// 报价单
    /// </summary>
    Quotation = 70,

    /// <summary>
    /// 自定义对象
    /// </summary>
    Custom = 999
}
