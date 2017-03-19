using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class ContentHistory
    {
        public int? Id { get; set; }
        public int ContentId { get; set; }
        public string ChangedField { get; set; }
        public string OldValue { get; set; }
        public DateTime Changed { get; set; }
        public int ChangedByUserId { get; set; }
    }
}
