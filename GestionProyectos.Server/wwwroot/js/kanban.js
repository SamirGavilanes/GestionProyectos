(function () {
    document.addEventListener('dragstart', function (e) {
        var handle = e.target.closest('[data-kanban-drag-handle]');
        var card = e.target.closest('[data-kanban-card]');
        if (!handle || !card || !e.dataTransfer) return;

        if (e.target.closest('button, a, input, select, textarea, [data-kanban-no-drag]')) {
            e.preventDefault();
            return;
        }

        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', 'kanban-item');
    });
})();
