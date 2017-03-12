﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.RequestObjects
{
    public class SearchRequest
    {
        public int? PageSize { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int? Page { get; set; }
        public string OrderBy { get; set; }
    }

    public class ContentRequest
    {
        public int? ContentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public int? ParentId { get; set; }
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