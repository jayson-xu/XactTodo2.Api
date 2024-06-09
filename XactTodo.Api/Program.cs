
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
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
            }catch(Exception ex)
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
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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
    }
}
