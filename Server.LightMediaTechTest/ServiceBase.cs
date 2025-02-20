using Server.LightMediaTechTest.DatabaseManager;
using System.Security.Cryptography;
using System.Text;

namespace Server.LightMediaTechTest
{
    public class ServiceBase
    {
        internal async Task<ServiceResponse<T>> ExecAsync<T>(Func<MyContext, ServiceResponse<T>, Task<T?>> method)
        {
            MyContext context = new MyContext();
            ServiceResponse<T> serviceResponse = new ServiceResponse<T>();

            try
            {
                var invoke = method.DynamicInvoke(context, serviceResponse) as Task<T?>;

                if (invoke is null)
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

            if (!serviceResponse.Errors.Any())
                serviceResponse.Success = true;

            return serviceResponse;
        }

        internal async Task<ServiceResponse> ExecAsync(Func<MyContext, ServiceResponse, Task> method)
        {
            MyContext context = new MyContext();
            ServiceResponse serviceResponse = new ServiceResponse();

            try
            {
                var invoke = method.DynamicInvoke(context, serviceResponse) as Task;

                if (invoke is null)
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

            if (!serviceResponse.Errors.Any())
                serviceResponse.Success = true;

            return serviceResponse;
        }

        internal (string ProcessedHash, string Salt) ProcessHash(string password, string? salt = null)
        {
            using (SHA512 sha512 = SHA512.Create())
            {
                salt = salt ?? Guid.NewGuid().ToString();
                byte[] combinedHash = Encoding.ASCII.GetBytes(salt + password);

                return new(Encoding.ASCII.GetString(sha512.ComputeHash(combinedHash)), salt);

            }
        }

        internal bool ValidateHash(string password, string storedHash, string storedSalt)
        {
            var result = ProcessHash(password, storedSalt);

            if (result.ProcessedHash == storedHash)
                return true;
            else
                return false;
        }
    }
}

public class ServiceResponse<T>() : ServiceResponse
{
    public T? Result { get; set; }
}

public class ServiceResponse()
{
    public bool Success { get; set; } = false;
    public List<string> Errors { get; set; } = new List<string>();
}