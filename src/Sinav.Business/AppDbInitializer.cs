using System;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Sinav.Data.Context;
using Sinav.Data.Models;

namespace Sinav.Business
{
    public class AppDbInitializer
    {


        public static void SeedUsers(UserManager<AppUser> userManager, AppDbContext _context)
        {
            if (_context.Cities.Count() == 0)
            {
                var cities =
                    "Adana, Adıyaman, Afyon, Ağrı, Amasya, Ankara, Antalya, Artvin, Aydın, Balıkesir, Bilecik, Bingöl, Bitlis, Bolu, Burdur, Bursa, Çanakkale, Çankırı, Çorum, Denizli, Diyarbakır," +
                    " Edirne, Elazığ, Erzincan, Erzurum, Eskişehir, Gaziantep, Giresun, Gümüşhane, Hakkari, Hatay, Isparta, İçel (Mersin), İstanbul, İzmir, Kars, Kastamonu, Kayseri, Kırklareli," +
                    " Kırşehir, Kocaeli, Konya, Kütahya, Malatya, Manisa, Kahramanmaraş, Mardin, Muğla, Muş, Nevşehir, Niğde, Ordu, Rize, Samsun, Siirt, Sinop, Sivas, Tekirdağ, Tokat," +
                    " Trabzon, Tunceli, Şanlıurfa, Uşak, Van, Yozgat, Zonguldak, Aksaray, Bayburt, Karaman, Kırıkkale, Batman, Şırnak, Bartın, Ardahan, Iğdır, Yalova, Karabük, Kilis, Osmaniye, Düzce";

                cities = cities.Trim();
                _context.Cities.Add(new City() {Name = "Sakarya"});
                foreach (var city in cities.Split(",").ToList())
                {
                    _context.Cities.Add(new City() {Name = city});
                }

                _context.SaveChanges();

            }

            if (_context.UnionBranches.Count() == 0)
            {
                var branch = new UnionBranch()
                {
                    CityId = 1,
                    Curator = "YUNUS EMRE YÜZÇELER",
                    Phone = "05454154054",
                    Email = "sakarya@buromemursen.org.tr",
                    Name = "Sakarya",
                    IsDeleted = false
                };
                _context.UnionBranches.Add(branch);
                _context.SaveChanges();
            }
       /*     if (userManager.FindByEmailAsync("admin@admin.com").Result==null)
            {
                AppUser user = new AppUser()
                {
                    Email = "admin@admin.com",
                    Avatar = @"\assets\images\users\Polat-Alemdar.jpg",
                    OrganizationId = 1,
                    FirstName = "Necati",
                    LastName = "Haymatlos",
                    IsApproved = true,
                    StaffId = 1,
                    Gender = true,
                    UnionBranchId = _context.UnionBranches.First(x => x.Name == "Sakarya").Id,
                    UserName = "admin@admin.com",
                    EmailConfirmed = true
                };

                IdentityResult result = userManager.CreateAsync(user, "tuqivoj_2").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }*/
            
            if (userManager.FindByEmailAsync("admin@bmskariyer.com").Result==null)
            {
                AppUser user = new AppUser()
                {
                    Email = "admin@bmskariyer.com",
                    Avatar = @"\assets\images\users\Polat-Alemdar.jpg",
                    OrganizationId = 1,
                    FirstName = "Yunus Emre",
                    LastName = "YÜZÇELER",
                    IsApproved = true,
                    StaffId = 1,
                    Gender = true,
                    UnionBranchId = _context.UnionBranches.First(x => x.Name == "Sakarya").Id,
                    EmailConfirmed = true,
                    UserName = "admin@bmskariyer.com",
                    PhoneNumber = "5382168604"
                };

                IdentityResult result = userManager.CreateAsync(user, "tuqivoj_2").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }
            
            if (userManager.FindByEmailAsync("n_kilic@hotmail.com").Result==null)
            {
                AppUser user = new AppUser()
                {
                    Email = "n_kilic@hotmail.com",
                    Avatar = @"\assets\images\users\Polat-Alemdar.jpg",
                    OrganizationId = 1,
                    FirstName = "HASAN",
                    LastName = "KILIÇ",
                    IsApproved = true,
                    StaffId = 1,
                    Gender = true,
                    UnionBranchId = _context.UnionBranches.First(x => x.Name == "Sakarya").Id,
                    EmailConfirmed = true,
                    UserName = "n_kilic@hotmail.com",
                    PhoneNumber = "5052185049"
                };

                IdentityResult result = userManager.CreateAsync(user, "123456").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }

            if (userManager.FindByEmailAsync("editor@bmskariyer.com").Result==null)
            {
                AppUser user = new AppUser()
                {
                    Email = "editor@bmskariyer.com",
                    Avatar = @"\assets\images\users\Polat-Alemdar.jpg",
                    OrganizationId = 1,
                    FirstName = "ABDÜLHEY",
                    LastName = "ÇOBANOĞLU",
                    IsApproved = true,
                    StaffId = 1,
                    Gender = true,
                    UnionBranchId = _context.UnionBranches.First(x => x.Name == "Sakarya").Id,
                    EmailConfirmed = true,
                    UserName = "editor@bmskariyer.com",
                    PhoneNumber = "5555555555"
                };

                IdentityResult result = userManager.CreateAsync(user, "tr1042_64").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }
            
            
            if (userManager.FindByEmailAsync("editor2@bmskariyer.com").Result==null)
            {
                AppUser user = new AppUser()
                {
                    Email = "editor2@bmskariyer.com",
                    Avatar = @"\assets\images\users\Polat-Alemdar.jpg",
                    OrganizationId = 1,
                    FirstName = "ERHAN",
                    LastName = "GÜLLÜ",
                    IsApproved = true,
                    StaffId = 1,
                    Gender = true,
                    UnionBranchId = _context.UnionBranches.First(x => x.Name == "Sakarya").Id,
                    EmailConfirmed = true,
                    UserName = "editor2@bmskariyer.com",
                    PhoneNumber = "5555555555"
                };

                IdentityResult result = userManager.CreateAsync(user, "tr1042_64").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "admin").Wait();
                }
            }


        }  
    }
}