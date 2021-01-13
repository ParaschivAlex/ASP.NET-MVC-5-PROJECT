using BookStore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookStore.Startup))]
namespace BookStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateAdminUserAndApplicationRoles();
        }
        private void CreateAdminUserAndApplicationRoles()
        {
            
            ApplicationDbContext context = new ApplicationDbContext();
            context.Configuration.LazyLoadingEnabled = true;
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            // Se adauga rolurile aplicatiei
            if (!roleManager.RoleExists("Admin"))
            {
				// Se adauga rolul de administrator
				var role = new IdentityRole
				{
					Name = "Admin"
				};
				roleManager.Create(role);
				// se adauga utilizatorul administrator
				var user = new ApplicationUser
				{
					UserName = "admin@gmail.com",
					Email = "admin@gmail.com"
				};
				var adminCreated = UserManager.Create(user, "!1Admin");
                if (adminCreated.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Admin");
                }
            }
            if (!roleManager.RoleExists("Colaborator"))
            {
				IdentityRole role = new IdentityRole
				{
					Name = "Colaborator"
				};
				roleManager.Create(role);
            }
            if (!roleManager.RoleExists("User"))
            {
				var role = new IdentityRole
				{
					Name = "User"
				};
				roleManager.Create(role);
            }
			if (!roleManager.RoleExists("FaraDrepturi"))
			{
				var role = new IdentityRole
				{
					Name = "Fara Drepturi"
				};
				roleManager.Create(role);
			}
		}
    }
}
