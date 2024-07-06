using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;

namespace XactTodo.Api
{
    /// <summary> 
    /// 隐藏接口，不生成到swagger文档展示 
    /// 注意：如果不加[HiddenApi]标记的接口名称和加过标记的隐藏接口名称相同，则该普通接口也会被隐藏不显示，所以建议接口名称最好不要重复
    /// </summary> 
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public partial class HiddenApiAttribute : Attribute { }

    public class HiddenApiFilter : IDocumentFilter
    {

        /// <summary> 
        /// 重写Apply方法，移除隐藏接口的生成 
        /// </summary> 
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
                if (apiDescription.CustomAttributes().Any(a=>a.GetType()==typeof(HiddenApiAttribute)))
                {
                    string key = "/" + apiDescription.RelativePath;
                    if (key.Contains("?"))
                    {
                        int idx = key.IndexOf("?", StringComparison.Ordinal);
                        key = key.Substring(0, idx);
                    }
                    swaggerDoc.Paths.Remove(key);
                }
            }
        }
    }
}