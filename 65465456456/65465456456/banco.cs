using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace _65465456456
{
    internal class banco
    {
        public static class MySQLConnection
        {
            private static string connectionString = "Server=localhost;Database=escola;Uid=root;Pwd=123456789;";

            public static MySqlConnection GetConnection()
            {
                var conn = new MySqlConnection(connectionString);
                conn.Open();
                return conn;
            }
        }

    }
}
