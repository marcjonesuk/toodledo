using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Website.Models.ContentViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Website.Models;
using Microsoft.AspNetCore.Authorization;
using Web.Controllers;

namespace Website.Controllers
{
    public class SearchRequest
    {
        public int? PageSize { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        public int? Page { get; set; }
        public string OrderBy { get; set; }
    }

    public static class ContentViewModelExtentions
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

    public class ContentRequest
    {
        public int? ContentId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Type { get; set; }
        public int? ParentId { get; set; }
    }

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

        public ContentViewModel Update(ContentRequest request)
        {
            if (request.ContentId == null)
                throw new ArgumentNullException(nameof(request.ContentId));

            var htmlBody = Markdown.Encode(request.Body);
            ContentApi.Update(request.ContentId.Value, request.Title, request.Body, htmlBody);

            return ContentApi.Select(request.ContentId.Value)
                             .AsViewModel()
                             .WithChildren()
                             .WithChildrenCount()
                             .WithTags();
        }

        public ContentViewModel Create(ContentRequest request)
        {
            var content = new Content();
            content.Title = request.Title;
            content.Body = request.Body;
            content.HtmlBody = Markdown.Encode(content.Body);
            content.UserId = user.Id;
            content.Type = request.Type;
            var id = ContentApi.Insert(content);

            if (request.ParentId != null)
            {
                ContentApi.Relate(request.ParentId.Value, id);
            }

            return ContentApi.Select(id).AsViewModel()
                .WithChildren()
                .WithChildrenCount()
                .WithTags();
        }

        public List<ContentViewModel> Search(SearchRequest request)
        {
            request.PageSize = request.PageSize ?? 10;
            request.Page = request.Page ?? 1;

            var contents = ContentApi.Search(10, request.Page, request.Type, request.Text, request.OrderBy, null);
            var searchResult = contents.Select(c => c.AsViewModel()
                    .WithTags()
                    .WithChildrenCount()
                    .WithUser()).ToList();

            return searchResult;
        }
    }

    public class ContentController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ContentController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        // GET: Content/5
        public ContentViewModel Index(int id)
        {
            var item = ContentApi.Select(id);
            return ContentViewModel.New(item).WithAll();
        }



        // GET: Content/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Content/Create
        [Authorize]
        public Content Create([FromBody]CreateContentRequest request)
        {

        }

        // GET: Content/Create
        [Authorize]
        public Content CreateChild([FromBody]CreateContentRequest request)
        {

        }



        // GET: Content/Update
        [Authorize]
        public Content Update([FromBody]CreateContentRequest request)
        {
            try
            {
                return UpdateInternal(request);
            }
            catch
            {
                throw;
            }
        }

        // POST: Content/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Content/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Content/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Content/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Content/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}