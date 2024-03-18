using System;
using System.Data.Common;
using Npgsql;

namespace DatabaseProgramming;

public class MyConnect
{
    string host = "localhost";
    string user = "postgres";
    string pass = "70607114";
    string port = "5432";
    string db = "db_blazor";

    public NpgsqlConnection GetConnection()
    {
        try
        {
            string strConn = String.Format(
                "Host={0};Username={1};Password={2};Database={3};Port={4}",
                host,
                user,
                pass,
                db,
                port
            );
            NpgsqlConnection conn = new NpgsqlConnection(strConn);
            conn.Open();

            return conn;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}
