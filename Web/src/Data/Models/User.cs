using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data
{
    public class User
    {
        public User()
        {
            DisplayName = "Unknown";
            ProfileImageUrl = "http://www.batchcoloring.com/wp-content/uploads/2015/02/Casper-the-Friendly-Ghost-Flying-Coloring-Pages-300x300.jpg";
        }

        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
