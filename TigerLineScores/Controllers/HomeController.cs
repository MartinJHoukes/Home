using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TigerLineScores.Models;

namespace TigerLineScores.Controllers
{
    public class HomeController : Controller
    {
        private TigerLineScoresEntities1 db = new TigerLineScoresEntities1();

        // GET: Home
        public ActionResult Index(int? id)
        {
            return View();
        }

        public ActionResult Logout()
        {
            // Clear session variables
            Session["userid"] = null;
            Session["Admin"] = null;
            Session["UserName"] = null;

            return RedirectToAction("Index", "Home", 0);
        }

    }
}