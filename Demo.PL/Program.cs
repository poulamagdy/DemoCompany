using AutoMapper;
using Demo.BLL.Interface;
using Demo.BLL.Repository;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Mapper;
using Demo.PL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<DemoCompanyContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IUintOfWork, UintOfWork>();
            builder.Services.AddAutoMapper(map => map.AddProfiles(new List<Profile>() { 
                new DepartmentProfile(), new EmployeeProfile(), new UserProfile(), new RolesProfile() }));
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = new PathString("Account/Login");
                    option.AccessDeniedPath = new PathString("Home/Error");

                });

            builder.Services.AddIdentity<ApplicationUser, ApplicationRoles>(option =>
            {
                option.Password.RequireDigit = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequiredLength = 10;
                option.Password.RequireNonAlphanumeric = true;
                option.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<DemoCompanyContext>()
            .AddDefaultTokenProviders();

            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
            builder.Services.AddScoped<IMailSettings, EmailSettings>();

            builder.Services.Configure<SmsSettings>(builder.Configuration.GetSection("Twillo"));
            builder.Services.AddScoped<ISmsService, SmsService>();

            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddGoogle(o =>
            {
                IConfiguration googleAuth = builder.Configuration.GetSection("Authentication:Google");

                o.ClientId = googleAuth["ClientId"];
                o.ClientSecret = googleAuth["ClientSecret"];
			});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}