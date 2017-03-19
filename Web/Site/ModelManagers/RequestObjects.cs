using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using data = Data;

namespace Website.RequestObjects
{
    public class SearchRequest
    {
        public int? PageSize { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int? Page { get; set; }
        public string OrderBy { get; set; }
        public int? TagId { get; set; }
    }

    public class ContentRequest
    {
        public int? UserId { get; set; }
        public int? ContentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public int? ParentId { get; set; }
        public List<data.TagSuggestion> AvailableTags { get; set; }
        public string Tags { get; set; }
    }

    public class VoteRequest
    {
        public int ContentId { get; set; }
        public int Direction { get; set; }
    }

    public class DeleteRequest
    {
        public int ContentId { get; set; }
    }
}
