package tech.kronk.zermos

import android.annotation.SuppressLint
import android.app.Activity
import android.app.DownloadManager
import android.content.Context
import android.content.Intent
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
import tech.kronk.zermos.R

class MainActivity : Activity() {
    private lateinit var mWebView: WebView
    private var networkCallback: ConnectivityManager.NetworkCallback? = null
    private val baseURL = "https://zermos.kronk.tech"
    private var isDeeplinkHandled = false

    @SuppressLint("SetJavaScriptEnabled")
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        mWebView = findViewById(R.id.activity_main_webview)
        val webSettings = mWebView.settings
        webSettings.javaScriptEnabled = true
        val defaultUA = webSettings.userAgentString
        webSettings.userAgentString = "$defaultUA zermos_mobile_app"

        mWebView.webViewClient = HelloWebViewClient(baseURL)
        handleDeeplink(intent)

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

        // Check cookies for theme and set status bar color
        checkCookiesAndSetTheme()

        if (!isDeeplinkHandled && isNetworkAvailable()) {
            mWebView.loadUrl(baseURL)
        } else if (!isDeeplinkHandled) {
            mWebView.loadUrl("file:///android_asset/offline.html")
        }

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

    private class HelloWebViewClient(private val baseURL: String) : WebViewClient() {
        override fun shouldOverrideUrlLoading(view: WebView, request: WebResourceRequest): Boolean {
            val url = request.url

            if ("somtoday" == url.scheme) {
                val code = url.getQueryParameter("code")
                val iss = url.getQueryParameter("iss")
                val state = url.getQueryParameter("state")

                val redirectUrl = "$baseURL/Koppelingen/Somtoday/Callback?code=$code&iss=$iss&state=$state"

                Log.d("WebViewClient", "Loading deeplink URL: $redirectUrl")

                view.loadUrl(redirectUrl)
                return true
            }

            view.loadUrl(url.toString())
            return true
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