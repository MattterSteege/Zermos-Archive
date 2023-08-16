// Update 'version' if you need to refresh the cache
var version = "v0.2.0";

// A list of local resources we always want to be cached.
const PRECACHE_URLS = [
    "/", "/Offline",
    "/css/style.css", "/js/global.js", "https://kit.fontawesome.com/052f5fa8f8.js", "https://ajax.googleapis.com/ajax/libs/jquery/3.6.4/jquery.min.js", "/manifest.webmanifest", "/serviceworker.js"
];

// The install handler takes care of precaching the resources we always need.
self.addEventListener('install', event => {
    console.log("[Service Worker] Installing service worker.");
    event.waitUntil(
        caches.open(version)
            .then(cache => cache.addAll(PRECACHE_URLS))
            .catch(error => {
                console.log("[Service Worker] Error while adding files to cache: " + error);
            })
    );
});

// The activate handler takes care of cleaning up old caches.
self.addEventListener('activate', event => {
});