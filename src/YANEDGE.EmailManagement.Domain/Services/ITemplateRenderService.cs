using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YANEDGE.EmailManagement.Domain.Services;

/// <summary>
/// 模板渲染服务接口
/// </summary>
public interface ITemplateRenderService
{
    /// <summary>
    /// 渲染模板
    /// </summary>
    /// <param name="template">模板内容</param>
    /// <param name="variables">变量字典</param>
    /// <returns>渲染后的内容</returns>
    Task<string> RenderAsync(string template, Dictionary<string, object> variables);

    /// <summary>
    /// 渲染主题模板
    /// </summary>
    /// <param name="subjectTemplate">主题模板</param>
    /// <param name="variables">变量字典</param>
    /// <returns>渲染后的主题</returns>
    Task<string> RenderSubjectAsync(string subjectTemplate, Dictionary<string, object> variables);

    /// <summary>
    /// 渲染正文模板
    /// </summary>
    /// <param name="bodyTemplate">正文模板</param>
    /// <param name="variables">变量字典</param>
    /// <param name="isHtml">是否为HTML</param>
    /// <returns>渲染后的正文</returns>
    Task<string> RenderBodyAsync(string bodyTemplate, Dictionary<string, object> variables, bool isHtml = true);
}
