using Quotes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Quotes.Controllers
{
    [Authorize(Roles="Admin")]
    public class AdminController : Controller
    {
        private QuotationContext db = new QuotationContext();
        //
        // GET: /Admin/
        public ActionResult adminIndex()
        {
            var quotations = db.Quotations.ToList();
            return View(quotations);
        }


        public ActionResult userSearch()
        {
            return View(); 
        }
	}
	
}
 
