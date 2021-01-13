using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BookStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categories
        public ActionResult Index()
        {
            var categories = db.Categories.Include("Books").OrderBy(category => category.CategoryName);
            // din categories iau si articolele si le sortez dupa category name folosind acea lambda expresie

            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            ViewBag.Categories = categories;
            return View();
        }
        public ActionResult Show(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                ViewBag.Category = category;
                ViewBag.Books = category.Books;

                return View(category);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }
        }
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Category category)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Categories.Add(category);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost adaugata!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(category);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(int id)
        {
            Category category = db.Categories.Find(id);
            ViewBag.Category = category;
            return View();
            //return View(category);
        }


        [HttpPut]
        public ActionResult Edit(int id, Category requestCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Category category = db.Categories.Find(id);
                    if (TryUpdateModel(category))
                    {
                        category.CategoryName = requestCategory.CategoryName;
                        db.SaveChanges();
                        TempData["message"] = "Categoria a fost modificata!";
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }
        }

        [HttpDelete]
        public ActionResult Delete(int id)
        {
            try
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                TempData["message"] = "Categoria a fost stearsa!";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }
        }

    }
}