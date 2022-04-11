using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Task4
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Ключ", "Значение");

            return View(data);
        }
    }
}