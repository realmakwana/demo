// Theme Management
window.setThemeColor = function (primary) {
    const root = document.documentElement;
    root.style.setProperty('--primary-color', primary);

    // Save to localStorage
    localStorage.setItem('theme-color', primary);
};

// Load saved theme color on page load
window.loadSavedTheme = function () {
    const primary = localStorage.getItem('theme-color');

    if (primary) {
        const root = document.documentElement;
        root.style.setProperty('--primary-color', primary);
    }
};

// Call on page load
document.addEventListener('DOMContentLoaded', function () {
    window.loadSavedTheme();
});
