using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public class TaskApiFactory : WebApplicationFactory<Program>
{
    // Generated ONCE per factory instance, not per request —
    // this is what keeps every request in a test run pointed at the same data
    private readonly string _dbName = "TestDb_" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.AddDbContext<TaskDbContext>(options =>
                options.UseInMemoryDatabase(_dbName));
        });
    }
}