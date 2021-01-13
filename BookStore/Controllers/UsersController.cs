﻿using System;
using Microsoft.AspNet.Identity.EntityFramework;
using BookStore.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace BookStore.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UsersController : Controller
	{
		private ApplicationDbContext db = new ApplicationDbContext();

		// GET: Users
		public ActionResult Index()
		{
			var users = from user in db.Users
						orderby user.UserName
						select user;
			ViewBag.UsersList = users;
			var books = db.Books.Include("Category").Include("User").Where(a => a.B_status == 0);
			ViewBag.cartiNeacceptate = books;
			return View();
		}

		public ActionResult Show(string id)
		{
			
			ApplicationUser user = db.Users.Find(id);
			ViewBag.utilizatorCurent = User.Identity.GetUserId();

			//var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());


			string currentRole = user.Roles.FirstOrDefault().RoleId;

			var userRoleName = (from role in db.Roles
								where role.Id == currentRole
								select role.Name).First();

			ViewBag.roleName = userRoleName;

			return View(user);
		}

		public ActionResult AcceptaProdus(int id)
		{
			Book book = db.Books.Find(id);
			book.B_status = 1;
			db.SaveChanges();
			return RedirectToAction("Index");
		}

		[HttpPost]
		public ActionResult StergeProdus(int id)
		{
			Book book = db.Books.Find(id);
			db.Books.Remove(book);
			db.SaveChanges();

			return RedirectToAction("Index");
		}


		public ActionResult Edit(string id)
		{
			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;
			return View(user);
		}

		[HttpPut]
		public ActionResult Edit(string id, ApplicationUser newData)
		{
			ApplicationUser user = db.Users.Find(id);
			user.AllRoles = GetAllRoles();
			var userRole = user.Roles.FirstOrDefault();
			ViewBag.userRole = userRole.RoleId;

			try
			{
				ApplicationDbContext context = new ApplicationDbContext();
				var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
				var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));


				if (TryUpdateModel(user))
				{
					user.UserName = newData.UserName;
					user.Email = newData.Email;
					user.PhoneNumber = newData.PhoneNumber;

					var roles = from role in db.Roles select role;
					foreach (var role in roles)
					{
						UserManager.RemoveFromRole(id, role.Name);
					}

					var selectedRole = db.Roles.Find(HttpContext.Request.Params.Get("newRole"));
					UserManager.AddToRole(id, selectedRole.Name);

					db.SaveChanges();
				}
				return RedirectToAction("Index");
			}
			catch (Exception e)
			{
				Response.Write(e.Message);
				newData.Id = id;
				return View(newData);
			}
		}

		[NonAction]
		public IEnumerable<SelectListItem> GetAllRoles()
		{
			var selectList = new List<SelectListItem>();

			var roles = from role in db.Roles select role;
			foreach (var role in roles)
			{
				selectList.Add(new SelectListItem
				{
					Value = role.Id.ToString(),
					Text = role.Name.ToString()
				});
			}
			return selectList;
		}

		[HttpDelete]
		public ActionResult Delete(string id)
		{
			ApplicationDbContext context = new ApplicationDbContext();

			var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

			var user = UserManager.Users.FirstOrDefault(u => u.Id == id);

			var books = db.Books.Where(a => a.UserId == id);
			foreach (var book in books)
			{
				db.Books.Remove(book);

			}

			var reviews = db.Reviews.Where(review=> review.UserId == id);
			foreach (var review in reviews)
			{
				db.Reviews.Remove(review);
			}

			// Commit pe articles
			db.SaveChanges();
			UserManager.Delete(user);
			return RedirectToAction("Index");
		}
	}
}