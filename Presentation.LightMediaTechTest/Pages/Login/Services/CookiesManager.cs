using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Pages.Login.Services
{
    public class CookiesManager
    {
        private IJSRuntime JSRuntime { get; set; }
        private protected string KeyUUID { get => "lightmediacookie"; }

        public CookiesManager(IJSRuntime _jsRuntime) =>
            JSRuntime = _jsRuntime;

        public async Task<string> GetCookieAsync(string key)
        {
            var cookieString = await JSRuntime.InvokeAsync<string>("GetCookie");
            return cookieString.Replace($"{KeyUUID}-{key}=", "");
        }

        public async Task StoreCookieAsync(string key, string value) =>
            await JSRuntime.InvokeVoidAsync("SetCookie", $"{KeyUUID}-{key}", value);
    }
}
