using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Volo.Abp.DependencyInjection;
using YANEDGE.EmailManagement.Domain.Services;

namespace YANEDGE.EmailManagement.Services.Implementation;

/// <summary>
/// 模板渲染服务实现
/// 支持 {{variable}} 和 ${variable} 两种语法
/// </summary>
public class TemplateRenderService : ITemplateRenderService, ITransientDependency
{
    // 匹配 {{variable}} 或 ${variable} 格式的变量
    private static readonly Regex VariablePattern = new Regex(
        @"\{\{(\w+)\}\}|\$\{(\w+)\}",
        RegexOptions.Compiled
    );

    public Task<string> RenderAsync(string template, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(template))
        {
            return Task.FromResult(string.Empty);
        }

        if (variables == null || variables.Count == 0)
        {
            return Task.FromResult(template);
        }

        var result = VariablePattern.Replace(template, match =>
        {
            // 获取变量名（可能在group 1或group 2中）
            var variableName = match.Groups[1].Success
                ? match.Groups[1].Value
                : match.Groups[2].Value;

            // 查找变量值（不区分大小写）
            var entry = variables.FirstOrDefault(kvp =>
                string.Equals(kvp.Key, variableName, StringComparison.OrdinalIgnoreCase));

            if (entry.Key != null && entry.Value != null)
            {
                return entry.Value.ToString() ?? string.Empty;
            }

            // 如果变量不存在，保留原始占位符
            return match.Value;
        });

        return Task.FromResult(result);
    }

    public Task<string> RenderSubjectAsync(string subjectTemplate, Dictionary<string, object> variables)
    {
        // 主题不需要HTML转义
        return RenderAsync(subjectTemplate, variables);
    }

    public async Task<string> RenderBodyAsync(string bodyTemplate, Dictionary<string, object> variables, bool isHtml = true)
    {
        if (string.IsNullOrEmpty(bodyTemplate))
        {
            return string.Empty;
        }

        // 如果是HTML，需要对变量值进行HTML转义
        if (isHtml && variables != null)
        {
            var escapedVariables = new Dictionary<string, object>();
            foreach (var kvp in variables)
            {
                var value = kvp.Value;
                if (value is string stringValue)
                {
                    // HTML转义，防止XSS
                    escapedVariables[kvp.Key] = HttpUtility.HtmlEncode(stringValue);
                }
                else
                {
                    escapedVariables[kvp.Key] = value;
                }
            }
            variables = escapedVariables;
        }

        return await RenderAsync(bodyTemplate, variables ?? new Dictionary<string, object>());
    }
}
