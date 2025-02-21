using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Server.LightMediaTechTest.DatabaseContext.Models;
using Server.LightMediaTechTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.DatabaseContext
{
    public class MyContext : DbContext
    {
        private readonly AppSettings _appSettings;
        internal MyContext(AppSettings appSettings) =>
            _appSettings = appSettings;

        internal async Task<MyContext?> InitializeAsync()
        {
            await Database.EnsureCreatedAsync();

            if (await Database.CanConnectAsync())
                return this;
            else
                return null;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventCatagory> EventCatagories { get; set; }
        public DbSet<EventUser> EventUsers { get; set; }

        /// <summary>
        /// Basic OnConfiguring declaration to allocate the connection string of the database and configure it.
        /// Because the database is code-first, any default and/or non-mutable data crucial for the function of the system will be seeded here.
        /// For a full scale product, it would generally be more legible to create a separate static method in a separate file and call it within the UseAsyncSeeding by passing the parameters.
        /// </summary>
        /// <param name="optionsBuilder">The optionsBuilder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder
            .UseSqlServer(_appSettings.ConnectionString)
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
            {
                if (context.Set<UserRole>() is DbSet<UserRole> userRole && await userRole.CountAsync() == 0)
                {
                    await userRole.AddRangeAsync(
                    [
                        new () { RoleName = "User", CanAmendEvents = false, CanExportDetails = false, CanManageUsers = false },
                        new () { RoleName = "Organiser", CanAmendEvents = false, CanExportDetails = true, CanManageUsers = true },
                        new () { RoleName = "Charity Staff", CanAmendEvents = true, CanExportDetails = true, CanManageUsers = true },
                    ], cancellationToken);
                }

                if (context.Set<EventCatagory>() is DbSet<EventCatagory> eventCatagory && await eventCatagory.CountAsync() == 0)
                {
                    await eventCatagory.AddRangeAsync(
                    [
                        new() { CatagoryName = "Fun-Run" },
                        new() { CatagoryName = "Bake-Off" },
                    ], cancellationToken);
                }

                if (context.ChangeTracker.HasChanges())
                    await context.SaveChangesAsync(cancellationToken);
            });
    }
}
