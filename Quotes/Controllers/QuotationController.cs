using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Quotes.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Quotes.Controllers
{
    public class QuotationController : Controller
    {
        private QuotationContext db;
        private UserManager<ApplicationUser> userManager;

        public QuotationController()
        {
        db = new QuotationContext();
        userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

        }
        // GET: /Quotation/
        public ActionResult Index(string filter)
        {
            //checks if user is logged in 
            if(User.Identity.IsAuthenticated)
                  ViewBag.loggedIn = true;            
            else
                  ViewBag.loggedIn = false;

        //check if the user is an admin
            if (User.IsInRole("Admin"))
            {
                ViewBag.admin = true;
                ViewBag.UserName = new SelectList(db.Users, "UserName", "UserName");
            }
            else
            {
                ViewBag.admin = false;
                ViewBag.UserName = null;
            }

            //if the hidden cookie exists then show the unhide switch
            if (Request.Cookies.AllKeys.Contains("hidden"))
                ViewBag.ShowUnhideButton = true;            
            else
                ViewBag.ShowUnhideButton = false;      

            //initialize this so there's no null errors
            ViewBag.ShowDisplayAllButton = false;

            //Collect the hidden ids from the cookie            
            List<int> hiddenIds = new List<int>();
            if (Request.Cookies.AllKeys.Contains("hidden"))
            {
                string[] CookieIds = Request.Cookies["hidden"].Value.ToString().Split(' ');               
                //change the strings into a list of ints
                foreach (string id in CookieIds)
                {
                    int temp = int.Parse(id);
                    hiddenIds.Add(temp);
                }
            }
            var quotations = db.Quotations.Include(q => q.Category);
            //search logic that checks all quotes for the indicated filter, and doesnt show hidden quotes
            if (String.IsNullOrEmpty(filter))
            {
                ViewBag.ShowDisplayAllButton = false;
                quotations = quotations.Where(c => !hiddenIds.Contains(c.QuotationID));                    
                return View(quotations.ToList());
            }
            else
            {
                ViewBag.ShowDisplayAllButton = true;
                quotations = quotations.Where(c => !hiddenIds.Contains(c.QuotationID));  
                quotations = db.Quotations.Include(q => q.Category).Where(q => q.Category.Name.Contains(filter) || q.Quote.Contains(filter) || q.Author.Contains(filter));
                return View(quotations.ToList());
            }
            
        }


        public ActionResult HideQuote(String Id)
        {
            //if there is not already a cookie create it and add the current id
            if(!Request.Cookies.AllKeys.Contains("hidden"))
            {
                HttpCookie cookie = new HttpCookie("hidden", " " + Id);
                Response.Cookies.Add(cookie);
            }
            //if the cookie is already there add the new id to it
            else
            {
                HttpCookie cookie = Request.Cookies["hidden"];
                cookie.Value += " " + Id;
                Response.Cookies.Add(cookie);
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult Unhide()
        {
            HttpCookie deleteCookie = new HttpCookie("hidden");
            deleteCookie.Expires = DateTime.Now.AddHours(-1);
            Response.Cookies.Add(deleteCookie);
            return RedirectToAction("Index");
        }

        public ActionResult userQuotes(string UserName)
        {

            ViewBag.userList = db.Users;
            if (String.IsNullOrEmpty(UserName))
            {
                var userid = User.Identity.GetUserId();
                var quotations = db.Quotations.Include(q => q.Category);
                quotations = quotations.Where(q => q.User.Id == userid);
                return View(quotations);
            }

            else
            {
                if (!User.IsInRole("Admin"))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                else
                {
                    var theUser = userManager.FindByName(UserName);
                    var quotations = db.Quotations.Include(q => q.Category);
                    quotations = quotations.Where(q => q.User.Id == theUser.Id);
                    return View(quotations);
                }
            }
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
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "Name");
            return View();
        }

        public ActionResult AddCategory()
        {            
            return View(); 
        }

        [Authorize]
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
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="QuotationID,CategoryID,Quote,Author,Date")] Quotation quotation)
        {
           var user = userManager.FindById(User.Identity.GetUserId());
           quotation.Date = DateTime.Now;
           quotation.User = user;
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
