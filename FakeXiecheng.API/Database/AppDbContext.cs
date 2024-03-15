using System;
using FakeXiecheng.API.Models;
using Microsoft.EntityFrameworkCore;
namespace FakeXiecheng.API.Database;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

//namespace FakeXiecheng.API.Database
//{
    public class AppDbContext : IdentityDbContext<ApplicationUser>//DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<TouristRoute> TouristRoutes { get; set; }
        public DbSet<TouristRoutePicture> touristRoutePictures { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<LineItem> lineItems { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<TouristRoute>().HasData(new TouristRoute()
            //{
            //Id = Guid.NewGuid(),
            //Title = "ceshititle",
            //Description = "shouming",
            //OriginalPrice = 0,
            //CreateTime = DateTime.UtcNow
            //Features = "beautiful",
            //Fees = "zan",
            //Notes = "xiaoxin"
            //});
            //var touristRouteJasonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + @"/Database/touristRoutesMockData.json"));
            var touristRouteJasonData = File.ReadAllText(@"/users/zengalier/Projects/FakeXiecheng.API/FakeXiecheng.API/bin/Debug/net7.0/Database/touristRoutesMockData.json");
            IList<TouristRoute> touristRoutes = JsonConvert.DeserializeObject<IList<TouristRoute>>(touristRouteJasonData);
            modelBuilder.Entity<TouristRoute>().HasData(touristRoutes);

            //var touristRoutePictureJasonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location + @"/Database/touristRoutePicturesMockData.json"));
            var touristRoutePictureJasonData = File.ReadAllText(@"/users/zengalier/Projects/FakeXiecheng.API/FakeXiecheng.API/bin/Debug/net7.0/Database/touristRoutePicturesMockData.json");
            IList<TouristRoutePicture> touristRoutePictures = JsonConvert.DeserializeObject<IList<TouristRoutePicture>>(touristRoutePictureJasonData);
            modelBuilder.Entity<TouristRoutePicture>().HasData(touristRoutePictures);

        //初始化用戶與角色的種子數據
        //1. 更新用戶與角色的外鍵
        modelBuilder.Entity<ApplicationUser>(u =>u.HasMany(x => x.UserRoles).WithOne().HasForeignKey(ur => ur.UserId).IsRequired());

        //2. 添加管理員角色
        var adminRoleId = "b693a53f-ad24-4fb0-bca0-92f56ae6406b";
        modelBuilder.Entity<IdentityRole>().HasData(
            new IdentityRole()
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "Admin".ToUpper()
            });

        //3. 添加用戶
        var adminUserId = "b693a53f-ad24-4fb0-bca0-92f56ae6404k";
        ApplicationUser adminUser = new ApplicationUser
        {
            Id = adminUserId,
            UserName = "TestAdmin@fakeXiecehng.com",
            Address = "taiwan",
            NormalizedUserName = "TestAdmin@fakeXiecehng.com".ToUpper(),
            Email = "TestAdmin@fakeXiecehng.com",
            NormalizedEmail = "TestAdmin@fakeXiecehng.com".ToUpper(),
            TwoFactorEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "0999999999",
            PhoneNumberConfirmed = false,
            //PasswordHash = "3DihfoHs-58fabf"
        };
        var ph = new PasswordHasher<ApplicationUser>();
        adminUser.PasswordHash = ph.HashPassword(adminUser, "3DihfoHs-58fabf");
        modelBuilder.Entity<ApplicationUser>().HasData(adminUser);
        //4. 給用戶加入管理員角色
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>()
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });
            base.OnModelCreating(modelBuilder);
        }


    }
//}
