using Data;
using LuceneSearch;
using Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Website.ModelManagers;
using Website.Models.ContentViewModels;
using Website.RequestObjects;

namespace Website
{
    public class ContentManager
    {
        readonly User user;

        public ContentManager()
        {
        }

        public ContentManager(User user)
        {
            this.user = user;
        }

        private void Authorized()
        {
            if (user == null)
                throw new UnauthorizedAccessException("No User set");
        }

        public ContentViewModel Get(int contentId)
        {
            return ContentApi.Select(contentId)
                .AsViewModel()
                .WithAll();
        }

        public ContentViewModel Update(ContentRequest request)
        {
            Authorized();

            if (request.ContentId == null)
                throw new ArgumentNullException(nameof(request.ContentId));

            var userId = user?.Id;
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(request), "User must be set to update an answer");
            }

            TagManager.SetTagsForContent((int)request.ContentId, request.Tags);

            var htmlBody = Markdown.Encode(request.Body);
            ContentApi.Update(request.ContentId.Value, request.Title, request.Body, htmlBody, (int)userId);

            var item = ContentApi.Select(request.ContentId.Value)
                .AsViewModel()
                .WithAll();

            try
            {
                Searcher.Instance.Index(new Searchable() { Id = item.Id, Type = item.Type, Title = item.Title, Body = item.Body, Username = item.User.DisplayName });
                ContentApi.MarkAsIndexed(item.Id);
            }
            catch
            {
            }

            return item;
        }

        public ContentViewModel Create(ContentRequest request)
        {
            Authorized();

            var content = new Content();
            content.Title = request.Title;
            content.Body = request.Body;
            content.HtmlBody = Markdown.Encode(content.Body);
            content.UserId = user.Id;
            content.Type = request.Type;
            var id = ContentApi.Insert(content);

            TagManager.SetTagsForContent(id, request.Tags);

            if (request.ParentId != null)
            {
                ContentApi.Relate(request.ParentId.Value, id);
            }

            var item = ContentApi.Select(id).AsViewModel()
                .WithChildren()
                .WithChildrenCount()
                .WithTags();

            try
            {
                Searcher.Instance.Index(new Searchable() { Id = id, Type = item.Type, Title = item.Title, Body = item.Body, Username = item.User.DisplayName });
                ContentApi.MarkAsIndexed(id);
            }
            catch
            {
            }

            return item;
        }

        public List<ContentViewModel> Search(SearchRequest request)
        {
            request.PageSize = request.PageSize ?? 10;
            request.Page = request.Page ?? 1;

            var contents = ContentApi.Search(request.PageSize.Value, request.Page, request.Type, request.Text, request.OrderBy, request.TagId);
            var searchResult = contents.Select(c => c.AsViewModel()
                    .WithTags()
                    .WithChildrenCount()
                    .WithUser()
                    .WithEditedBy()).ToList();

            return searchResult;
        }
    }
}
