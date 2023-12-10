using Google.Android.Material.BottomSheet;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;

namespace finwise.maui
{
    public static partial class PageExtensions
    {
        public static BottomSheetDialog ShowBottomSheet(this Page page, IView bottomSheetContent, bool dimDismiss)
        {
            var bottomSheetDialog = new BottomSheetDialog(Platform.CurrentActivity?.Window?.DecorView.FindViewById(global::Android.Resource.Id.Content)?.Context ?? throw new InvalidOperationException("Context is null"));
            bottomSheetDialog.SetContentView(bottomSheetContent.ToPlatform(page.Handler?.MauiContext ?? throw new Exception("MauiContext is null")));
            bottomSheetDialog.Behavior.Hideable = dimDismiss;
            bottomSheetDialog.Behavior.FitToContents = true;
            bottomSheetDialog.Show();
            return bottomSheetDialog;
        }

        public static void CloseBottomSheet(this BottomSheetDialog bottomSheet)
        {
            bottomSheet.Dismiss();
        }
    }
}
