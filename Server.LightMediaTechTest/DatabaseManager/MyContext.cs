using Microsoft.EntityFrameworkCore;
using Server.LightMediaTechTest.DatabaseManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseManager
{
    public class MyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Event> Events { get; set; }
        public DbSet<EventCatagory> EventCatagories { get; set; }
        public DbSet<EventUser> EventAttendees { get; set; }

        public MyContext()
        {
            if (Database.EnsureCreated())
            {
                using (var context = new MyContext())
                {
                    context.UserRoles.AddRange(new List<UserRole>()
                    {
                        new () { RoleName = "User", CanAmendEvents = false, CanExportDetails = false, CanManageUsers = false },
                        new () { RoleName = "Organiser", CanAmendEvents = false, CanExportDetails = true, CanManageUsers = true },
                        new () { RoleName = "Charity Staff", CanAmendEvents = true, CanExportDetails = true, CanManageUsers = true },
                    });

                    context.EventCatagories.AddRange(new List<EventCatagory>()
                    {
                        new() { CatagoryName = "Fun-Run" },
                        new() { CatagoryName = "Bake-Off" },
                    });
                    {

                        context.SaveChanges();
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=LightMediaDevelopment;Trusted_Connection=true;TrustServerCertificate=true;Encrypt=false;ConnectRetryCount=0");
        }
    }
}
