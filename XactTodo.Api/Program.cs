
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using XactTodo.Api.Authentication;
using XactTodo.Api.Controllers;
using XactTodo.Api.Queries;
using XactTodo.Domain.SeedWork;
using XactTodo.Infrastructure;
using XactTodo.Security.Session;

namespace XactTodo.Api
{
    public class Program
    {
        private static readonly string swaggerDocName = "v1";
        
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<IClaimsSession, ClaimsSession>();

            NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.Config").GetCurrentClassLogger();
            builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
            builder.Host.UseNLog();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<TodoContext>(
                options => options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            using var scope = builder.Services.BuildServiceProvider().CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var context = scope.ServiceProvider.GetRequiredService<TodoContext>();
                await context.Database.MigrateAsync(); // 自动迁移数据库
            }
            catch (Exception ex)
            {
                logger.LogError("执行数据库迁移失败，异常信息：" + ex.Message + Environment.NewLine + "ConnectionString:" + connectionString);
                return;
            }
            // Add services to the container.
            builder.Services.RegisterServiceTypes();
            builder.Services.RegisterRepositoryTypes();
            builder.Services.Add(new ServiceDescriptor(typeof(ITeamQueries), new TeamQueries(connectionString)));
            builder.Services.Add(new ServiceDescriptor(typeof(IMatterQueries), new MatterQueries(connectionString)));
            //添加身份验证
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerDefaults.AuthenticationScheme;
            }).AddBearer();

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //Swagger支持
            bool.TryParse(builder.Configuration.GetSection("SwaggerDoc:Enabled").Value ?? "true", out bool swaggerEnabled);
            if (swaggerEnabled)
            {
                AddSwagger(builder.Configuration, builder.Services);// app.Environment);
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (swaggerEnabled)//if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                        url: $"../swagger/{swaggerDocName}/swagger.json", //这里一定要使用相对路径，不然网站发布到子目录时将报告："Not Found /swagger/v1/swagger.json"
                        // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                        name: "RESTful API v1.0.0"
                    );
                });
            }

            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        public static void AddSwagger(ConfigurationManager configuration, IServiceCollection services)
        {
            services.AddSwaggerGenNewtonsoftSupport(); //支持通过JsonProperty定义别名
            services.AddSwaggerGen(options =>
            {
                var info = new OpenApiInfo
                {
                    Version = configuration.GetSection("SwaggerDoc:Version").Value,
                    Title = configuration.GetSection("SwaggerDoc:Title").Value,
                    Description = configuration.GetSection("SwaggerDoc:Description").Value,
                };
                //读取appsettings.json文件中的联系人信息
                string contactName = configuration.GetSection("SwaggerDoc:ContactName").Value;
                string contactNameEmail = configuration.GetSection("SwaggerDoc:ContactEmail").Value;
                string s = configuration.GetSection("SwaggerDoc:ContactUrl").Value;
                Uri contactUrl = string.IsNullOrEmpty(s) ? null : new Uri(s);
                if (!string.IsNullOrEmpty(contactName))
                    info.Contact = new OpenApiContact { Name = contactName, Email = contactNameEmail, Url = contactUrl };
                //info.License = new OpenApiLicense { Name = contactName, Url = contactUrl }
                options.SwaggerDoc(swaggerDocName, info);

                var xmlPath = Path.Combine(AppContext.BaseDirectory, "Api.xml");
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);

                options.DocumentFilter<HiddenApiFilter>(); // 在接口类、方法标记属性 [HiddenApi]，可以阻止【Swagger文档】生成
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                //options.OperationFilter<SecurityRequirementsOperationFilter>();
                var security = new OpenApiSecurityRequirement
                {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new[] { "readAccess", "writeAccess" }
                        }
                };
                options.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                //给api添加token令牌证书
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }

    }
}
