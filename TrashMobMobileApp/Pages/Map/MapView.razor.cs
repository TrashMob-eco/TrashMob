﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrashMobMobileApp.Pages.Map
{
    public partial class MapView
    {

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;

        protected override Task OnInitializedAsync()
        {
            TitleContainer.Title = "Event Map";
            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JsRuntime.InvokeVoidAsync("InitMap");
        }
    }
}