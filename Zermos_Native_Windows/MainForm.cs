using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;

namespace Zermos_Native_Windows
{
    public partial class MainForm : Form
    {
        private WebView2 webView;
        private bool isDebugVersion;
        private string deepLink;

        public MainForm(bool isDebugVersion, string? deepLink = null)
        {
            this.isDebugVersion = isDebugVersion;
            this.deepLink = deepLink ?? string.Empty;
            InitializeComponent();
            InitializeWebView();
        }

        private void HandleDeeplink(Uri uri)
        {
            // Extract the query parameters from the deeplink URI
            var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            var code = queryParams["code"];
            var iss = queryParams["iss"];
            var state = queryParams["state"];

            // Navigate the WebView to the callback URL with the extracted parameters
            string callbackUrl = $"https://zermos.kronk.tech/koppelingen/somtoday/callback?code={code}&iss={iss}&state={state}";
            webView.Source = new Uri(callbackUrl);
        }

        private async Task InitializeWebView()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            await webView.EnsureCoreWebView2Async();
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false; // Disable context menus
            webView.CoreWebView2.Settings.UserAgent += " zermos_windows_app";

            // Handle JavaScript message from the website
            //webView.CoreWebView2.WebMessageReceived += WebView_WebMessageReceived;

            Controls.Add(webView);

            // Load either the beta or production site based on the flag
            string url = isDebugVersion ? "https://zermos-beta.kronk.tech" : "https://zermos.kronk.tech";
            
            
            // If there's a deeplink, navigate to the callback with parameters
            if (!string.IsNullOrEmpty(this.deepLink))
            {
                HandleDeeplink(new Uri(this.deepLink));
            }
            else
            {
                webView.Source = new Uri(url);
            }
            
        }

        //invoke using window.chrome.webview.postMessage("#ff0000")
        // private void WebView_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        // {
        //     // Assuming the website sends a JSON message like { "color": "#ff0000" }
        //     var colorMessage = e.TryGetWebMessageAsString();
        //     if (!string.IsNullOrEmpty(colorMessage))
        //     {
        //         try
        //         {
        //             Color newColor = ColorTranslator.FromHtml(colorMessage);
        //             this.BackColor = newColor; // Change the form background color
        //             //color of the top bar (minimize, maximize, close) set to the same color
        //             this.ControlBox = false;
        //             this.ControlBox = true;
        //         }
        //         catch
        //         {
        //             // Handle invalid color
        //         }
        //     }
        // }
    }
}