using AkilliSayac.Areas.Identity.Data;
using AkilliSayac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;

namespace AkilliSayac.Data
{
    [Authorize]
    public static class ContextSeed
    {
        public static void Seed(UserManager<AkilliSayacUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
            SeedLogTypes(db);
            SeedAnomalyTypes(db);
            SeedMalwareTypes(db);
            SeedDevices(db);
        }

        private static void SeedDevices(ApplicationDbContext db)
        {
            if (db.Devices.Count() == 0)
            {
                Device device = new Device
                {
                    DeviceName = "7UT85"
                };
                db.Devices.Add(device);

                Device device1 = new Device
                {
                    DeviceName = "6MD85"
                };
                db.Devices.Add(device1);
            }
            db.SaveChanges();
        }

        private static void SeedAnomalyTypes(ApplicationDbContext db)
        {
            if (db.AnomalyTypes.Count() == 0)
            {
                AnomalyType anomalyType = new AnomalyType
                {
                    AnomalyTypeName = "Point"
                };
                db.AnomalyTypes.Add(anomalyType);

                AnomalyType anomalyType1 = new AnomalyType
                {
                    AnomalyTypeName = "Contextual"
                };
                db.AnomalyTypes.Add(anomalyType1);

                AnomalyType anomalyType2 = new AnomalyType
                {
                    AnomalyTypeName = "Collective"
                };
                db.AnomalyTypes.Add(anomalyType2);
            }
            db.SaveChanges();
        }

        private static void SeedMalwareTypes(ApplicationDbContext db)
        {
            if (db.MalwareTypes.Count() == 0)
            {
                MalwareType malwareType = new MalwareType
                {
                    MalwareTypeName = "Worm"
                };
                db.MalwareTypes.Add(malwareType);

                MalwareType malwareType1 = new MalwareType
                {
                    MalwareTypeName = "Rootkit"
                };
                db.MalwareTypes.Add(malwareType1);

                MalwareType malwareType2 = new MalwareType
                {
                    MalwareTypeName = "Spyware"
                };
                db.MalwareTypes.Add(malwareType2);

                MalwareType malwareType3 = new MalwareType
                {
                    MalwareTypeName = "Adware"
                };
                db.MalwareTypes.Add(malwareType3);
            }
            db.SaveChanges();
        }

        private static void SeedLogTypes(ApplicationDbContext db)
        {
            if (db.LogTypes.Count() == 0)
            {
                LogType logType = new LogType
                {
                    LogTypeName = "Sistem",
                    LogMessageNumber = null
                };
                db.LogTypes.Add(logType);

                LogType logType1 = new LogType
                {
                    LogTypeName = "Pil Arızası",
                    LogMessageNumber = 827
                };
                db.LogTypes.Add(logType1);

                LogType logType2 = new LogType
                {
                    LogTypeName = "Cihaz Başlatıldı",
                    LogMessageNumber = 2112
                };
                db.LogTypes.Add(logType2);

                LogType logType3 = new LogType
                {
                    LogTypeName = "Zaman Eşzamanlama Hatası",
                    LogMessageNumber = 4739
                };
                db.LogTypes.Add(logType3);

                LogType logType4 = new LogType
                {
                    LogTypeName = "Güç Kaynağı Arızası",
                    LogMessageNumber = 4738
                };
                db.LogTypes.Add(logType4);
            }
            db.SaveChanges();
        }

        private static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "SuperAdmin"
                };
                var result = roleManager.CreateAsync(role).Result;
            }
            if (!roleManager.RoleExistsAsync("Admin").Result)
            {
                var role = new IdentityRole
                {
                    Name = "Admin"
                };
                var result = roleManager.CreateAsync(role);
            }
        }

        private static void SeedUsers(UserManager<AkilliSayacUser> userManager)
        {
            if(userManager.Users.Count() == 0)
            {
                var user = new AkilliSayacUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "baris@sau.edu.tr",
                    Email = "baris@sau.edu.tr",
                    RoleName = "SuperAdmin",
                    LastPasswordChangedDate = DateTime.Now,
                    EmailConfirmed = true
                };
                var result = userManager.CreateAsync(user, "P@ssword1").Result;
                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, "SuperAdmin").Wait();
                }
            }
        }
    }
}
