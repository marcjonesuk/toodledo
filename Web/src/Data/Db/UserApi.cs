﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class UserApi : DbApi
    {
        public static User Select(int id)
        {
            var item = Get($@"SELECT [Id]
                        ,[DisplayName]
                        ,[ProfileImageUrl]
                        FROM[toodledo].[dbo].[User] WHERE [Id] = '{id}'")[0];
            return item;
        }

        public static void Update(int id, string displayName, string profileImageUrl)
        {
            Execute($@"UPDATE [dbo].[User] SET DisplayName = '{displayName}', ProfileImageUrl = '{profileImageUrl}' WHERE id = {id}");
        }

        private static List<User> Get(string sql)
        {
            var results = new List<User>();
            ExecuteReader(sql, (r) => results = Map(r));
            return results;
        }

        public static List<User> Map(SqlDataReader reader)
        {
            var results = new List<User>();
            while (reader.Read())
            {
                var item = new User()
                {
                    Id = reader.GetInt32(0),
                    DisplayName = reader.GetString(1)
                };
                if (reader.GetValue(2) != DBNull.Value)
                {
                    item.ProfileImageUrl = reader.GetString(2);
                }
                results.Add(item);
            }
            return results;
        }

        public static User GetByAspNetId(string aspnetUserId)
        {
            var item = Get($@"SELECT [Id]
                        ,[DisplayName]
                        ,[ProfileImageUrl]
                        FROM[toodledo].[dbo].[User] WHERE [AspNetId] = '{aspnetUserId}'")[0];
            return item;
        }
    }
}
