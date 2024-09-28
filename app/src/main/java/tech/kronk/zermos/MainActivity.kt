package tech.kronk.zermos

import android.annotation.SuppressLint
import android.app.Activity
import android.app.DownloadManager
import android.content.Context
import android.content.Intent
import android.graphics.Bitmap
import android.graphics.Color
import android.net.ConnectivityManager
import android.net.Network
import android.net.NetworkCapabilities
import android.net.Uri
import android.os.Build
import android.os.Bundle
import android.os.Environment
import android.util.Log
import android.view.View
import android.view.WindowManager
import android.webkit.*
import android.widget.Toast

class MainActivity : Activity() {
    private lateinit var mWebView: WebView
    private var networkCallback: ConnectivityManager.NetworkCallback? = null
    private val baseURL = "https://zermos.kronk.tech"
    private var isDeeplinkHandled = false
    private var isErrorPageLoaded = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        // Correct the WebView initialization with the correct ID from your layout
        mWebView = findViewById(R.id.activity_main_webview)

        setupWebView()

        // Load the base URL initially if network is available
        if (isNetworkAvailable()) {
            mWebView.loadUrl(baseURL)
        } else {
            mWebView.loadUrl("file:///android_asset/offline.html")
        }

        setupNetworkCallback()  // Ensures WebView is aware of network changes
        handleDeeplink(intent)   // In case a deeplink is triggered on startup
        checkCookiesAndSetTheme()
    }

    @SuppressLint("SetJavaScriptEnabled")
    private fun setupWebView() {
        val webSettings = mWebView.settings
        webSettings.javaScriptEnabled = true
        webSettings.domStorageEnabled = true
        webSettings.cacheMode = WebSettings.LOAD_DEFAULT  // Use the default cache mode
        mWebView.clearCache(false)  // Clear cache, but do not remove cached resources from disk
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            webSettings.mixedContentMode = WebSettings.MIXED_CONTENT_ALWAYS_ALLOW
        }

        val defaultUA = webSettings.userAgentString
        webSettings.userAgentString = "$defaultUA zermos_mobile_app"

        mWebView.webViewClient = object : WebViewClient() {
            private var retryCount = 0
            private val maxRetries = 3

            override fun onReceivedError(view: WebView?, request: WebResourceRequest?, error: WebResourceError?) {
                super.onReceivedError(view, request, error)

                // Log the error details
                Log.e("WebView Error", "Error code: ${error?.errorCode}, description: ${error?.description}")

                // Handle only main frame errors
                if (request?.isForMainFrame == true) {
                    if (!isNetworkAvailable()) {
                        view?.loadUrl("file:///android_asset/offline.html")
                    } else if (retryCount < maxRetries) {
                        retryCount++

                        // Delay retry by 2 seconds to avoid rapid retries
                        view?.postDelayed({
                            Log.d("WebView", "Retrying load after error... (attempt: $retryCount)")
                            view.reload()
                        }, 2000)
                    } else {
                        // After max retries, show an error page or message
                        view?.loadUrl("file:///android_asset/error.html")
                        retryCount = 0  // Reset retry count
                    }
                }
            }

            override fun onPageStarted(view: WebView?, url: String?, favicon: Bitmap?) {
                super.onPageStarted(view, url, favicon)
                Log.d("WebView", "Page started loading: $url")
                isErrorPageLoaded = false
            }

            override fun onPageFinished(view: WebView?, url: String?) {
                super.onPageFinished(view, url)

                Log.d("WebView", "Page finished loading: $url")
            }


            override fun shouldOverrideUrlLoading(view: WebView, request: WebResourceRequest): Boolean {
                val url = request.url.toString()

                if ("somtoday" == request.url.scheme) {
                    // handle deeplinks
                    val code = request.url.getQueryParameter("code")
                    val iss = request.url.getQueryParameter("iss")
                    val state = request.url.getQueryParameter("state")

                    val redirectUrl = "$baseURL/Koppelingen/Somtoday/Callback?code=$code&iss=$iss&state=$state"
                    Log.d("WebViewClient", "Loading deeplink URL: $redirectUrl")

                    view.loadUrl(redirectUrl)
                    return true
                }

                // Check if the error occurs after a form submission
                if (request.method == "POST") {
                    Log.d("WebViewClient", "Form resubmission detected. Reloading page.")
                    view.reload()  // Reload the page to avoid cache miss
                    return true
                }

                return false  // Let the WebView handle the URL
            }

        }

        mWebView.addJavascriptInterface(WebAppInterface(), "AndroidFunction")

        mWebView.setDownloadListener { url, userAgent, contentDisposition, mimetype, contentLength ->
            val request = DownloadManager.Request(Uri.parse(url))
            request.setMimeType(mimetype)
            request.addRequestHeader("cookie", CookieManager.getInstance().getCookie(url))
            request.addRequestHeader("User-Agent", "zermos_mobile_app")
            request.setDescription("Downloading file...")
            request.setTitle(URLUtil.guessFileName(url, contentDisposition, mimetype))
            request.setNotificationVisibility(DownloadManager.Request.VISIBILITY_VISIBLE_NOTIFY_COMPLETED)
            request.setDestinationInExternalPublicDir(Environment.DIRECTORY_DOWNLOADS, URLUtil.guessFileName(url, contentDisposition, mimetype))
            val dm = getSystemService(DOWNLOAD_SERVICE) as DownloadManager
            dm.enqueue(request)
            Toast.makeText(applicationContext, "Downloading File", Toast.LENGTH_LONG).show()
        }
    }

    private fun setupNetworkCallback() {
        networkCallback = object : ConnectivityManager.NetworkCallback() {
            override fun onAvailable(network: Network) {
                runOnUiThread {
                    mWebView.loadUrl(baseURL)
                }
            }

            override fun onLost(network: Network) {
                runOnUiThread {
                    mWebView.loadUrl("file:///android_asset/offline.html")
                }
            }
        }

        val connectivityManager = getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
        connectivityManager.registerDefaultNetworkCallback(networkCallback!!)
    }

    private fun checkCookiesAndSetTheme() {
        val cookieManager = CookieManager.getInstance()
        val cookies = cookieManager.getCookie(baseURL)
        cookies?.split(";")?.forEach { cookie ->
            val parts = cookie.trim().split("=")
            if (parts.size == 2 && parts[0] == "theme") {
                setStatusBarColorForTheme(parts[1])
                return
            }
        }
        // Default theme if no cookie found
        setStatusBarColorForTheme("light")
    }

    private fun handleDeeplink(intent: Intent) {
        val data = intent.data
        if (data != null && "somtoday" == data.scheme) {
            val code = data.getQueryParameter("code")
            val iss = data.getQueryParameter("iss")
            val state = data.getQueryParameter("state")

            Log.d("Deeplink", "Code: $code Iss: $iss State: $state")

            if (code != null && iss != null && state != null) {
                val redirectUrl = "$baseURL/Koppelingen/Somtoday/Callback?code=$code&iss=$iss&state=$state"
                Log.d("Deeplink", "Redirecting to URL: $redirectUrl")
                mWebView.loadUrl(redirectUrl)
                isDeeplinkHandled = true
            }
        }
    }

    override fun onNewIntent(intent: Intent) {
        super.onNewIntent(intent)
        setIntent(intent)
        handleDeeplink(intent)
    }

    private fun isNetworkAvailable(): Boolean {
        val connectivityManager = getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
        val nw = connectivityManager.activeNetwork ?: return false
        val actNw = connectivityManager.getNetworkCapabilities(nw) ?: return false
        return when {
            actNw.hasTransport(NetworkCapabilities.TRANSPORT_WIFI) -> true
            actNw.hasTransport(NetworkCapabilities.TRANSPORT_CELLULAR) -> true
            actNw.hasTransport(NetworkCapabilities.TRANSPORT_ETHERNET) -> true
            actNw.hasTransport(NetworkCapabilities.TRANSPORT_VPN) -> true
            else -> false
        }
    }

    override fun onBackPressed() {
        if (mWebView.canGoBack()) {
            mWebView.goBack()
        } else {
            super.onBackPressed()
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        networkCallback?.let {
            val connectivityManager = getSystemService(Context.CONNECTIVITY_SERVICE) as ConnectivityManager
            connectivityManager.unregisterNetworkCallback(it)
        }
    }

    private inner class WebAppInterface {
        @JavascriptInterface
        fun changeStatusBarColor(theme: String) {
            runOnUiThread {
                setStatusBarColorForTheme(theme)
            }
        }
    }

    private fun setStatusBarColorForTheme(theme: String) {
        when (theme) {
            "light", "blue" -> setStatusBarColor("#f8f9fa", true)
            "dark", "red" -> setStatusBarColor("#1a1a1a", false)
            "pink" -> setStatusBarColor("#ffebf2", true)
            else -> setStatusBarColor("#f8f9fa", true)
        }
    }

    private fun setStatusBarColor(color: String, isLightColor: Boolean) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.LOLLIPOP) {
            window.addFlags(WindowManager.LayoutParams.FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS)
            window.statusBarColor = Color.parseColor(color)

            if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.M) {
                if (isLightColor) {
                    window.decorView.systemUiVisibility = View.SYSTEM_UI_FLAG_LIGHT_STATUS_BAR
                } else {
                    window.decorView.systemUiVisibility = 0
                }
            }
        }
    }
}