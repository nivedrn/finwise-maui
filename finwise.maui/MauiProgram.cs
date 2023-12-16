﻿using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using finwise.maui.ViewModels;
using finwise.maui.Views;
using finwise.maui.Helpers;

namespace finwise.maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MyStorage>();
            builder.Services.AddSingleton<BaseViewModel>();

            builder.Services.AddTransient<AppShellViewModel>();
            builder.Services.AddTransient<ExpenseEditorPage>();
            builder.Services.AddTransient<ExpenseEditorViewModel>();
            builder.Services.AddTransient<HomePageViewModel>();


            //          Microsoft.Maui.Handlers.PickerHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
            //          {
            //#if ANDROID
            //              handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            //#endif
            //          });

//            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(Entry), (handler, view) =>
//            {
//#if ANDROID
//                        handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
//#endif
//            });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
