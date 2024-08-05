const CACHE_NAME = '${ZERMOSVERSION}';

// List of URLs to exclude from caching
const EXCEPTION_URLS = [
    '/login',
    '/logout',
    '/Koppelingen/Somtoday/Callback',
    `/Login`,
    '/Login/Callback'
];

// Function to check if a URL matches any of the exception patterns
function isException(url) {
    const urlObj = new URL(url);
    const path = urlObj.pathname;

    return EXCEPTION_URLS.some(exceptionPath => {
        // Use startsWith to match the path regardless of parameters
        return path.startsWith(exceptionPath);
    });
}

self.addEventListener('install', (event) => {
    // No specific actions needed on install
});

self.addEventListener('activate', (event) => {
    event.waitUntil(
        caches.keys().then((cacheNames) => {
            return Promise.all(
                cacheNames.map((cacheName) => {
                    if (cacheName !== CACHE_NAME) {
                        return caches.delete(cacheName);
                    }
                })
            );
        })
    );
});

self.addEventListener('fetch', (event) => {
    // Check if the URL is in the exception list
    if (isException(event.request.url)) {
        // For exception URLs, fetch from network without caching
        event.respondWith(fetch(event.request));
        return;
    }

    event.respondWith(
        fetch(event.request)
            .then((response) => {
                // Clone the response as it can only be consumed once
                const responseToCache = response.clone();

                // Check if the request is for a CSS or JS file from the same origin
                if (event.request.url.startsWith(self.location.origin) &&
                    (event.request.url.endsWith('.css') || event.request.url.endsWith('.js'))) {
                    caches.open(CACHE_NAME)
                        .then((cache) => {
                            cache.put(event.request, responseToCache);
                        });
                }

                // Check if this is the specific fetch request we want to cache
                if (event.request.headers.get('X-Requested-With') === 'XMLHttpRequest' &&
                    event.request.method === 'GET' &&
                    event.request.cache === 'no-cache') {
                    if (response.ok) {
                        caches.open(CACHE_NAME)
                            .then((cache) => {
                                cache.put(event.request, responseToCache);
                            });
                    }
                }

                return response;
            })
            .catch(() => {
                // If fetch fails (offline), try to serve from cache
                return caches.match(event.request);
            })
            .then((response) => {
                // If we have a response, return it. Otherwise, fetch from network.
                return response || fetch(event.request);
            })
    );
});