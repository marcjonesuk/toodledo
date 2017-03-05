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
        public posts GetPosts()
        {
            var filePath = @"C:\Users\Maisie\Source\Repos\toodledo\Web\src\Data\beer.stackexchange.com\Posts.xml";
            var fs = new FileStream(filePath, FileMode.Open);
            var reader = XmlReader.Create(fs);
            var xml = new XmlSerializer(typeof(posts)).Deserialize(reader);
            return (posts)xml;
        }

        public IEnumerable<Question> PostToQuestion(List<row> posts)
        {
            foreach (var post in posts)
            {
                yield return new Question()
                {
                    Created = DateTime.ParseExact(post.CreationDate, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture),
                    Id = int.Parse(post.Id),
                    Username = post.OwnerUserId,
                    Title = post.Title,
                    Body = post.Body
                };
            }
        }
    }

    public class posts
    {
        public List<row> rows { get; set; }
    }

    public class row
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string PostTypeId { get; set; }
        [XmlAttribute]
        public string AcceptedAnswerId { get; set; }
        [XmlAttribute]
        public string CreationDate { get; set; }
        [XmlAttribute]
        public string Score { get; set; }
        [XmlAttribute]
        public string ViewCount { get; set; }
        [XmlAttribute]
        public string Body { get; set; }
        [XmlAttribute]
        public string OwnerUserId { get; set; }
        [XmlAttribute]
        public string LastEditorUserId { get; set; }
        [XmlAttribute]
        public string LastEditDate { get; set; }
        [XmlAttribute]
        public string LastActivityDate { get; set; }
        [XmlAttribute]
        public string Title { get; set; }
        [XmlAttribute]
        public string Tags { get; set; }
        [XmlAttribute]
        public string AnswerCount { get; set; }
        [XmlAttribute]
        public string CommentCount { get; set; }
    }
}
