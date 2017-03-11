using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class ContentApi : DbApi
    {
        public static int Insert(Content content)
        {
            return (int)(decimal)Execute($@"
                INSERT INTO [dbo].[Content] ([Title], [Body], [UserId], [Type], [HtmlBody]) 
                VALUES ( '{content.Title.SqlEncode()}', '{content.Body.SqlEncode()}', {content.UserId}, '{content.Type}', '{content.HtmlBody.SqlEncode()}' ); 
                SELECT SCOPE_IDENTITY();");
        }

        public static Content Select(int id)
        {
            var item = GetContent($@"SELECT [Id]
                        ,[Title]
                        ,[Body]
                        ,[UserId]
                        ,[Type]
                        ,[HtmlBody]
                        ,[Created]
                        FROM[toodledo].[dbo].[Content] WHERE [Id] = '{id}'")[0];

            item.User = UserApi.Select(item.UserId);

            var children = SelectByParent(item.Id);
            item.Children = children;
            return item;
        }

        private static int GetChildrenCount(int id)
        {
            return (int)Execute($"SELECT COUNT(*) FROM [toodledo].[dbo].[ContentRelation] WHERE ParentId = {id}");
        }

        private static List<Content> GetContent(string sql)
        {
            var results = new List<Content>();
            ExecuteReader(sql, (r) => results = Map(r));
            return results;
        }

        public static List<Content> SelectByParent(int parentId)
        {
            var result = GetContent($@"SELECT c.Id, 
                                             c.Title, 
                                             c.Body, 
                                             c.UserId, 
                                             c.Type,
                                             c.HtmlBody,
                                             c.Created
                                      FROM[toodledo].[dbo].[ContentRelation] r
                                      INNER JOIN[toodledo].[dbo].[Content] c
                                      ON r.ChildId = c.Id
                                      WHERE r.ParentId = {parentId}");

            Hydrate(result);
            return result;
        }

        private static string SearchQuery(int pageSize, int? pageNo, string type, string search, string orderBy)
        {
            var sql = $@"SELECT [Id]
                        ,[Title]
                        ,[Body]
                        ,[UserId]
                        ,[Type]
                        ,[HtmlBody]
                        ,[Created]
                        FROM[toodledo].[dbo].[Content] 
                        WHERE [Type] = '{type}'";

            if (search != null)
            {
                sql += $" AND [Body] LIKE '%{search}%' ";
            }

            orderBy = orderBy ?? "created";

            switch (orderBy)
            {
                case "created":
                    sql += " ORDER BY Id DESC";
                    break;
            }

            pageNo = pageNo ?? 1;
            sql += $" OFFSET {(pageNo - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            return sql;
        }

        public static int GetSearchResultCount(string type, string search)
        {
            var sql = $@"SELECT COUNT(*)
                        FROM[toodledo].[dbo].[Content] 
                        WHERE [Type] = '{type}'";

            if (search != null)
            {
                sql += $" AND [Body] LIKE '%{search}%' ";
            }
            return (int)Execute(sql);
        }

        private static void Hydrate(List<Content> content)
        {
            content.ForEach(i =>
            {
                i.User = UserApi.Select(i.UserId);
                i.ChildrenCount = GetChildrenCount(i.Id);
                i.Score = VoteApi.Select(i.Id);
            });
        }

        public static List<Content> Search(int pageSize, int? pageNo, string type, string search, string orderBy)
        {
            var sql = SearchQuery(pageSize, pageNo, type, search, orderBy);
            var result = GetContent(sql);
            Hydrate(result);
            return result;
        }

        public static void InsertAsChild(int parentId, Content content)
        {
            var id = Insert(content);
            Relate(parentId, id);
        }

        public static void Relate(int parentId, int childId)
        {
            Execute($@"INSERT INTO [dbo].[ContentRelation] ([ParentId], [ChildId]) 
                       VALUES ( '{parentId}', '{childId}' )");
        }

        public static void Update(int id, string title, string body, string htmlBody)
        {
            Execute($@"UPDATE [dbo].[Content] SET Title = '{title.SqlEncode()}', Body = '{body.SqlEncode()}', HtmlBody = '{htmlBody.SqlEncode()}' WHERE id = {id}");
        }

        //TODO: delete me
        public static void AddCreatedDate(int id, string createdDate)
        {
            Execute($@"UPDATE [dbo].[Content] SET Created = '{createdDate}' WHERE id = {id}");
        }

        public static void UpdateScore(int id, int score)
        {
            Execute($@"UPDATE [dbo].[Content] SET Score = {score} WHERE id = {id}");
        }

        public static List<Content> Map(SqlDataReader reader)
        {
            var results = new List<Content>();
            while (reader.Read())
            {
                var item = new Content()
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Body = reader.GetString(2),
                    UserId = reader.GetInt32(3),
                    Type = reader.GetString(4),
                    HtmlBody = reader.GetString(5),
                    Created = reader.GetDateTime(6)
                };
                results.Add(item);
            }
            return results;
        }
    }
}
