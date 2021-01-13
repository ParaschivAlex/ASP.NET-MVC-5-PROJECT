using System;
using BookStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BookStore.Controllers
{
	
	public class ReviewsController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();
		[Authorize(Roles = "Admin")]
		// GET: Reviews
		public ActionResult Index()
		{
			var reviews = from review in db.Reviews.Include("Book")
						  orderby review.ReviewId
						  select review;
			if (TempData.ContainsKey("message"))
			{
				ViewBag.message = TempData["message"].ToString();
			}
			ViewBag.Reviews = reviews;

			return View();
		}
		[Authorize(Roles = "Admin")]
		public ActionResult Show(int id)
		{
			Review review = db.Reviews.Find(id);
			ViewBag.Review = review;
			return View();
		}
		[Authorize(Roles = "Admin,Colaborator,User")]
		[HttpDelete]
		public ActionResult Delete(int id)
		{

			Review rev = db.Reviews.Find(id);
			if (rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
			{
				db.Reviews.Remove(rev);
				TempData["message"] = "Comentariul a fost sters!";
				db.SaveChanges();
				return Redirect("/Books/Show/" + rev.BookId);
			}
			else
			{
				TempData["message"] = "Nu aveti drepturile de a sterge acest mesaj !";
				return Redirect("/Books/Show/" + rev.BookId);
			}
			//sau la index
		}
		[Authorize(Roles = "Admin")]
		public ActionResult New()
		{
			return View();
		}
		[Authorize(Roles = "Admin,Colaborator,User")]
		[HttpPost]
		public ActionResult New(Review rev)
		{
			rev.Date = DateTime.Now;
			rev.UserId = User.Identity.GetUserId();
			try
			{
				
				if (ModelState.IsValid)
				{
					db.Reviews.Add(rev);
					db.SaveChanges();
					TempData["message"] = "Review adaugat cu succes";
					return Redirect("/Books/Show/" + rev.BookId);
				}
				return Redirect("/Books/Show/" + rev.BookId);
			}
			catch (Exception)
			{ return Redirect("/Books/Show/" + rev.BookId); }
		}
		[Authorize(Roles = "Admin,Colaborator,User")]
		public ActionResult Edit(int id)
		{
			Review rev = db.Reviews.Find(id);
			ViewBag.Review = rev;
			return View(rev);
		}
		[Authorize(Roles = "Admin,Colaborator,User")]
		[HttpPut]
		public ActionResult Edit(int id, Review requestReview)
		{
			try
			{
				Review rev = db.Reviews.Find(id);
				if (ModelState.IsValid)
				{
					if(rev.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
					{
						if (TryUpdateModel(rev))
						{
							rev.Rating = requestReview.Rating;
							rev.Comment = requestReview.Comment;
							db.SaveChanges();
							TempData["message"] = "Review editat cu succes!";
							return Redirect("/Books/Show/" + rev.BookId);
						}
					}
					else
					{
						TempData["message"] = "Nu aveti drepturile pentru a edita acest comentariu";
						return Redirect("/Books/Show/" + rev.BookId);
					}
					
				}
				return Redirect("/Books/Show/" + rev.BookId);
			}
			catch (Exception)
			{
				TempData["message"] = "ceva a mers prost";
				return Redirect("/Books/all");
			}
		}

	}
}