using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Entities.AppContext
{
    public class Context: DbContext
    {
        #region DB Set

        public DbSet<User> Users { get; set;}
        public DbSet<ActivityType> activityTypes{ get; set;}
        public DbSet<UsersActivity> usersActivities{ get; set;}

        #endregion

        #region Methods

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                var server = "localhost";
                var user = "sa";
                var catalog = "generalDB";
                var password = "@.J0s3RmP4ss";

                var stringConnection = $"Server=tcp:{server},1433;Initial Catalog={catalog};" +
                        $" PersistSecurityInfo = False;User ID={user}; Password={password}; " +
                        " MultipleActiveResultSets = False; Encrypt=True; " +
                        " TrustServerCertificate=True; Connection Timeout=30;";

                optionsBuilder.UseSqlServer(stringConnection);

            }
            catch (Exception ex)
            {
                var m = ex.Message;
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Constraints

            modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.PhoneNumber).IsUnique();
            modelBuilder.Entity<ActivityType>().HasIndex(at => at.ActivityTypeName).IsUnique();

            #endregion

            #region DefaultValues

            var activityType1 = new ActivityType() { ActivityTypeId = 1, ActivityTypeName = "User logged"};
            var activityType2 = new ActivityType() { ActivityTypeId = 2, ActivityTypeName = "User logged out"};
            var activityType3 = new ActivityType() { ActivityTypeId = 3, ActivityTypeName = "New user created"};
            var activityType4 = new ActivityType() { ActivityTypeId = 4, ActivityTypeName = "User verified"};

            #endregion

            #region SeedData

            modelBuilder.Entity<ActivityType>().HasData(new ActivityType[] { activityType1, activityType2, activityType3, activityType4 });
            
            #endregion
        }
        #endregion
    }
}