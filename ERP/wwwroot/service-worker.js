// Transport ERP - PWA Service Worker
const CACHE_NAME = 'transport-erp-v1';

self.addEventListener('install', (event) => {
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    event.waitUntil(clients.claim());
});

self.addEventListener('fetch', (event) => {
    const url = new URL(event.request.url);

    // CRITICAL: Do not intercept SignalR traffic and Blazor internal calls
    if (url.pathname.includes('_blazor') || url.pathname.includes('negotiate')) {
        return;
    }

    // CRITICAL: Let navigation requests pass through to the server for Blazor Server
    if (event.request.mode === 'navigate') {
        return;
    }

    // For other assets, just fetch normally (no caching for now to avoid issues)
    event.respondWith(fetch(event.request));
});
