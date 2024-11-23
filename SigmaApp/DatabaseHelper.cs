using System;
using System.Collections.Generic;
using System.Data.SQLite;

public class DatabaseHelper
{
    private readonly string connectionString = "Data Source=SigmaDatabase.sqlite;Version=3;";

    public SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(connectionString);
    }

    public void ExecuteNonQuery(string query)
    {
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }

    public List<T> ExecuteQuery<T>(string query, Func<SQLiteDataReader, T> readFunc)
    {
        var list = new List<T>();
        using (var connection = GetConnection())
        {
            connection.Open();
            using (var command = new SQLiteCommand(query, connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(readFunc(reader));
                }
            }
        }
        return list;
    }
}
