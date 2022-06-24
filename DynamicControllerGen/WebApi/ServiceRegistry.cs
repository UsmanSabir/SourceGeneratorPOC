namespace WebApi
{
    public static partial class ServiceRegistry
    {
        static partial void MyMethodImpl(IServiceCollection services);

        public static void Register(this IServiceCollection services)
        {
            MyMethodImpl(services);
        }
    }
}
