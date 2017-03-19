using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Db
{
    public class ContentHistoryApi : DbApi
    {
        public static int Insert(string fieldName, Content oldContent, int userId)
        {
            foreach (var property in oldContent.GetType().GetProperties())
            {
                if (fieldName == property.Name)
                {
                    return Insert(new ContentHistory
                    {
                        ContentId = oldContent.Id,
                        ChangedField = property.Name,
                        OldValue = (string)property.GetValue(oldContent),
                        Changed = DateTime.UtcNow,
                        ChangedByUserId = userId
                    });
                }
            }
            throw new Exception($"Field {fieldName} does not exist on type " + nameof(Content));
        }

        public static int Insert(ContentHistory contentHistory)
        {
            return (int)(decimal)Execute($@"
                INSERT INTO [dbo].[ContentHistory] (ContentId, Field, OldValue, Changed, ChangedBy) 
                VALUES ( '{contentHistory.ContentId}', '{contentHistory.ChangedField}', '{contentHistory.OldValue.SqlEncode()}', '{FormatDate(contentHistory.Changed)}', '{contentHistory.ChangedByUserId}'); 
                SELECT SCOPE_IDENTITY();");
        }

        public static List<ContentHistory> SelectByContentId(int id)
        {
            List<ContentHistory> contentHistory;
            var key = $"contenthistory-{id}";

            //if (!Cache.TryGetValue(key, out contentHistory))
            //{
                contentHistory = GetContentHistory($@"SELECT [Id]
                          ,[ContentId]
                          ,[Field]
                          ,[OldValue]
                          ,[Changed]
                          ,[ChangedBy]
                        FROM[toodledo].[dbo].[ContentHistory] WHERE [ContentId] = '{id}'");

            //    Cache.Set(key, contentHistory, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));
            //}

            return contentHistory;
        }

        private static List<ContentHistory> GetContentHistory(string sql)
        {
            var results = new List<ContentHistory>();
            ExecuteReader(sql, (r) => results = Map(r));
            return results;
        }

        public static List<ContentHistory> Map(SqlDataReader reader)
        {
            var results = new List<ContentHistory>();
            while (reader.Read())
            {
                var item = new ContentHistory()
                {
                    Id = reader.GetInt32(0),
                    ContentId = reader.GetInt32(1),
                    ChangedField = reader.GetString(2),
                    OldValue = reader.GetString(3),
                    Changed = reader.GetDateTime(4),
                    ChangedByUserId = reader.GetInt32(5)
                };
                results.Add(item);
            }
            return results;
        }
    }
}
