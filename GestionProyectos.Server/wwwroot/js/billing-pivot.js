(function () {
    function getContainer(target) {
        if (!target) return null;
        if (typeof target === 'string') return document.getElementById(target);
        if (target instanceof HTMLElement) return target;
        return null;
    }

    function parseRecords(records) {
        if (typeof records === 'string') {
            try {
                return JSON.parse(records);
            } catch (error) {
                console.error('Billing pivot: invalid JSON payload.', error);
                return [];
            }
        }
        return Array.isArray(records) ? records : [];
    }

    function toTwoDecimals(value) {
        return Math.round(value * 100) / 100;
    }

    function normalizeRecords(records) {
        return parseRecords(records).map(function (row) {
            const horas = Number(row.Horas);
            const avance = Number(row.Avance);
            return {
                Fecha: row.Fecha == null ? '' : String(row.Fecha),
                'Tipo de hora': row['Tipo de hora'] == null ? '' : String(row['Tipo de hora']),
                Proyecto: row.Proyecto == null ? '' : String(row.Proyecto),
                Requerimiento: row.Requerimiento == null ? '' : String(row.Requerimiento),
                Cliente: row.Cliente == null ? '' : String(row.Cliente),
                Empresa: row.Empresa == null ? '' : String(row.Empresa),
                'Miembro de equipo': row['Miembro de equipo'] == null ? '' : String(row['Miembro de equipo']),
                Tarea: row.Tarea == null ? '' : String(row.Tarea),
                Horas: Number.isFinite(horas) ? toTwoDecimals(horas) : 0,
                Avance: Number.isFinite(avance) ? toTwoDecimals(avance) : 0
            };
        });
    }

    function hasPivotGrid(container) {
        return !!container.querySelector('.pvtTable, .pvtUi, .billing-pivot-layout');
    }

    function showMessage(container, message, isError) {
        container.innerHTML =
            '<div class="billing-pivot-empty">' +
            '<p class="' + (isError ? 'billing-pivot-empty--error' : 'billing-pivot-empty--info') + '">' + message + '</p>' +
            '</div>';
    }

    function compactPivotTableHeaders(container) {
        const table = container.querySelector('table.pvtTable');
        if (!table) return;

        table.querySelectorAll('thead th.pvtAxisLabel').forEach(function (th) {
            th.classList.add('billing-pivot-axis-label');
        });

        table.querySelectorAll('thead th.pvtColLabel').forEach(function (th) {
            th.classList.add('billing-pivot-col-label');
        });

        table.querySelectorAll('tbody th.pvtRowLabel').forEach(function (th) {
            th.classList.add('billing-pivot-row-label');
        });

        // Solo ocultar la celda esquina vacía; no eliminar filas de encabezado ni etiquetas con texto.
        table.querySelectorAll('tbody tr:first-child th.pvtRowLabel[rowspan]').forEach(function (th) {
            if (!th.textContent || !th.textContent.trim()) {
                th.classList.add('billing-pivot-corner-cell');
            }
        });
    }

    function syncPivotHeight(container) {
        if (!container) return;

        const shell = container.closest('.billing-pivot-shell');
        const rendererArea = container.querySelector('.pvtRendererArea');
        if (!shell || !rendererArea) return;

        const layout = container.querySelector('.billing-pivot-layout');
        const toolbar = layout
            ? layout.querySelector('.billing-pivot-toolbar')
            : container.querySelector('table.pvtUi tr:first-child');

        const shellStyles = window.getComputedStyle(shell);
        const shellPaddingY = parseFloat(shellStyles.paddingTop) + parseFloat(shellStyles.paddingBottom);
        const wrapStyles = window.getComputedStyle(container);
        const wrapPaddingY = parseFloat(wrapStyles.paddingTop) + parseFloat(wrapStyles.paddingBottom);
        const toolbarHeight = toolbar ? toolbar.offsetHeight : 0;
        const layoutGap = layout ? 4 : 0;
        const available = shell.clientHeight - toolbarHeight - wrapPaddingY - shellPaddingY - layoutGap;

        if (available > 80) {
            rendererArea.style.minHeight = available + 'px';
            rendererArea.style.height = available + 'px';
        }
    }

    function ensurePivotResizeHandler() {
        if (window.__billingPivotResizeBound) return;
        window.__billingPivotResizeBound = true;
        window.addEventListener('resize', function () {
            document.querySelectorAll('.billing-pivot-wrap').forEach(syncPivotHeight);
        });
    }

    function compactPivotLayout(container) {
        const existingLayout = container.querySelector('.billing-pivot-layout');
        const freshUi = container.querySelector('table.pvtUi');

        if (existingLayout && !freshUi) return;

        if (existingLayout) existingLayout.remove();

        const ui = container.querySelector('table.pvtUi');
        if (!ui) return;

        const rendererArea = ui.querySelector('.pvtRendererArea');
        if (!rendererArea) return;

        const rendererSelect = ui.querySelector('select.pvtRenderer');
        const rendererCell = rendererSelect ? rendererSelect.closest('td') : ui.querySelector('tr td');
        const colsAxis = ui.querySelector('.pvtAxisContainer.pvtCols');
        const valsAxis = ui.querySelector('.pvtVals');
        const rowsAxis = ui.querySelector('.pvtAxisContainer.pvtRows');
        const unusedAxes = Array.from(ui.querySelectorAll('.pvtAxisContainer')).filter(function (axis) {
            return !axis.classList.contains('pvtRows') && !axis.classList.contains('pvtCols');
        });

        const layout = document.createElement('div');
        layout.className = 'billing-pivot-layout';

        const toolbar = document.createElement('div');
        toolbar.className = 'billing-pivot-toolbar';

        if (rendererCell) {
            const rendererWrap = document.createElement('div');
            rendererWrap.className = 'billing-pivot-renderer-wrap';
            Array.from(rendererCell.childNodes).forEach(function (node) {
                rendererWrap.appendChild(node);
            });
            toolbar.appendChild(rendererWrap);
        }

        const axes = document.createElement('div');
        axes.className = 'billing-pivot-axes';

        if (valsAxis) axes.appendChild(valsAxis);
        if (colsAxis) axes.appendChild(colsAxis);
        if (rowsAxis) axes.appendChild(rowsAxis);
        unusedAxes.forEach(function (axis) {
            axes.appendChild(axis);
        });

        toolbar.appendChild(axes);
        layout.appendChild(toolbar);
        layout.appendChild(rendererArea);
        ui.replaceWith(layout);
    }

    function enhancePivotUi(container) {
        if (!container) return;

        compactPivotLayout(container);
        ensurePivotResizeHandler();

        container.querySelectorAll('select').forEach(function (select) {
            select.classList.add('billing-pivot-select');
        });

        container.querySelectorAll('.pvtAxisContainer').forEach(function (axis) {
            if (axis.classList.contains('pvtRows')) {
                axis.setAttribute('data-axis-label', 'Filas');
            } else if (axis.classList.contains('pvtCols')) {
                axis.setAttribute('data-axis-label', 'Columnas');
            } else {
                axis.setAttribute('data-axis-label', 'Campos');
            }
        });

        container.querySelectorAll('.pvtVals').forEach(function (valsAxis) {
            valsAxis.setAttribute('data-axis-label', 'Valores');
        });

        const table = container.querySelector('table.pvtTable');
        if (table) {
            table.classList.add('billing-pivot-table');
        }

        const rendererArea = container.querySelector('.pvtRendererArea');
        if (rendererArea) {
            rendererArea.classList.add('billing-pivot-renderer-area');
        }

        compactPivotTableHeaders(container);
        syncPivotHeight(container);
    }

    window.renderBillingPivot = function (containerTarget, records) {
        const container = getContainer(containerTarget);
        if (!container) return;

        window.disposeBillingPivot(container.id || containerTarget);

        const normalized = normalizeRecords(records);
        if (normalized.length === 0) {
            showMessage(container, 'No hay datos para el pivot.', false);
            return;
        }

        if (typeof jQuery === 'undefined' || !jQuery.fn || !jQuery.fn.pivotUI) {
            showMessage(container, 'No se pudo cargar PivotTable.js.', true);
            return;
        }

        container.innerHTML = '';

        try {
            jQuery(container).pivotUI(normalized, {
                rows: ['Proyecto'],
                cols: ['Tipo de hora'],
                vals: ['Horas'],
                aggregatorName: 'Suma',
                rendererName: 'Tabla',
                unusedAttrsVertical: false,
                menuLimit: 500,
                hiddenAttributes: ['Avance'],
                onRefresh: function () {
                    enhancePivotUi(container);
                }
            }, true, 'es');

            enhancePivotUi(container);
        } catch (error) {
            console.error('Billing pivot render failed:', error);
            showMessage(container, 'No se pudo renderizar el pivot.', true);
        }
    };

    window.syncBillingPivotHeight = function (containerTarget) {
        syncPivotHeight(getContainer(containerTarget));
    };

    window.disposeBillingPivot = function (containerTarget) {
        const container = getContainer(containerTarget);
        if (!container) return;

        if (typeof jQuery !== 'undefined') {
            jQuery(container).empty();
        } else {
            container.innerHTML = '';
        }
    };

    window.billingPivotIsEmpty = function (containerTarget) {
        const container = getContainer(containerTarget);
        if (!container) return true;
        if (container.children.length === 0) return true;
        return !hasPivotGrid(container);
    };
})();
