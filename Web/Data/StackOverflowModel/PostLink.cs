using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Data.StackOverflowModel.PostLink
{
    public class postlinks
    {
        public List<row> rows { get; set; }
    }

    public class row
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public string CreationDate { get; set; }
        [XmlAttribute]
        public string PostId { get; set; }
        [XmlAttribute]
        public string RelatedPostId { get; set; }
        [XmlAttribute]
        public string LinkTypeId { get; set; }
    }
}
