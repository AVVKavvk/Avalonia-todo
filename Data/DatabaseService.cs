using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using TodoApp.Models;

namespace TodoApp.Data;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService()
    {
        var dbFolder = AppContext.BaseDirectory;
        var dbPath = Path.Combine(dbFolder, "todos.db3");
        _connectionString = $"Data Source={dbPath}";

        InitialiseDatabase();
    }

    private void InitialiseDatabase()
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS TodoItems (
                Id          INTEGER PRIMARY KEY AUTOINCREMENT,
                Title       TEXT    NOT NULL,
                IsCompleted INTEGER NOT NULL DEFAULT 0,
                CreatedAt   TEXT    NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();
    }

    public Task<List<TodoItem>> GetAllTodosAsync() =>
        Task.Run(() =>
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT Id, Title, IsCompleted, CreatedAt FROM TodoItems ORDER BY CreatedAt DESC";

            var items = new List<TodoItem>();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                items.Add(
                    new TodoItem
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        IsCompleted = reader.GetInt32(2) == 1,
                        CreatedAt = DateTime.Parse(reader.GetString(3)),
                    }
                );
            }
            return items;
        });

    public async Task<TodoItem?> GetTodoAsync(int id)
    {
        using var conn = new SqliteConnection(_connectionString);
        conn.Open();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, Title, IsCompleted, CreatedAt FROM TodoItems WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new TodoItem
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                IsCompleted = reader.GetInt32(2) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(3)),
            };
        }
        return null;
    }

    public Task<int> AddTodoAsync(TodoItem item) =>
        Task.Run(() =>
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = """
            INSERT INTO TodoItems (Title, IsCompleted, CreatedAt)
            VALUES ($title, $completed, $created);
            SELECT last_insert_rowid();
            """;
            cmd.Parameters.AddWithValue("$title", item.Title);
            cmd.Parameters.AddWithValue("$completed", item.IsCompleted ? 1 : 0);
            cmd.Parameters.AddWithValue("$created", item.CreatedAt.ToString("o"));

            var id = (long)cmd.ExecuteScalar()!;
            item.Id = (int)id;
            return (int)id;
        });

    public Task<int> UpdateTodoAsync(TodoItem item) =>
        Task.Run(() =>
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = """
            UPDATE TodoItems
            SET Title = $title, IsCompleted = $completed
            WHERE Id = $id
            """;
            cmd.Parameters.AddWithValue("$title", item.Title);
            cmd.Parameters.AddWithValue("$completed", item.IsCompleted ? 1 : 0);
            cmd.Parameters.AddWithValue("$id", item.Id);

            return cmd.ExecuteNonQuery();
        });

    public Task<int> DeleteTodoAsync(TodoItem item) =>
        Task.Run(() =>
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "DELETE FROM TodoItems WHERE Id = $id";
            cmd.Parameters.AddWithValue("$id", item.Id);

            return cmd.ExecuteNonQuery();
        });
}
