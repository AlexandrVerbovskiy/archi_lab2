using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using Dtos;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IAppCache _cache;
    private readonly IDbConnection _dbConnection;

    public UsersController(IAppCache cache, IDbConnection dbConnection)
    {
        _cache = cache;
        _dbConnection = dbConnection;
    }

    [HttpGet]
    public ActionResult<List<UserDto>> GetUsers()
    {
        return _cache.GetOrAdd("users", FetchUsersFromDatabase, TimeSpan.FromMinutes(5));
    }

    [HttpPost]
    public ActionResult CreateUser([FromBody] UserDto userDto)
    {
        if (userDto == null)
        {
            return BadRequest("Invalid user data.");
        }

        using (var connection = _dbConnection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "INSERT INTO Users (Name, Email) VALUES (@Name, @Email)";
                command.Parameters.Add(new SqliteParameter("@Name", userDto.Name));
                command.Parameters.Add(new SqliteParameter("@Email", userDto.Email));
                command.ExecuteNonQuery();
            }
        }

        _cache.Remove("users");
        return Ok("Created successfully.");
    }

    [HttpPut("{id}")]
    public ActionResult UpdateUser(int id, [FromBody] UserDto userDto)
    {
        if (userDto == null || id != userDto.Id)
        {
            return BadRequest("Invalid user data.");
        }

        using (var connection = _dbConnection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id";
                command.Parameters.Add(new SqliteParameter("@Name", userDto.Name));
                command.Parameters.Add(new SqliteParameter("@Email", userDto.Email));
                command.Parameters.Add(new SqliteParameter("@Id", userDto.Id));
                command.ExecuteNonQuery();
            }
        }

        _cache.Remove("users");
        return Ok("Updated successfully.");
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteUser(int id)
    {
        using (var connection = _dbConnection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DELETE FROM Users WHERE Id = @Id";
                command.Parameters.Add(new SqliteParameter("@Id", id));
                command.ExecuteNonQuery();
            }
        }

        _cache.Remove("users");
        return Ok("Deleted successfully.");
    }

    private List<UserDto> FetchUsersFromDatabase()
    {
        var users = new List<UserDto>();

        using (var connection = _dbConnection)
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Id, Name, Email FROM Users";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserDto
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2),
                        });
                    }
                }
            }
        }

        //test

        return users;
    }
}
