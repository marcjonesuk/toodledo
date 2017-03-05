using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Data.StackOverflowModel.Posts
{
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
