using Data.StackOverflowModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Data
{
    public class StackOverflowData
    {
        public Data.StackOverflowModel.Posts.posts GetPosts()
        {
            var posts = GetObjectFromXml<Data.StackOverflowModel.Posts.posts>("Posts");
            foreach (var item in posts.rows)
            {
                item.Body = item.Body.Replace("<p>", "");
                item.Body = item.Body.Replace("</p>", "");
            }
            return posts;
        }

        public Dictionary<int, User> GetUsers()
        {
            var usersFromXml = GetObjectFromXml<Data.StackOverflowModel.Users.users>("Users");

            Dictionary<int, User> users = new Dictionary<int, User>();
            foreach (var user in usersFromXml.rows)
            {
                var userid = int.Parse(user.Id);
                users.Add(userid, new User
                {
                    UserId = userid,
                    DisplayName = user.DisplayName,
                    ProfileImageUrl = user.ProfileImageUrl
                });
            }

            return users;
        }

        public IEnumerable<Question> PostToQuestion(List<Data.StackOverflowModel.Posts.row> posts, Dictionary<int, User> users)
        {
            foreach (var post in posts)
            {
                int userid;
                User user = null;
                if (int.TryParse(post.OwnerUserId, out userid))
                {
                    users.TryGetValue(userid, out user);
                }

                int length = Math.Min(50, post.Body.Length);
                var title = string.IsNullOrWhiteSpace(post.Title) ? post.Body.Substring(0, length) : post.Title;

                yield return new Question()
                {
                    Created = DateTime.ParseExact(post.CreationDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                    Id = int.Parse(post.Id),
                    User = user,
                    Title = title,
                    Body = post.Body
                };
            }
        }

        static string filePathTemplate = @"wwwroot\data\{0}.xml";
        public T GetObjectFromXml<T>(string fileName)
        {
            var filePath = string.Format(filePathTemplate, fileName);
            var fs = new FileStream(filePath, FileMode.Open);
            var reader = XmlReader.Create(fs);
            var xml = new XmlSerializer(typeof(T)).Deserialize(reader);
            return (T)xml;
        }
    }
}
