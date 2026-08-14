let tableActionsMenuScrollHandler = null;
let tableActionsMenuDotNetRef = null;

window.positionTableActionsMenu = function (trigger, menu) {
    if (!trigger || !menu) return;

    const rect = trigger.getBoundingClientRect();
    const menuWidth = menu.offsetWidth || 144;
    const menuHeight = menu.offsetHeight || menu.scrollHeight || 80;
    const gap = 2;
    const padding = 8;

    let top = rect.bottom + gap;
    let left = rect.right - menuWidth;

    if (top + menuHeight > window.innerHeight - padding) {
        top = Math.max(padding, rect.top - menuHeight - gap);
    }

    left = Math.max(padding, Math.min(left, window.innerWidth - menuWidth - padding));

    menu.style.top = top + 'px';
    menu.style.left = left + 'px';
};

window.registerTableActionsMenuListeners = function (dotNetRef) {
    window.unregisterTableActionsMenuListeners();
    tableActionsMenuDotNetRef = dotNetRef;
    tableActionsMenuScrollHandler = function () {
        if (tableActionsMenuDotNetRef) {
            tableActionsMenuDotNetRef.invokeMethodAsync('CloseFromScroll');
        }
    };
    window.addEventListener('scroll', tableActionsMenuScrollHandler, true);
    window.addEventListener('resize', tableActionsMenuScrollHandler, true);
};

window.unregisterTableActionsMenuListeners = function () {
    if (tableActionsMenuScrollHandler) {
        window.removeEventListener('scroll', tableActionsMenuScrollHandler, true);
        window.removeEventListener('resize', tableActionsMenuScrollHandler, true);
        tableActionsMenuScrollHandler = null;
    }
    tableActionsMenuDotNetRef = null;
};
