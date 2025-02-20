using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using Presentation.LightMediaTechTest.Pages.Login.Models;
using Server.LightMediaTechTest.DatabaseManager.Models;
using Server.LightMediaTechTest.UserManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Components.PresentationBase
{
    public partial class PresentationPageBase : LayoutComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject]
        UserManager UserManager { get; set; } = default!;

        public User? CurrentUser { get; set; }
        private protected string _keyUUID { get => "lightmediacookie"; }

        public delegate Task OnAfterRenderAsyncOverload(object sender, EventArgs args);
        public event OnAfterRenderAsyncOverload? OnAfterRenderAsyncOverloadEvent;

        /// <summary>
        /// Because this is a base class, we can use this method to update systems that are essential for every page's function.
        /// </summary>
        /// <param name="firstRender">Whether it is the page's first render cycle or not.</param>
        /// <returns>The completed task.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var result = await GetCookieAsync<int>(LoginConstants.UserCookieId);

            var resp = await UserManager.FetchUserByIdAsync(result);
            if (!resp.Success)
                return;

            CurrentUser = resp.Result;

            if (OnAfterRenderAsyncOverloadEvent is not null)
                await OnAfterRenderAsyncOverloadEvent.Invoke(this, new());

            StateHasChanged();
            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        /// Creates a file within the server, forces a download on the user's browser, then deletes the file from the server's memory.
        /// </summary>
        /// <param name="fileName">The full name of the file being created including extension.</param>
        /// <param name="fileContent">The content of the file.</param>
        /// <returns>The completed task.</returns>
        public async Task CreateAndDownloadFileAsync(string fileName, string fileContent)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Working");
            var filePath = Path.Combine(dir, fileName);
            Directory.CreateDirectory(dir);

            await File.WriteAllTextAsync(filePath, fileContent);

            using (var fileStream = File.OpenRead(filePath))
            {
                using (var streamRef = new DotNetStreamReference(fileStream))
                {
                    await JSRuntime.InvokeVoidAsync("DownloadFileFromStreamAsync", fileName, streamRef);
                }
            }

            File.Delete(filePath);
        }

        /// <summary>
        /// Retrieves a cookie from the session storage and returns it.
        /// </summary>
        /// <typeparam name="T">The type of the data being retrieved.</typeparam>
        /// <param name="key">The key identifying the data.</param>
        /// <returns>The data parsed to the desired type</returns>
        public async Task<T> GetCookieAsync<T>(string key) =>
            ParseCookies<T>(key, await JSRuntime.InvokeAsync<string>("GetCookie"));

        /// <summary>
        /// Stores a cookie into the session storage.
        /// </summary>
        /// <param name="key">The key to store the data under.</param>
        /// <param name="value">The value of the data.</param>
        /// <returns>The completed task.</returns>
        public async Task StoreCookieAsync(string key, string value) =>
            await JSRuntime.InvokeVoidAsync("SetCookie", $"{_keyUUID}-{key}", value);

        /// <summary>
        /// A helper method designed only to be used by the two public facing classes and assist in parsing the data fetched from a session.
        /// </summary>
        /// <typeparam name="T">The desired type of the data.</typeparam>
        /// <param name="key">The key identifying the data.</param>
        /// <param name="cookie">The value of the full cookie.</param>
        /// <returns></returns>
        private T ParseCookies<T>(string key, string cookie)
        {
            try
            {
                return (T)Convert.ChangeType(cookie.Replace($"{_keyUUID}-{key}=", ""), typeof(T));
            }
            catch
            {
                return default!;
            }
        }
    }
}
