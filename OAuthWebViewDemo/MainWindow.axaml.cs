using Avalonia.Controls;
using OAuthWebView;
using System;

namespace OAuthWebViewDemo {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            button.Click += Button_Click;

            result.Text = "For example, enter this URLs:\n" +
                "Start: https://oauth.vk.com/authorize?client_id=6614620&response_type=token&revoke=1&display=mobile\n" + 
                "End: https://oauth.vk.com/blank.html";
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            Uri startUri = null, endUri = null;
            if (!Uri.TryCreate(start.Text, UriKind.Absolute, out startUri)
             || !Uri.TryCreate(end.Text, UriKind.Absolute, out endUri)) return;

            button.IsEnabled = false;

            var window = new OAuthWindow(startUri, endUri, "OAuth", 480, 420);
            Uri authResult = await window.StartAuthenticationAsync();

            result.Text = authResult == null ? "WebView closed" : authResult.ToString();
            button.IsEnabled = true;
        }
    }
}
