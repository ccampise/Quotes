using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quotes.Models;

namespace Quotes.Controllers
{
    public class QuotationController : Controller
    {
        private QuotationContext db = new QuotationContext();

        // GET: /Quotation/
        public ActionResult Index(string filter)
        {
            if(ViewBag.Unhide != null && ViewBag.Unhide)
            {
                for (int i = 0; i < Request.Cookies.Count; i++)
                {
                   var cookieName = Request.Cookies[i].Name;
                   HttpCookie deleteCookie = new HttpCookie(cookieName);
                    deleteCookie.Expires = DateTime.Now.AddHours(-1);
                    Response.Cookies.Add(deleteCookie);
                }
            }
            if (Request.Cookies.Count == 0)
            {
                ViewBag.ShowUnhideButton = false;
            }
            else
            {
                ViewBag.ShowUnhideButton = true;
            }
            ViewBag.ShowDisplayAllButton = false;
            List<int> CookieIds = new List<int>();
            var quotations = db.Quotations.Include(q => q.Category);

            for(int i = 0; i< Request.Cookies.Count; i++)
            {
                HttpCookie cookie = Request.Cookies[i];
                CookieIds.Add(int.Parse(cookie.Value));                
            }


            if (String.IsNullOrEmpty(filter))
            {
                ViewBag.ShowDisplayAllButton = false;
                quotations = quotations.Where(c => !CookieIds.Contains(c.QuotationID));                    
                return View(quotations.ToList());
            }
            else
            {
                ViewBag.ShowDisplayAllButton = true;
                quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Contains(filter) || q.Quote.Contains(filter) || q.Author.Contains(filter));
                return View(quotations.ToList());
            }
            
        }

        public ActionResult HideQuote(String Id)
        {
            
            HttpCookie cookie = new HttpCookie("hidden"+Id, Id);
            Response.Cookies.Add(cookie);            
            return RedirectToAction("Index");
        }

        public ActionResult Unhide()
        {
            ViewBag.Unhide = true; 
            return RedirectToAction("Index");
        }

        // GET: /Quotation/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // GET: /Quotation/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            return View();
        }

        public ActionResult AddCategory()
        {            
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCategory(Category category)
        {
            if(ModelState.IsValid)
            {
                var check = db.Categories.Where(c => c.Name == category.Name);
                if (check.Count() == 0)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    return RedirectToAction("Create");
                }
                else
                {
                    ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", category.CategoryID);
                    return View(category);
                }
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", category.CategoryID);
            return View(category);
        }

        // POST: /Quotation/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="QuotationID,CategoryID,Quote,Author,Date")] Quotation quotation)
        {
            quotation.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.Quotations.Add(quotation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

        // GET: /Quotation/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

         // POST: /Quotation/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="QuotationID,CategoryID,Quote,Author,Date")] Quotation quotation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(quotation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name", quotation.CategoryID);
            return View(quotation);
        }

        // GET: /Quotation/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Quotation quotation = db.Quotations.Find(id);
            if (quotation == null)
            {
                return HttpNotFound();
            }
            return View(quotation);
        }

        // POST: /Quotation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Quotation quotation = db.Quotations.Find(id);
            db.Quotations.Remove(quotation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
