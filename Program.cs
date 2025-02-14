using FluentMigrator.Runner;
using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using LazyCache;
using DotNetEnv;

Env.Load();
var connectionString = $"Data Source={Env.GetString("DATABASE_PATH")}";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(connectionString)
        .ScanIn(typeof(AddUsersTable).Assembly).For.Migrations());

builder.Services.AddSingleton<IAppCache, CachingService>();
builder.Services.AddSingleton<IDbConnection>(sp =>
{
    return new SqliteConnection(connectionString);
});

var app = builder.Build();

if (args.Length > 0 && args[0] == "migrate")
{
    using (var scope = app.Services.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
    Console.WriteLine("Migrations applied successfully!");
    return; 
}

if (args.Length > 0 && args[0] == "migrate:rollback")
{
    using (var scope = app.Services.CreateScope())
    {
        var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.MigrateDown(0);
    }
    Console.WriteLine("Migrations rolled back!");
    return;
}

app.UseAuthorization();
app.MapControllers();
app.Run();