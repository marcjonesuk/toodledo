//using Data.StackOverflowModel;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Xml;
//using System.Xml.Serialization;

//namespace Data
//{
//    public class StackOverflowData
//    {
//        Dictionary<int, User> _users;

//        public StackOverflowModel.Posts.posts GetPosts()
//        {
//            var posts = GetObjectFromXml<Data.StackOverflowModel.Posts.posts>("Posts");
//            foreach (var item in posts.rows)
//            {
//                item.Body = item.Body.Replace("<p>", "");
//                item.Body = item.Body.Replace("</p>", "");
//                item.Body = item.Body.Replace("<h2>", "");
//                item.Body = item.Body.Replace("</h2>", "");
//            }
//            return posts;
//        }

//        public Dictionary<int, User> GetUsers()
//        {
//            var usersFromXml = GetObjectFromXml<StackOverflowModel.Users.users>("Users");

//            Dictionary<int, User> users = new Dictionary<int, User>();
//            foreach (var user in usersFromXml.rows)
//            {
//                var userid = int.Parse(user.Id);
//                users.Add(userid, new User
//                {
//                    UserId = userid,
//                    DisplayName = user.DisplayName,
//                    ProfileImageUrl = user.ProfileImageUrl
//                });
//            }
//            _users = users;
//            return users;
//        }
        
//        private User GetUser(string userid)
//        {
//            int id;
//            User user = new User();
//            if (int.TryParse(userid, out id))
//            {
//                _users.TryGetValue(id, out user);
//            }
//            return user;
//        }

//        public IEnumerable<Question> PostToQuestion(List<StackOverflowModel.Posts.row> posts, Dictionary<int, User> users)
//        {
//            var questions = new List<StackOverflowModel.Posts.row>();
//            var answers = new List<StackOverflowModel.Posts.row>();

//            foreach (var post in posts)
//            {
//                if (post.PostTypeId == "1")
//                {
//                    questions.Add(post);
//                }
//                else
//                {
//                    answers.Add(post);
//                }
//            }

//            foreach (var question in questions)
//            {
//                int userid;
//                User user = new User();
//                if (int.TryParse(question.OwnerUserId, out userid))
//                {
//                    users.TryGetValue(userid, out user);
//                }

//                var qans = answers.Where(a => a.ParentId == question.Id);

//                int acceptedAnswer;
//                int.TryParse(question.AcceptedAnswerId, out acceptedAnswer);

//                yield return new Question()
//                {
//                    Created = DateTime.ParseExact(question.CreationDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
//                    Id = int.Parse(question.Id),
//                    User = user,
//                    Title = question.Title,
//                    Body = question.Body,
//                    Answers = PostToAnswer(qans, users).ToList(),
//                    AcceptedAnswerId = acceptedAnswer
//                };
//            }
//        }
//        public IEnumerable<Answer> PostToAnswer(IEnumerable<StackOverflowModel.Posts.row> answers, Dictionary<int, User> users)
//        {
//            foreach (var answer in answers)
//            {
//                int userid;
//                User user = null;
//                if (int.TryParse(answer.OwnerUserId, out userid))
//                {
//                    users.TryGetValue(userid, out user);
//                }

//                yield return new Answer()
//                {
//                    Created = DateTime.ParseExact(answer.CreationDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
//                    Id = int.Parse(answer.Id),
//                    User = user,
//                    Body = answer.Body,
//                    Score = int.Parse(answer.Score)
//                };
//            }
//        }

//        static string filePathTemplate = @"data\{0}.xml";
//        public T GetObjectFromXml<T>(string fileName)
//        {
//            var filePath = string.Format(filePathTemplate, fileName);
//            var fs = new FileStream(filePath, FileMode.Open);
//            var reader = XmlReader.Create(fs);
//            var xml = new XmlSerializer(typeof(T)).Deserialize(reader);
//            return (T)xml;
//        }
//    }
//}
