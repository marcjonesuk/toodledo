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
                INSERT INTO [dbo].[Content] ([Title], [Body], [UserId], [Type], [HtmlBody], [Created]) 
                VALUES ( '{content.Title.SqlEncode()}', '{content.Body.SqlEncode()}', {content.UserId}, '{content.Type}', '{content.HtmlBody.SqlEncode()}', '{content.Created}'); 
                SELECT SCOPE_IDENTITY();");
        }

        public static Content Select(int id)
        {
            var items = GetContent($@"SELECT [Id]
                        ,[Title]
                        ,[Body]
                        ,[UserId]
                        ,[Type]
                        ,[HtmlBody]
                        ,[Created]
                        ,[Score]
                        FROM[toodledo].[dbo].[Content] WHERE [Id] = '{id}'");

            var item = items[0];
            //var children = SelectByParent(item.Id).OrderByDescending(c => c.Score).ToList();
            //item.Children = children;
            return item;
        }

        public static int GetChildrenCount(int id)
        {
            return (int)Execute($"SELECT COUNT(*) FROM [toodledo].[dbo].[ContentRelation] WHERE ParentId = {id}");
        }

        private static List<Content> GetContent(string sql)
        {
            var results = new List<Content>();
            ExecuteReader(sql, (r) => results = Map(r));
            return results;
        }

        public static List<Content> GetParent(int childId)
        {
            var result = GetContent($@"SELECT c.Id, 
                                             c.Title, 
                                             c.Body, 
                                             c.UserId, 
                                             c.Type,
                                             c.HtmlBody,
                                             c.Created,
                                             c.Score
                                      FROM[toodledo].[dbo].[ContentRelation] r
                                      INNER JOIN[toodledo].[dbo].[Content] c
                                      ON r.ParentId = c.Id
                                      WHERE r.ChildId = {childId}");

            return result;
        }

        public static List<Content> SelectByParent(int parentId)
        {
            var result = GetContent($@"SELECT c.Id, 
                                             c.Title, 
                                             c.Body, 
                                             c.UserId, 
                                             c.Type,
                                             c.HtmlBody,
                                             c.Created,
                                             c.Score
                                      FROM[toodledo].[dbo].[ContentRelation] r
                                      INNER JOIN[toodledo].[dbo].[Content] c
                                      ON r.ChildId = c.Id
                                      WHERE r.ParentId = {parentId}");

            return result;
        }

        private static string SearchQuery(int pageSize, int? pageNo, string type, string search, string orderBy, int? tagId)
        {
            var sql = $@"SELECT [Id]
                        ,[Title]
                        ,[Body]
                        ,[UserId]
                        ,[Type]
                        ,[HtmlBody]
                        ,[Created]
                        ,[Score]
                        FROM[toodledo].[dbo].[Content] c";

            if (tagId != null)
            {
                sql += $" INNER JOIN [dbo].[ContentTag] ct ON c.[Id] = ct.[ContentId] AND ct.TagId = {tagId}";
            }

            if (type != null)
                sql += $" WHERE[Type] = '{type}'";

            if (search != null)
                sql += $" AND [Body] LIKE '%{search}%' ";

            orderBy = orderBy ?? "created-desc";

            switch (orderBy)
            {
                case "created-desc":
                    sql += " ORDER BY Created DESC";
                    break;

                case "score-desc":
                    sql += " ORDER BY Score DESC";
                    break;

                default:
                    sql += " ORDER BY Id DESC";
                    break;
            }

            pageNo = pageNo ?? 1;
            sql += $" OFFSET {(pageNo - 1) * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";

            return sql;
        }

        public static int GetSearchResultCount(string type, string search, int? tagId)
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

      

        public static List<Content> Search(int pageSize, int? pageNo, string type, string search, string orderBy, int? tagId)
        {
            var sql = SearchQuery(pageSize, pageNo, type, search, orderBy, tagId);
            var result = GetContent(sql);
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
                    Created = reader.GetDateTime(6),
                    Score = reader.GetInt32(7)
                };
                results.Add(item);
            }
            return results;
        }
    }
}
