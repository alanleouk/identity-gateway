using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class ApplicationBuilderExtensions
    {
        public static void MigrateDatabase<TDb>(this WebApplication app, int timeoutSeconds = 60)
            where TDb : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TDb>();
                var defaultTimeout = context.Database.GetCommandTimeout();
                context.Database.SetCommandTimeout(TimeSpan.FromSeconds(timeoutSeconds));
                context.Database.Migrate();
                context.Database.SetCommandTimeout(defaultTimeout);
            }
        }
    }
}
