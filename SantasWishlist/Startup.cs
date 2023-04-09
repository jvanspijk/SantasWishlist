using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SantasWishlist.Context;
using SantasWishlist.Domain;
using SantasWishlistWeb.Models;
using System.Globalization;


namespace SantasWishlistWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //CultureInfo.InvariantCulture omdat de kerstman kindjes over
            //de hele wereld moet kunnen bedienen:
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SantasWishlistContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("SantasWishlist")));            
            services.AddScoped<IGiftRepository, GiftRepository>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddControllersWithViews();
            services.ConfigureApplicationCookie(options => options.LoginPath = "/login");
            services.AddIdentity<SantasWishlistUser, IdentityRole>().AddEntityFrameworkStores<SantasWishlistContext>().AddDefaultTokenProviders();
            
        }
        public void ConfigureTestServices(IServiceCollection services)
        {

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<SantasWishlistUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            
            app.UseAuthentication();
            DataSeeder.SeedRolesAndUsers(roleManager, userManager);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
