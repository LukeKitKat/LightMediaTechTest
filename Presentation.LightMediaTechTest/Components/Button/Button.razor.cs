using BlazorComponentUtilities;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.LightMediaTechTest.Components.Button
{
    public partial class Button
    {
        [Parameter]
        public EventCallback OnClick { get; set; }
        [Parameter]
        public string Class { get; set; } = string.Empty;
        [Parameter]
        public string Label { get; set; } = string.Empty;
        [Parameter]
        public string LabelClass { get; set; } = string.Empty;
        [Parameter]
        public string Icon { get; set; } = string.Empty;
        [Parameter]
        public string IconClass { get; set; } = string.Empty;
        [Parameter]
        public ButtonTypes ButtonType { get; set; }
        [Parameter]
        public bool Disabled { get; set; }

        /// <summary>
        /// CssBuilder to handle the class string creation for the button itself.
        /// </summary>
        private string _class => new CssBuilder("button")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// CssBuilder to handle the class string creation for the label.
        /// </summary>
        private string _labelClass => new CssBuilder("button-label")
            .AddClass(LabelClass)
            .Build();

        /// <summary>
        /// CssBuilder to handle the class string creation for the icon.
        /// </summary>
        private string _iconClass => new CssBuilder("icon-button-icon")
            .AddClass(Icon)
            .AddClass(IconClass)
            .Build();

        private string _htmlButtonType = string.Empty;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            switch (ButtonType)
            {
                case ButtonTypes.OnClick:
                    _htmlButtonType = "button";
                    break;

                case ButtonTypes.Submit:
                    _htmlButtonType = "submit";
                    break;
            }
        }

        /// <summary>
        /// A simple OnClick handler so that whenever OnClick is called, the page re-renders changes.
        /// </summary>
        private void OnClickHandler()
        {
            OnClick.InvokeAsync();
            StateHasChanged();
        }
    }
}
