using Microsoft.EntityFrameworkCore;
using Server.LightMediaTechTest.DatabaseManager.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.LightMediaTechTest.UserManager
{
    public class UserManager : ServiceBase
    {
        /// <summary>
        /// Generates an example user for this Test.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> GenerateExampleUserAsync()
        {
            return await ExecAsync(async (db, resp) =>
            {
                // This is purely for the test environment. Normally an admin user would be generated in the database by an Engineer manually. However for this test, I am generating it automatically.
                // Password for this user is: Test
                var roleId = (await db.UserRoles.AsNoTracking().FirstAsync(x => x.RoleName == "Charity Staff")).Id;

                await db.Users.AddAsync(new User()
                {
                    AccountName = "Admin",
                    DisplayName = "Admin",
                    Email = "admin@charity.co.uk",
                    DoB = DateTime.MaxValue,
                    UserRoleId = roleId,
                    PasswordHash = "????=????\u001c???????\v?L??\"F\u0019?Z?u??r??u???$\u0015??\u0002?-??????b\u0005e\u0002\u001d?k\u001eT?\u0019??",
                    PasswordSalt = "ebc556e1-8834-4806-a1bb-dcdc119cb422"
                });

                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Registers a new user within the database using the data given by the model provided.
        /// </summary>
        /// <param name="user">The data model containing the user data.</param>
        /// <param name="password">The password entered by the user during registration.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> RegisterNewUserAsync(User user, string password)
        {
            return await ExecAsync(async (db, resp) =>
            {
                var loginServiceResult = ProcessHash(password);

                user.DisplayName = user.AccountName ?? string.Empty;
                user.PasswordHash = loginServiceResult.ProcessedHash;
                user.PasswordSalt = loginServiceResult.Salt;

                await db.AddAsync(user);
                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Validates the user details entered during login and returns the success condition.
        /// </summary>
        /// <param name="username">The username entered by the user.</param>
        /// <param name="attemptedPassword">The password attempted by the user.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<bool>> ValidateUserLoginAsync(string username, string attemptedPassword)
        {
            return await ExecAsync<bool>(async (db, resp) =>
            {
                var user = await db.Users.FirstOrDefaultAsync(x => x.AccountName == username);

                if (user is not null)
                {
                    if (ValidateHash(attemptedPassword, user.PasswordHash, user.PasswordSalt))
                    {
                        user.LastLoginLocation = Guid.NewGuid().ToString();
                        user.LastLogin = DateTime.UtcNow;

                        db.Update(user);
                        await db.SaveChangesAsync();

                        return true;
                    }
                }

                return false;
            });
        }

        /// <summary>
        /// Updates a user using the data provided by the passed data model.
        /// </summary>
        /// <param name="user">The data model containing the user data.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse> UpdateUserDetails(User user)
        {
            return await ExecAsync(async (db, resp) =>
            {
                db.Update(user);
                await db.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Fetches all roles within the database and returns them.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<List<UserRole>>> FetchAllUserRolesAsync()
        {
            return await ExecAsync<List<UserRole>>(async (db, resp) =>
            {
                return await db.UserRoles.AsNoTracking().ToListAsync();
            });
        }

        /// <summary>
        /// Fetches every user within the database and returns them.
        /// </summary>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<List<User>>> FetchAllUsersAsync()
        {
            return await ExecAsync<List<User>>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking().ToListAsync();
            });
        }

        /// <summary>
        /// Fetches a specific user by the provided ID.
        /// </summary>
        /// <param name="userId">The ID of the desired user.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<User?>> FetchUserByIdAsync(int userId)
        {
            return await ExecAsync<User?>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking()
                                     .Include(x => x.UserRoles)
                                     .FirstOrDefaultAsync(x => x.Id == userId);
            });
        }

        /// <summary>
        /// Fetches a user by their username.
        /// </summary>
        /// <param name="username">The username of the desired user.</param>
        /// <returns>The service response indicating conditions of the service after completion, as well as containing the result.</returns>
        public async Task<ServiceResponse<User?>> FetchUserByUsernameAsync(string username)
        {
            return await ExecAsync<User?>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.AccountName == username);
            });
        }
    }
}
