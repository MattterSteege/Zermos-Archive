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

// The fetch handler serves responses for same-origin resources from a cache.
// If no response is found, it populates the runtime cache with the response
// from the network before returning it to the page.
self.addEventListener('fetch', event => {
    // Skip cross-origin requests, like those for Google Analytics.
    if (event.request.url.startsWith(self.location.origin)) {
        event.respondWith(
            caches.match(event.request).then(cachedResponse => {
                if (cachedResponse) {
                    return cachedResponse;
                }

                return caches.open(version).then(cache => {
                    return fetch(event.request).then(response => {
                        // Put a copy of the response in the runtime cache.
                        cache.put(event.request, response.clone());
                        return response;
                    }).catch(error => {
                        console.log("[Service Worker]  Error fetching: " + error);
                    });
                });
            })
                .catch(error => {
                    console.log("[Service Worker]  Error in caches.match: " + error);
                })
        );
    }
});

self.addEventListener('message', function (event) {
    if (event.data === "clearCache") {
        console.log("[Service Worker] Clearing Service Worker caches");
        caches.keys().then(function (names) {
            for (var name in names) {
                console.log("[Service Worker] Clearing cache " + names[name]);
                caches.delete(names[name]);
            }
        });
    }
    if (event.data === "skipWaiting") {
        console.log('[Service Worker] skipWaiting');
        self.skipWaiting();
    }
    if (event.data === "claim") {
        console.log('[Service Worker] claim');
        clients.claim();
    }
    if (event.data === "cleanup") {
        const currentCaches = [PRECACHE, RUNTIME];
        console.log("[Service Worker] Cleaning up. New caches: " + currentCaches);
        event.waitUntil(
            caches.keys().then(cacheNames => {
                return cacheNames.filter(cacheName => !currentCaches.includes(cacheName));
            }).then(cachesToDelete => {
                console.log("[Service Worker] Deleting old caches: " + cachesToDelete);
                return Promise.all(cachesToDelete.map(cacheToDelete => {
                    console.log("[Service Worker] Deleting cache " + cacheToDelete);
                    return caches.delete(cacheToDelete)
                        .catch(error => {
                            console.log("[Service Worker] Error while deleting cache service worker: " + error);
                        })
                }));
            })
                .catch(error => {
                    console.log("[Service Worker] Error while cleaning up: " + error);
                })
        );
    }
});

self.addEventListener('statechange', function (event) {
    console.log('[Service Worker] ' + event);
});
