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
   
    

  


    //public class ContentController : Controller
    //{
    //    private readonly UserManager<ApplicationUser> _userManager;

    //    public ContentController(UserManager<ApplicationUser> userManager)
    //    {
    //        this._userManager = userManager;
    //    }

    //    // GET: Content/5
    //    public ContentViewModel Index(int id)
    //    {
    //        var item = ContentApi.Select(id);
    //        return ContentViewModel.New(item).WithAll();
    //    }



    //    // GET: Content/Details/5
    //    public ActionResult Details(int id)
    //    {
    //        return View();
    //    }

    //    // GET: Content/Create
    //    [Authorize]
    //    public Content Create([FromBody]CreateContentRequest request)
    //    {

    //    }

    //    // GET: Content/Create
    //    [Authorize]
    //    public Content CreateChild([FromBody]CreateContentRequest request)
    //    {

    //    }



    //    // GET: Content/Update
    //    [Authorize]
    //    public Content Update([FromBody]CreateContentRequest request)
    //    {
    //        try
    //        {
    //            return UpdateInternal(request);
    //        }
    //        catch
    //        {
    //            throw;
    //        }
    //    }

    //    // POST: Content/Create
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Create(IFormCollection collection)
    //    {
    //        try
    //        {
    //            // TODO: Add insert logic here

    //            return RedirectToAction("Index");
    //        }
    //        catch
    //        {
    //            return View();
    //        }
    //    }

    //    // GET: Content/Edit/5
    //    public ActionResult Edit(int id)
    //    {
    //        return View();
    //    }

    //    // POST: Content/Edit/5
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Edit(int id, IFormCollection collection)
    //    {
    //        try
    //        {
    //            // TODO: Add update logic here

    //            return RedirectToAction("Index");
    //        }
    //        catch
    //        {
    //            return View();
    //        }
    //    }

    //    // GET: Content/Delete/5
    //    public ActionResult Delete(int id)
    //    {
    //        return View();
    //    }

    //    // POST: Content/Delete/5
    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public ActionResult Delete(int id, IFormCollection collection)
    //    {
    //        try
    //        {
    //            // TODO: Add delete logic here

    //            return RedirectToAction("Index");
    //        }
    //        catch
    //        {
    //            return View();
    //        }
    //    }
    //}
}