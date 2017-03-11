﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Data
{
    public static class SqlExtensions
    {
        public static string SqlEncode(this string s)
        {
            if (s == null)
                return null;

            return s.Replace("'", "''");
        }
    }

    public class DbApi
    {
        protected static object Execute(string sql)
        {
            //
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=toodledo;Integrated Security=SSPI;");
            SqlCommand cmd = new SqlCommand();
            //SqlDataReader reader;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            connection.Open();
            var reader = cmd.ExecuteScalar();

            connection.Close();
            return reader;
        }

        protected static void ExecuteReader(string sql, Action<SqlDataReader> a)
        {
            //
            SqlConnection connection = new SqlConnection(@"Data Source=localhost\SQLEXPRESS;Initial Catalog=toodledo;Integrated Security=SSPI;");
            SqlCommand cmd = new SqlCommand();
            SqlDataReader reader;
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;
            cmd.Connection = connection;
            connection.Open();
            reader = cmd.ExecuteReader();
            a(reader);
            connection.Close();
        }

        public int? Insert(User user)
        {
            return (int)(decimal)Execute($@"
                INSERT INTO [dbo].[User] ([Username], [DisplayName]) 
                VALUES ( '{user.DisplayName.SqlEncode()}', '{user.DisplayName.SqlEncode()}' ); 
                SELECT SCOPE_IDENTITY();");
        }
    }
}