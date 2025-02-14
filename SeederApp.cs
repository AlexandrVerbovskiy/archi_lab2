using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using DotNetEnv;

Env.Load();
var connectionString = $"Data Source={Env.GetString("DATABASE_PATH")}";

namespace MigrationApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddFluentMigratorCore()
                .ConfigureRunner(runner => runner
                    .AddSqlServer()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn("Seeders")
                    .For.Migrations())
                .BuildServiceProvider();

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
            
            Console.WriteLine("Seeders completed successfully");
        }
    }
}
