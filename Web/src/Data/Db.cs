using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class Update
    {
        public string Field { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
    }

    public class ChangeSet
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public DateTime Committed { get; set; }
        public Update[] Updates { get; set; }
        public string UserId { get; set; }
    }

    public class Content
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        public string Type { get; set; }

        public DateTime Created { get; internal set; }
        public DateTime LastModified { get; internal set; }

        public string UserId { get; internal set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public int Score { get; set; }
        public string[] ChangeSets { get; set; }
        public string[] Parents { get; set; }
        public string[] Children { get; set; }
        public string[] Tags { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class Db
    {
        public static string Uri { get; set; }
        public static string Key { get; set; }
        private static DocumentClient client { get; set; }

        static Db()
        {
          
        }


        // ADD THIS PART TO YOUR CODE
        private async Task GetStartedDemo()
        {





        }
    }
}
