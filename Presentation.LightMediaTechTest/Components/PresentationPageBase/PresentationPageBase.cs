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

        public Task? OnAfterRenderAsyncHandler { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            var result = await GetCookieAsync<int>(LoginConstants.UserCookieId);

            var resp = await UserManager.FetchUserByIdAsync(result);
            if (!resp.Success)
                return;

            CurrentUser = resp.Result;

            if (OnAfterRenderAsyncHandler is not null)
                await OnAfterRenderAsyncHandler;

            StateHasChanged();
            
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task<T> GetCookieAsync<T>(string key) =>
            ParseCookies<T>(key, await JSRuntime.InvokeAsync<string>("GetCookie"));

        public async Task StoreCookieAsync(string key, string value) =>
            await JSRuntime.InvokeVoidAsync("SetCookie", $"{_keyUUID}-{key}", value);

        private T ParseCookies<T>(string key, string value)
        {
            try
            {
                return (T)Convert.ChangeType(value.Replace($"{_keyUUID}-{key}=", ""), typeof(T));
            }
            catch
            { 
                return default!; 
            }
        }
    }
}
