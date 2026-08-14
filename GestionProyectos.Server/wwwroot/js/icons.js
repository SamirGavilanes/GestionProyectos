window.initLucideIcons = function () {
    var run = function () {
        if (window.lucide && typeof window.lucide.createIcons === 'function') {
            window.lucide.createIcons();
            return true;
        }

        return false;
    };

    if (run() && typeof requestAnimationFrame === 'function') {
        requestAnimationFrame(run);
        return;
    }

    if (!run()) {
        var attempts = 0;
        var retry = function () {
            if (run() || ++attempts >= 30) {
                return;
            }

            setTimeout(retry, 50);
        };

        setTimeout(retry, 50);
    }

    if (typeof requestAnimationFrame === 'function') {
        requestAnimationFrame(run);
    }
};
