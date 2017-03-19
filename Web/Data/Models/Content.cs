using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Data
{
    public class Content
    {
        public Content()
        {
            Created = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string Type { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public int UserId { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
