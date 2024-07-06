
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
                await context.Database.MigrateAsync(); // �Զ�Ǩ�����ݿ�
            }
            catch (Exception ex)
            {
                logger.LogError("ִ�����ݿ�Ǩ��ʧ�ܣ��쳣��Ϣ��" + ex.Message + Environment.NewLine + "ConnectionString:" + connectionString);
                return;
            }
            // Add services to the container.
            builder.Services.RegisterServiceTypes();
            builder.Services.RegisterRepositoryTypes();
            builder.Services.Add(new ServiceDescriptor(typeof(ITeamQueries), new TeamQueries(connectionString)));
            builder.Services.Add(new ServiceDescriptor(typeof(IMatterQueries), new MatterQueries(connectionString)));
            //��������֤
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerDefaults.AuthenticationScheme;
            }).AddBearer();

            builder.Services.AddControllers().AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //Swagger֧��
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
                        // url: ����� SwaggerDoc �� name�� "/swagger/{SwaggerDoc name}/swagger.json"
                        url: $"../swagger/{swaggerDocName}/swagger.json", //����һ��Ҫʹ�����·������Ȼ��վ��������Ŀ¼ʱ�����棺"Not Found /swagger/v1/swagger.json"
                        // description: ��� Swagger UI ���Ͻ��x��ͬ�汾�� SwaggerDocument �@ʾ���Qʹ�á�
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
            services.AddSwaggerGenNewtonsoftSupport(); //֧��ͨ��JsonProperty�������
            services.AddSwaggerGen(options =>
            {
                var info = new OpenApiInfo
                {
                    Version = configuration.GetSection("SwaggerDoc:Version").Value,
                    Title = configuration.GetSection("SwaggerDoc:Title").Value,
                    Description = configuration.GetSection("SwaggerDoc:Description").Value,
                };
                //��ȡappsettings.json�ļ��е���ϵ����Ϣ
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

                options.DocumentFilter<HiddenApiFilter>(); // �ڽӿ��ࡢ����������� [HiddenApi]��������ֹ��Swagger�ĵ�������
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
                options.AddSecurityRequirement(security);//���һ�������ȫ�ְ�ȫ��Ϣ����AddSecurityDefinition����ָ���ķ�������Ҫһ�£�������Bearer��
                //��api���token����֤��
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization",//jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header,//jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
            });
        }

    }
}
