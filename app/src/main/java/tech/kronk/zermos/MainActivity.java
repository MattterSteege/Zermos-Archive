package tech.kronk.zermos;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.DownloadManager;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.net.ConnectivityManager;
import android.net.ConnectivityManager.NetworkCallback;
import android.net.Network;
import android.net.NetworkCapabilities;
import android.net.Uri;
import android.os.Build;
import android.os.Bundle;
import android.os.Environment;
import android.util.Log;
import android.view.Window;
import android.view.WindowManager;
import android.webkit.CookieManager;
import android.webkit.URLUtil;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.Toast;

import tech.kronk.zermos.R;

public class MainActivity extends Activity {
    private WebView mWebView;
    private NetworkCallback networkCallback;
    private String baseURL = "https://zermos.kronk.tech";

    @SuppressLint("SetJavaScriptEnabled")
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        mWebView = findViewById(R.id.activity_main_webview);
        WebSettings webSettings = mWebView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        String defaultUA = webSettings.getUserAgentString(); // Get the default User-Agent string
        webSettings.setUserAgentString(defaultUA + " zermos_mobile_app");

        mWebView.setWebViewClient(new HelloWebViewClient(baseURL));
        handleDeeplink(getIntent()); // Call this after setting the WebView client

        // Add the JavaScript interface
        mWebView.addJavascriptInterface(new WebAppInterface(), "AndroidFunction");

        mWebView.setDownloadListener((url, userAgent, contentDisposition, mimetype, contentLength) -> {
            DownloadManager.Request request = new DownloadManager.Request(Uri.parse(url));
            request.setMimeType(mimetype);
            request.addRequestHeader("cookie", CookieManager.getInstance().getCookie(url));
            request.addRequestHeader("User-Agent", "zermos_mobile_app");
            request.setDescription("Downloading file...");
            request.setTitle(URLUtil.guessFileName(url, contentDisposition, mimetype));
            request.setNotificationVisibility(DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED);
            request.setDestinationInExternalPublicDir(Environment.DIRECTORY_DOWNLOADS, URLUtil.guessFileName(url, contentDisposition, mimetype));
            DownloadManager dm = (DownloadManager) getSystemService(DOWNLOAD_SERVICE);
            dm.enqueue(request);
            Toast.makeText(getApplicationContext(), "Downloading File", Toast.LENGTH_LONG).show();
        });

        // Handle the incoming deeplink
        handleDeeplink(getIntent());

        // Check network availability and load the initial URL
        if (!isDeeplinkHandled && isNetworkAvailable()) {
            mWebView.loadUrl(baseURL);
        } else if (!isDeeplinkHandled) {
            mWebView.loadUrl("file:///android_asset/offline.html");
        }

        networkCallback = new NetworkCallback() {
            @Override
            public void onAvailable(Network network) {
                runOnUiThread(() -> {
                    if (!mWebView.getUrl().startsWith("file:///android_asset")) {
                        mWebView.loadUrl(baseURL);
                    }
                });
            }

            @Override
            public void onLost(Network network) {
                runOnUiThread(() -> {
                    if (mWebView.getUrl() != null) {
                        mWebView.loadUrl("file:///android_asset/offline.html");
                    }
                });
            }
        };

        ConnectivityManager connectivityManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        connectivityManager.registerDefaultNetworkCallback(networkCallback);
    }

    private boolean isDeeplinkHandled = false; // Add this flag

    private void handleDeeplink(Intent intent) {
        Uri data = intent.getData();
        if (data != null) {
            if ("somtoday".equals(data.getScheme())) {
                String code = data.getQueryParameter("code");
                String iss = data.getQueryParameter("iss");
                String state = data.getQueryParameter("state");

                Log.d("Deeplink", "Code: " + code + " Iss: " + iss + " State: " + state);

                if (code != null && iss != null && state != null) {
                    String redirectUrl = baseURL + "/Koppelingen/Somtoday/Callback?code=" + code + "&iss=" + iss + "&state=" + state;

                    // Log the URL being loaded in WebView
                    Log.d("Deeplink", "Redirecting to URL: " + redirectUrl);

                    mWebView.loadUrl(redirectUrl);
                    isDeeplinkHandled = true; // Ensure the deeplink is marked as handled
                }
            }
        }
    }

    @Override
    protected void onNewIntent(Intent intent) {
        super.onNewIntent(intent);
        setIntent(intent);
        handleDeeplink(intent); // Ensure this is called here too
    }

    private boolean isNetworkAvailable() {
        ConnectivityManager connectivityManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
        Network nw = connectivityManager.getActiveNetwork();
        if (nw == null) return false;
        NetworkCapabilities actNw = connectivityManager.getNetworkCapabilities(nw);
        return actNw != null && (actNw.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) || actNw.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) || actNw.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET) || actNw.hasTransport(NetworkCapabilities.TRANSPORT_VPN));
    }

    private static class HelloWebViewClient extends WebViewClient {

        private String baseURL;

        public HelloWebViewClient(String baseURL) {
            this.baseURL = baseURL;
        }

        @Override
        public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
            Uri url = request.getUrl();

            // Check if the URL scheme is 'somtoday'
            if ("somtoday".equals(url.getScheme())) {
                String code = url.getQueryParameter("code");
                String iss = url.getQueryParameter("iss");
                String state = url.getQueryParameter("state");

                String redirectUrl = baseURL + "/Koppelingen/Somtoday/Callback?code=" + code + "&iss=" + iss + "&state=" + state;

                // Log the URL being loaded
                Log.d("WebViewClient", "Loading deeplink URL: " + redirectUrl);

                // Load the new URL in the WebView
                view.loadUrl(redirectUrl);
                return true; // Indicate that you've handled the URL
            }

            // For other URLs, let the WebView handle them
            view.loadUrl(url.toString());
            return true;
        }
    }




    @Override
    public void onBackPressed() {
        if (mWebView.canGoBack()) {
            mWebView.goBack();
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        if (networkCallback != null) {
            ConnectivityManager connectivityManager = (ConnectivityManager) getSystemService(Context.CONNECTIVITY_SERVICE);
            connectivityManager.unregisterNetworkCallback(networkCallback);
        }
    }

    /*STATUS BAR CHANGING*/
    // JavaScript interface class to handle the theme change
    private class WebAppInterface {
        @android.webkit.JavascriptInterface
        public void changeStatusBarColor(final String theme) {
            // Ensure this code runs on the main thread
            runOnUiThread(new Runnable() {
                @Override
                public void run() {
                    switch (theme) {
                        default:
                        case "light":
                        case "blue":
                            setStatusBarColor("#f8f9fa");
                            break;
                        case "dark":
                        case "red":
                            setStatusBarColor("#1a1a1a");
                            break;
                        case "pink":
                            setStatusBarColor("#ffebf2");
                            break;
                    }
                }
            });
        }
    }

    private void setStatusBarColor(String color) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            Window window = getWindow();
            window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS);
            window.setStatusBarColor(Color.parseColor(color));
        }
    }
}