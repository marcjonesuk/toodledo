using Data;
using Data.Db;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.ModelManagers;
using Website.Models.ContentViewModels;
using Website.RequestObjects;

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
            return i.WithChildren().WithUser().WithTags().WithEditedBy();
        }

        public static ContentViewModel WithParent(this ContentViewModel c)
        {
            c.Parent = ContentApi.GetParent(c.Id).First().AsViewModel();
            return c;
        }

        public static ContentViewModel WithEditedBy(this ContentViewModel c)
        {
            var editedBy = ContentHistoryApi.SelectByContentId(c.Id).Select(ch => ch.ChangedByUserId).Distinct().ToList();
            if (!editedBy.Contains(c.UserId))
            {
                editedBy.Add(c.UserId);
            }
            c.EditedBy = editedBy.Select(u => UserApi.GetById(u)).ToList();
            return c;
        }

        public static ContentRequest AsRequest(this Content c)
        {
            var tags = TagManager.GetTagStringForContentAndAvailable(c.Id);
            ContentRequest request = new ContentRequest
            {
                ContentId = c.Id,
                Title = c.Title,
                Body = c.Body,
                Type = c.Type,
                AvailableTags = tags.Item2,
                Tags = tags.Item1
            };

            return request;
        }
    }
}
