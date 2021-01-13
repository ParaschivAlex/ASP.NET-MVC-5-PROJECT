using BookStore.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Proiect2.Controllers
{
	[Authorize(Roles = "User,Colaborator,Admin")]
	public class Is_InsController : Controller
	{

		private ApplicationDbContext db = new ApplicationDbContext();

		public ActionResult Index()
		{
			var userCurent = User.Identity.GetUserId();
			Basket cos = db.Baskets.Where(a => a.UserId == userCurent).First();

			var quantities = from q in db.Is_Ins where q.BasketId == cos.BasketId select q;
			ViewBag.Is_Ins = quantities;
			ViewBag.UserCurent = db.Users.Find(User.Identity.GetUserId());

			CalculeazaTotal();
			
			return View();
		}

		[HttpPost]
		public ActionResult New(Is_In q)
		{

			var userCurent = User.Identity.GetUserId();
			Basket cos = db.Baskets.Where(a => a.UserId == userCurent).First();

			q.BasketId = cos.BasketId;

			if (cos.Is_Ins.Where(a => a.BookId == q.BookId).Count() != 0)
			{
				Is_In quantity_temp = cos.Is_Ins.Where(a => a.BookId == q.BookId).First();
				quantity_temp.Quantity++;
				db.SaveChanges();
			}
			else
			{
				db.Is_Ins.Add(q);
				db.SaveChanges();
			}

			return Redirect("/Books/Index");
		}

		[HttpPut]
		public ActionResult EditPlus(int id)
		{
			Is_In q = db.Is_Ins.Find(id);
			q.Quantity++;
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpPut]
		public ActionResult EditMinus(int id)
		{
			Is_In q = db.Is_Ins.Find(id);
			if (q.Quantity == 1)
			{
				db.Is_Ins.Remove(q);
				db.SaveChanges();
				return RedirectToAction("Index");
			}
			else
			{
				q.Quantity--;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
		}

		[HttpDelete]
		public ActionResult Delete(int id)
		{
			Is_In q = db.Is_Ins.Find(id);
			db.Is_Ins.Remove(q);
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		void CalculeazaTotal()
		{
			var userCurent = User.Identity.GetUserId();
			Basket cos = db.Baskets.Where(a => a.UserId == userCurent).First();

			double sumaTotala = 0;


			foreach (var q in cos.Is_Ins)
			{
				sumaTotala = sumaTotala + (q.Quantity * q.Book.Price);
			}

			ViewBag.Suma = sumaTotala;
		}

	}
}