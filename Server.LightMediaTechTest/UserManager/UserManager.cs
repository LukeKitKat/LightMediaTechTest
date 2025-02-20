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

        public async Task<ServiceResponse> UpdateUserDetails(User user)
        {
            return await ExecAsync(async (db, resp) =>
            {
                db.Update<User>(user);
                await db.SaveChangesAsync();
            });
        }

        public async Task<ServiceResponse<List<UserRole>>> FetchAllUserRolesAsync()
        {
            return await ExecAsync<List<UserRole>>(async (db, resp) =>
            {
                return await db.UserRoles.AsNoTracking().ToListAsync();
            });
        }

        public async Task<ServiceResponse<List<User>>> FetchAllUsersAsync()
        {
            return await ExecAsync<List<User>>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking().ToListAsync();
            });
        }

        public async Task<ServiceResponse<User?>> FetchUserByIdAsync(int userId)
        {
            return await ExecAsync<User?>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking()
                                     .Include(x => x.UserRoles)
                                     .FirstOrDefaultAsync(x => x.Id == userId);
            });
        }

        public async Task<ServiceResponse<User?>> FetchUserByUsernameAsync(string username)
        {
            return await ExecAsync<User?>(async (db, resp) =>
            {
                return await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.AccountName == username);
            });
        }

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
    }
}
