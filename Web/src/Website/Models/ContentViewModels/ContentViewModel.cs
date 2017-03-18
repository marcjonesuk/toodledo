using Data;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Models.ContentViewModels
{
    public class ContentViewModel : Content
    {
        public static ContentViewModel New(Content content)
        {
            return new ContentViewModel(content);
        }

        public ContentViewModel(Content content)
        {
            Id = content.Id;
            Type = content.Type;
            Created = content.Created;
            LastModified = content.LastModified;
            UserId = content.UserId;
            Body = content.Body;
            HtmlBody = content.HtmlBody;
            Title = content.Title;
            Score = content.Score;
        }

        public bool? AllowEdit { get; set; }
        public bool? AllowDelete { get; set; }
        public List<Tag> Tags { get; set; }
        public User User { get; set; }
        public List<ContentViewModel> Children { get; set; }
        public ContentViewModel Parent { get; set; }
        public int? ChildrenCount { get; set; }
        public int? ParentId { get; set; }
        public List<User> EditedBy { get; set; }
        public List<ContentHistory> History { get; set; }
    }
}