using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Data;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var presentationPath = Path.GetFullPath(Path.Combine(basePath, "..", "Presentation"));
        if (!File.Exists(Path.Combine(presentationPath, "appsettings.json")))
        {
            presentationPath = Path.GetFullPath(Path.Combine(basePath, "src", "External", "Presentation"));
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(File.Exists(Path.Combine(presentationPath, "appsettings.json")) ? presentationPath : basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found. Set it in Presentation/appsettings.json or ConnectionStrings__DefaultConnection.");

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromMilliseconds(10),
                errorNumbersToAdd: null);
        });

        return new AppDbContext(optionsBuilder.Options);
    }
}
