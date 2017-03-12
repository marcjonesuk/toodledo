using Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Website.Controllers
{
    public class BaseController : Controller
    {
        public User GetCurrentUser()
        {
            try
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var user = claim.Value;
                var currentUser = UserApi.GetByAspNetId(user);
                return currentUser;
            }
            catch
            {
                return null;
            }
        }
    }
}
