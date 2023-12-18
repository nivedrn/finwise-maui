using Microsoft.Extensions.Logging;
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
                    fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
                    fonts.AddFont("Nunito-SemiBoldItalic.ttf", "NunitoSemiBoldItalic");
                    fonts.AddFont("Nunito-Black.ttf", "NunitoBlack");
                    fonts.AddFont("Nunito-BlackItalic.ttf", "NunitoBlackItalic");
                    fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                    fonts.AddFont("Nunito-BoldItalic.ttf", "NunitoBoldItalic");
                    fonts.AddFont("Nunito-ExtraBold.ttf", "NunitoExtraBold");
                    fonts.AddFont("Nunito-ExtraBoldItalic.ttf", "NunitoExtraBoldItalic");
                    fonts.AddFont("Nunito-ExtraLight.ttf", "NunitoExtraLight");
                    fonts.AddFont("Nunito-ExtraLightItalic.ttf", "NunitoExtraLightItalic");
                    fonts.AddFont("Nunito-Italic.ttf", "NunitoItalic");
                    fonts.AddFont("Nunito-Light.ttf", "NunitoLight");
                    fonts.AddFont("Nunito-LightItalic.ttf", "NunitoLightItalic");
                    fonts.AddFont("Nunito-Medium.ttf", "NunitoMedium");
                    fonts.AddFont("Nunito-MediumItalic.ttf", "NunitoMediumItalic");
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
