using XactTodo.Domain.SeedWork;
using XactTodo.Infrastructure;

namespace XactTodo.Api
{
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 自动注册Domain项目中的所有Service类
        /// </summary>
        internal static void RegisterServiceTypes(this IServiceCollection services)
        {
            var types = typeof(IService).Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Service"));
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                //该类实现了IService接口
                if (interfaces.Any(p => p == typeof(IService)))
                {
                    //如果该类实现了除IService接口外的另一个xxService接口，则将该类登记为此接口的实现类
                    var intf = interfaces.FirstOrDefault(p => p != typeof(IService) && p.Name.EndsWith("Service"));
                    if (intf != null)
                    {
                        services.AddScoped(intf, type);
                    }
                    else //否则，将直接使用该类登记依赖注入
                    {
                        services.AddScoped(type, type);
                    }
                }
            }
        }

        /// <summary>
        /// 自动注册Domain项目中的所有仓储类
        /// </summary>
        internal static void RegisterRepositoryTypes(this IServiceCollection services)
        {
            //services.AddScoped<IRepository<User, int>, UserRepository>();
            //不用像上面那样每个仓储类注册一次，下面的代码会自动搜索所有实现了仓储接口的类并注册
            var types = typeof(TodoContext).Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Repository"));
            foreach (var type in types)
            {
                //if(typeof(IRepository<,>).IsAssignableFrom(type)) //这样判断是不行的，因为未明确泛型接口的参数
                var intfIRepositoryName = typeof(IRepository<,>).Name;
                if (type.GetInterface(intfIRepositoryName) == null)
                    continue;
                /* 下面将遍历当前仓储类实现的所有接口，如果该接口为IRepository<,>本身或其派生接口，则登记依赖注入
                 * 不过，在注入仓储实例时，以不同的接口类型声明的变量将无法使用同一仓储类的实例，不过影响不大，重点是方便！
                 * 例如在某(几)个Controller构造函数中分别声明了IRepository<User,int>和IUserRepository类型的变量，
                 * 会分别创建两个UserRepository类的实例，不过因为两个实例中的DbContext实例还是同一个，所以没什么影响。
                 */
                var intfaces = type.GetInterfaces();
                for (var i = 0; i < intfaces.Length; i++)
                {
                    var intface = intfaces[i];
                    //留意：intface==typeof(IRepository<,>) 和 intface.Name==typeof(IRepository<,>).Name 并非等效
                    if (intface.Name != intfIRepositoryName && intface.GetInterface(intfIRepositoryName) == null)
                        continue;
                    if (intface.IsGenericTypeDefinition)
                    {
                        var argTypes = intface.GetGenericArguments();
                        if (argTypes.Length > 0)
                            intface = intface.MakeGenericType(argTypes);
                    }
                    services.AddScoped(intface, type);
                }
            }
        }

    }
}
