using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Data
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class TagSuggestion
    {
        public TagSuggestion(Tag tag)
        {
            Id = tag.Id;
            Name = tag.Name;
            Accepted = false;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Accepted { get; set; }
    }

    public class TagApi : DbApi
    {
        public static bool Tag(int contentId, int tagId)
        {
            if (contentId == 0)
                throw new ArgumentException(nameof(contentId));

            if (tagId == 0)
                throw new ArgumentException(nameof(tagId));

            try
            {
                Execute($"INSERT INTO [dbo].[ContentTag] ([ContentId], [TagId]) VALUES ({contentId}, {tagId})");
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static bool UnTag(int contentId, int tagId)
        {
            if (contentId == 0)
                throw new ArgumentException(nameof(contentId));

            if (tagId == 0)
                throw new ArgumentException(nameof(tagId));

            try
            {
                Execute($"DELETE FROM [dbo].[ContentTag] WHERE ContentId = {contentId} AND TagId = {tagId}");
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public static List<Tag> SelectByContent(int contentId)
        {
            var results = new List<Tag>();
            ExecuteReader($"SELECT t.[Id], t.[Name] FROM [dbo].[ContentTag] ct INNER JOIN [Tag] t ON ct.TagId = t.Id WHERE ContentId = {contentId }",
            (r) => { results = Map(r); } );
            return results;
        }

        public static List<Tag> Select()
        {
            var results = new List<Tag>();
            ExecuteReader($"SELECT [Id], [Name], c.c FROM [dbo].[Tag] t INNER JOIN (SELECT TagId, COUNT(*) c FROM ContentTag GROUP BY TagId) c ON t.Id = c.TagId",
            (r) => { results = Map(r); });
            return results;
        }
        
        public static List<TagSuggestion> SelectSuggestions()
        {
            var results = Select();
            return results.Select(t => new TagSuggestion(t)).ToList();
        }

        public static List<Tag> Map(SqlDataReader reader)
        {
            var results = new List<Tag>();
            while (reader.Read())
            {
                var item = new Tag()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                };

                if (reader.FieldCount == 3)
                    item.Count = reader.GetInt32(2);

                results.Add(item);
            }
            return results;
        }
    }
}
