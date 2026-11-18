using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.Data.SqlClient;

namespace Cars.Core;

public class DatabaseConnection
{
    private readonly string connectionString;

    public DatabaseConnection(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public SqlConnection CreateConnection()
    {
        return new SqlConnection(connectionString);
    }
}

