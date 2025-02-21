using Microsoft.Extensions.Configuration;
using Server.LightMediaTechTest.DatabaseContext;
using Server.LightMediaTechTest.Models;
using System.Security.Cryptography;
using System.Text;

namespace Server.LightMediaTechTest
{
    public class ServiceBase(AppSettings appSettings)
    {
        private readonly AppSettings _appSettings = appSettings;

        /// <summary>
        /// A handler which constructs a service response which handles the result of a desired type within a given Task, as well as any errors.
        /// </summary>
        /// <typeparam name="T">The type of the result of the task.</typeparam>
        /// <param name="method">The task to be handled.</param>
        /// <returns>The a service response containing the result and details of the given Task.</returns>
        internal async Task<ServiceResponse<T>> ExecAsync<T>(Func<MyContext, ServiceResponse<T>, Task<T?>> method)
        {
            ServiceResponse<T> serviceResponse = new();
            MyContext? context = await new MyContext(_appSettings).InitializeAsync();

            if (context is null)
            {
                serviceResponse.Errors.Add("Database could not initialize");
            }
            else
            {
                try
                {
                    if (method.DynamicInvoke(context, serviceResponse) is not Task<T?> invoke)
                    {
                        serviceResponse.Success = false;
                        return serviceResponse;
                    }

                    serviceResponse.Result = await invoke;
                }
                catch (Exception ex)
                {
                    serviceResponse.Errors.Add($"{ex.Message} - {ex.InnerException?.Message}" ?? "No Message Specified.");
                }
            }

            if (serviceResponse.Errors.Count == 0)
                serviceResponse.Success = true;

            return serviceResponse;
        }

        /// <summary>
        /// A handler which constructs a service response which handles the given Task, as well as any errors.
        /// </summary>
        /// <param name="method">The task to be handled.</param>
        /// <returns>The a service response containing the details of the given Task.</returns>
        internal async Task<ServiceResponse> ExecAsync(Func<MyContext, ServiceResponse, Task> method)
        {
            ServiceResponse serviceResponse = new();
            MyContext? context = await new MyContext(_appSettings).InitializeAsync();

            if (context is null)
            {
                serviceResponse.Errors.Add("Database could not initialize");
            }
            else
            {
                try
                {
                    if (method.DynamicInvoke(context, serviceResponse) is not Task invoke)
                    {
                        serviceResponse.Success = false;
                        return serviceResponse;
                    }

                    await invoke;
                }
                catch (Exception ex)
                {
                    serviceResponse.Errors.Add($"{ex.Message} - {ex.InnerException?.Message}" ?? "No Message Specified.");
                }
            }

            if (serviceResponse.Errors.Count == 0)
                serviceResponse.Success = true;

            return serviceResponse;
        }

        /// <summary>
        /// Salts and creates hashes of given data. Generally used for passwords.
        /// </summary>
        /// <param name="data">The data being hashed.</param>
        /// <param name="salt">The salt, if any, used alongside the hash.</param>
        /// <returns>The processed hash and the salt in case a salt was generated for the data.</returns>
        internal (string ProcessedHash, string Salt) ProcessHash(string data, string? salt = null)
        {
            salt ??= Guid.NewGuid().ToString();
            byte[] combinedHash = Encoding.ASCII.GetBytes(salt + data);

            return new(Encoding.ASCII.GetString(SHA512.HashData(combinedHash)), salt);
        }

        /// <summary>
        /// Validates given data against the stored hash and salt to identify if they match.
        /// </summary>
        /// <param name="data">The unhashed, fresh data.</param>
        /// <param name="storedHash">The salted and hashed data from storage.</param>
        /// <param name="storedSalt">The salt used alongside the hash.</param>
        /// <returns>A condition stating whether the data is valid and matches the stored hash, or not.</returns>
        internal bool ValidateHash(string data, string storedHash, string storedSalt)
        {
            if (ProcessHash(data, storedSalt).ProcessedHash == storedHash)
                return true;
            else
                return false;
        }
    }
}

