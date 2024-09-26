using Microsoft.Web.WebView2.WinForms;
using System;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

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
            Logger.Log($"Initializing MainForm. Debug version: {isDebugVersion}, deeplink: {this.deepLink}");
            InitializeComponent();
            InitializeWebView();
        }

        private async Task InitializeWebView()
        {
            try
            {
                Logger.Log("Starting InitializeWebView method");
                webView = new WebView2
                {
                    Dock = DockStyle.Fill
                };
                Logger.Log("WebView2 instance created");

                var env = await CoreWebView2Environment.CreateAsync(null, Path.Combine(Path.GetTempPath(), "WebView2UserData"));
                Logger.Log("CoreWebView2Environment created");

                await webView.EnsureCoreWebView2Async(env);
                Logger.Log("EnsureCoreWebView2Async completed");

                webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView.CoreWebView2.Settings.UserAgent += " zermos_windows_app";
                Logger.Log("WebView2 settings configured");

                Controls.Add(webView);
                Logger.Log("WebView2 added to Controls");

                string url = isDebugVersion ? "https://zermos-beta.kronk.tech" : "https://zermos.kronk.tech";

                if (!string.IsNullOrEmpty(this.deepLink))
                {
                    Logger.Log($"Handling deeplink: {this.deepLink}");
                    HandleDeeplink(new Uri(this.deepLink));
                }
                else
                {
                    Logger.Log($"Navigating to URL: {url}");
                    webView.Source = new Uri(url);
                }

                Logger.Log("Navigation initiated");
            }
            catch (WebView2RuntimeNotFoundException ex)
            {
                Logger.Log($"WebView2 Runtime not found: {ex.Message}");
                MessageBox.Show("WebView2 Runtime is not installed. Please run the installer again or download it from Microsoft's website.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in InitializeWebView: {ex.Message}");
                Logger.Log($"Stack trace: {ex.StackTrace}");
                MessageBox.Show($"An error occurred while initializing WebView2: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HandleDeeplink(Uri uri)
        {
            try
            {
                Logger.Log($"Handling deeplink: {uri}");
                var queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var code = queryParams["code"];
                var iss = queryParams["iss"];
                var state = queryParams["state"];

                string callbackUrl =
                    $"https://zermos.kronk.tech/koppelingen/somtoday/callback?code={code}&iss={iss}&state={state}";
                Logger.Log($"Constructed callback URL: {callbackUrl}");

                webView.Source = new Uri(callbackUrl);
                Logger.Log("Navigation to callback URL initiated");
            }
            catch (Exception ex)
            {
                Logger.Log($"Exception in HandleDeeplink: {ex.Message}");
                Logger.Log($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}