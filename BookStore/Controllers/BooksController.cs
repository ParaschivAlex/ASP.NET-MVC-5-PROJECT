using BookStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace BookStore.Controllers
{
	
	public class BooksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Books

		public ActionResult Index()
        {
            int x = 1;
            if(User.IsInRole("Admin"))
            {
                x = 0;
            }
			var books = db.Books.Include("Category").Where(a => a.B_status >= x);
            ViewBag.Books = books;
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];//.ToString();
            }


			var search = "";


			if (Request.Params.Get("search") != null)
			{
				search = Request.Params.Get("search").Trim();
				List<int> bookIds = db.Books.Where(bk => bk.Title.Contains(search) || bk.Author.Contains(search)).Select(b => b.BookId).ToList();

				List<int> reviewIds = db.Reviews.Where(rev => rev.Comment.Contains(search)).Select(rev => rev.BookId).ToList();
				List<int> mergedIds = bookIds.Union(reviewIds).ToList();
                if(Request.Params.Get("pret") != null)
                {
                    if(Request.Params.Get("pret") == "crescator")
                    {
                        books = db.Books.Where(book => mergedIds.Contains(book.BookId)).Include("Category").Include("User").OrderBy(a => a.Price);
                    }
                    else
                    {
                        books = db.Books.Where(book => mergedIds.Contains(book.BookId)).Include("Category").Include("User").OrderBy(a => -a.Price);
                    }
                }
				else
                {
                    books = db.Books.Where(book => mergedIds.Contains(book.BookId)).Include("Category").Include("User").OrderBy(a => a.Title);
                }
                ViewBag.Books = books;
			}
			ViewBag.SearchString = search;

			/*var totalItems = books.Count();
			var currentPage = Convert.ToInt32(Request.Params.Get("page"));
			var offset = 0;
			if (!currentPage.Equals(0))
			{ offset = (currentPage - 1) * this._perPage; }
			var paginatedBooks = books.Skip(offset).Take(this._perPage);

			if(TempData.ContainsKey("message"))
			{ ViewBag.message = TempData["message"].ToString(); }

			ViewBag.total = totalItems;
			ViewBag.lastPage = Math.Ceiling((float)totalItems / (float)this._perPage);
			ViewBag.Books = paginatedBooks;
			ViewBag.SearchString = search;
			*/


			return View();
        }
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Accept(int id)
        {
            try
            {
               
                
                   Book book = db.Books.Find(id);

                    if (TryUpdateModel(book))
                    {
                    //book = requestBook
                    book.B_status = 1;
                        db.SaveChanges();
                        TempData["message"] = "Cartea a fost modificata!";
                        return RedirectToAction("Index");
                    }
                else
                {
                    TempData["message"] = "smth wrong";
                    return RedirectToAction("Index");
                }
                
           
            }
            catch (Exception e)
            {
                TempData["message"] = "smth went wrong";
                Console.WriteLine(e.ToString());
                return RedirectToAction("Index");
            }

        }
        public ActionResult Show(int id)
        {
            Book book = db.Books.Find(id);
            ViewBag.Book = book;
            ViewBag.Category = book.Category;
            ViewBag.Reviews = book.Reviews;
			ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(book);
        }
        [Authorize(Roles = "Admin,Colaborator")]
        public ActionResult New()
        {
            //var categories = from cat in db.Categories select cat;
            //ViewBag.Categories = categories;
            //Book book = new Book();
            //book.Categ = GetAllCategories();
            Book book = new Book
            {
                Categ = GetAllCategories()
            };
            return View(book);
        }
        [Authorize(Roles = "Admin,Colaborator")]
        [HttpPost]
        public ActionResult New(Book book)
        {
			book.UserId = User.Identity.GetUserId();
			try
            {
                if (ModelState.IsValid)
                {

					if (User.IsInRole("Admin"))
					{
						book.B_status = 1;
						TempData["message"] = "Cartea a fost adaugata!";
					}
					else
					{
						book.B_status = 0;//0 neverificat   1 acceptat
						TempData["message"] = "Cartea urmeaza sa fie verificata.";
					}
					db.Books.Add(book);
                    db.SaveChanges();
					return RedirectToAction("Index");
                }
                else
                {
                    TempData["message"] = "Invalid fields";
                    return Redirect("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return View(book);
            }
        }
        [Authorize(Roles = "Admin,Colaborator")]
        public ActionResult Edit(int id)
        {
            Book book = db.Books.Find(id);
            ViewBag.Book = book;
            ViewBag.Category = book.Category;
			ViewBag.utilizatorCurent = User.Identity.GetUserId();
			book.Categ = GetAllCategories();
            return View(book);
        }
        [Authorize(Roles = "Admin,Colaborator")]
        [HttpPut]
        public ActionResult Edit(int id, Book requestBook)
        {
			
			try
            {
                if (ModelState.IsValid)
                {
                    Book book = db.Books.Find(id);

                    if (TryUpdateModel(book))
                    {
                        //book = requestBook
                        book.Title = requestBook.Title;
                        book.Author = requestBook.Author;
                        book.Price = requestBook.Price;
                        book.CategoryId = requestBook.CategoryId;
                        db.SaveChanges();
                        TempData["message"] = "Cartea a fost modificata!";
                        return RedirectToAction("Index");
                    }
                    return View(requestBook);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return View(requestBook);
            }

        }
        [Authorize(Roles = "Admin,Colaborator")]
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            Book book = db.Books.Find(id);
            db.Books.Remove(book);
            db.SaveChanges();
            TempData["message"] = "Cartea a fost stearsa!";
            return RedirectToAction("Index");
        }
        [HttpPut]
        public ActionResult AddToCart(int id)
        {
            Book book = db.Books.Find(id);
            if(User.IsInRole("User"))
            {
                TempData["message"] = "Produs adaugat cu succes in cos";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Trebuie sa va conectati pentru a adauga in cos";
                return Redirect("/account/login");
            }
        }
        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            // generam o lista goala
            var selectList = new List<SelectListItem>();
            // Extragem toate categoriile din baza de date
            var categories = from cat in db.Categories select cat;
            // iteram prin categorii
            foreach (var category in categories)
            {
                // Adaugam in lista elementele necesare pentru dropdown
                selectList.Add(new SelectListItem
                {
                    Value = category.CategoryId.ToString(),
                    Text = category.CategoryName.ToString()
                });
            }
            // returnam lista de categorii
            return selectList;
        }
    }
}