using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace wachturm.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task EnsureDatabaseCreatedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WachturmDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }
}