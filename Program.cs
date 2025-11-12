using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using movie_hospital_1.Reposotories.IReposotories;
using movie_hospital_1.Utilities;
using movie_hospital_1.Utilities.DBInitilizer;
using NuGet.Protocol.Core.Types;

namespace movie_hospital_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]);
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IRepossitory<Category>, Repossitory<Category>>();
            builder.Services.AddScoped<IRepossitory<Actor>, Repossitory<Actor>>();
            builder.Services.AddScoped<IRepossitory<Cinema>, Repossitory<Cinema>>();
            builder.Services.AddScoped<IRepossitory<Movie>, Repossitory<Movie>>();
            builder.Services.AddScoped<IDBInitializer, DBInitializer>();
            builder.Services.AddScoped<OrderRepository>();
            object value = builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
             .AddDefaultTokenProviders();

            builder.Services.AddScoped<MovieRepository>();
            builder.Services.AddScoped<CategoryRepository>();
            builder.Services.AddScoped<CinemaRepository>();
            builder.Services.AddScoped<ActorRepository>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped<IRepossitory<ApplicationUserOTP>, Repossitory<ApplicationUserOTP>>();

            var app = builder.Build();

            var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider.GetService<IDBInitializer>();
            service!.Initialize();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
