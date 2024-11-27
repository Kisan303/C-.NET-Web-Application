using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using PetShop.Services;
using PetShop.Interfaces;
using PetShop.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; // Required for UserStore
using Unity.Lifetime; // Ensure this is added for lifetime management

namespace PetShop
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Register ApplicationDbContext as singleton
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            // Register UserStore for ApplicationUser
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());

            // Register ApplicationUserManager for user management
            container.RegisterType<ApplicationUserManager>(new HierarchicalLifetimeManager());

            // Register ApplicationSignInManager for user sign-in operations
            container.RegisterType<ApplicationSignInManager>(new HierarchicalLifetimeManager());

            // Register PetService and any other services with the appropriate lifetime
            container.RegisterType<IPetService, PetService>(new TransientLifetimeManager());

            // Set the DependencyResolver to use Unity for dependency injection
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
