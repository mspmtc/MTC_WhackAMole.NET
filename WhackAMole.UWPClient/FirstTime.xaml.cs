using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WhackAMole.UWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstTime : Page
    {
        public FirstTime()
        {
            this.InitializeComponent();
        }

        private void Button_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values.Add("moleServiceEndpoint", MoleEndpoint.Text);
            ApplicationData.Current.LocalSettings.Values.Add("adminServiceEndpoint", AdminEndpoint.Text);
            this.Frame.Navigate(typeof(Start));
        }

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (args.IsContentChanging)
                Continue.IsEnabled = Uri.IsWellFormedUriString(AdminEndpoint.Text, UriKind.Absolute) && Uri.IsWellFormedUriString(MoleEndpoint.Text, UriKind.Absolute);
        }
    }
}
