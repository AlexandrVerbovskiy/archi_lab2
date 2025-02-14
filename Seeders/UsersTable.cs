using FluentMigrator;

[Migration(2)]
public class UsersTable : Migration
{
    public override void Up()
    {
        var users = new[]
        {
            new { Name = "John Doe", Email = "john@example.com" },
            new { Name = "Jane Smith", Email = "jane@example.com" },
            new { Name = "Alice Johnson", Email = "alice@example.com" },
            new { Name = "Bob Williams", Email = "bob@example.com" },
            new { Name = "Charlie Brown", Email = "charlie@example.com" },
            new { Name = "David Miller", Email = "david@example.com" },
            new { Name = "Emma Wilson", Email = "emma@example.com" },
            new { Name = "Frank Harris", Email = "frank@example.com" },
            new { Name = "Grace Clark", Email = "grace@example.com" },
            new { Name = "Henry Lee", Email = "henry@example.com" }
        };

        foreach (var user in users)
        {
            Insert.IntoTable("Users").Row(new
            {
                user.Name,
                user.Email,
            });
        }
    }

    public override void Down()
    {
        Delete.FromTable("Users").AllRows();
    }
}
