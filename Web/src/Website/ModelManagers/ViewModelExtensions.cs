using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.Models.ContentViewModels;

namespace Website
{ 
    public static class ViewModelExtensions
    {
        public static ContentViewModel AsViewModel(this Content c)
        {
            return ContentViewModel.New(c);
        }

        public static ContentViewModel WithUser(this ContentViewModel i)
        {
            i.User = UserApi.Select(i.UserId);
            return i;
        }

        public static ContentViewModel WithChildren(this ContentViewModel i)
        {
            var children = ContentApi.SelectByParent(i.Id);
            i.ChildrenCount = ContentApi.GetChildrenCount(i.Id);
            i.Children = children.Select(c => ContentViewModel.New(c).WithUser().WithTags()).ToList();
            return i;
        }

        public static ContentViewModel WithChildrenCount(this ContentViewModel i)
        {
            i.ChildrenCount = ContentApi.GetChildrenCount(i.Id);
            return i;
        }

        public static ContentViewModel WithTags(this ContentViewModel i)
        {
            i.Tags = TagApi.SelectByContent(i.Id);
            return i;
        }

        public static ContentViewModel WithAll(this ContentViewModel i)
        {
            return i.WithChildren().WithUser().WithTags();
        }

        public static ContentViewModel WithParent(this ContentViewModel c)
        {
            c.Parent = ContentApi.GetParent(c.Id).First().AsViewModel();
            return c;
        }
    }
}
