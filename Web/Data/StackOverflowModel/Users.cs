using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Data.StackOverflowModel.Users
{
    public class users
    {
        public List<row> rows { get; set; }
    }

    public class row
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string Reputation { get; set; }
        [XmlAttribute]
        public string CreationDate { get; set; }
        [XmlAttribute]
        public string DisplayName { get; set; }
        [XmlAttribute]
        public string LastAccessDate { get; set; }
        [XmlAttribute]
        public string Views { get; set; }
        [XmlAttribute]
        public string UpVotes { get; set; }
        [XmlAttribute]
        public string DownVotes { get; set; }
        [XmlAttribute]
        public string ProfileImageUrl { get; set; }
        [XmlAttribute]
        public string WebsiteUrl { get; set; }
        [XmlAttribute]
        public string Location { get; set; }
        [XmlAttribute]
        public string AboutMe { get; set; }
        [XmlAttribute]
        public string AccountId { get; set; }
    }
}
