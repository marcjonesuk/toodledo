using System;
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
                        FROM[toodledo].[dbo].[User] WHERE [Id] = '{id}'")[0];
            return item;
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
                results.Add(item);
            }
            return results;
        }

        public static User GetByAspNetId(string aspnetUserId)
        {
            var item = Get($@"SELECT [Id]
                        ,[DisplayName]
                        FROM[toodledo].[dbo].[User] WHERE [AspNetId] = '{aspnetUserId}'")[0];
            return item;
        }
    }
}
