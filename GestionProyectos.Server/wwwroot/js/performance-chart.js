let performanceChartInstance = null;
let performanceFocusedDatasetIndex = null;
let pendingVisibleUserIds = null;

function resolveCanvas(target) {
    if (!target) return null;
    if (target instanceof HTMLCanvasElement) return target;
    if (typeof target === 'string') return document.getElementById(target);
    return null;
}

const performanceQuadrantPlugin = {
    id: 'performanceQuadrants',
    beforeDraw(chart) {
        const { ctx, chartArea, scales } = chart;
        if (!chartArea || !scales.x || !scales.y) return;

        const xZero = scales.x.getPixelForValue(0);
        const yMidValue = (scales.y.min + scales.y.max) / 2;
        const yMid = scales.y.getPixelForValue(yMidValue);
        const { left, right, top, bottom } = chartArea;

        const zones = [
            { x1: left, x2: xZero, y1: yMid, y2: bottom, fill: 'rgba(16, 185, 129, 0.12)' },
            { x1: xZero, x2: right, y1: yMid, y2: bottom, fill: 'rgba(59, 130, 246, 0.12)' },
            { x1: left, x2: xZero, y1: top, y2: yMid, fill: 'rgba(245, 158, 11, 0.14)' },
            { x1: xZero, x2: right, y1: top, y2: yMid, fill: 'rgba(239, 68, 68, 0.12)' }
        ];

        ctx.save();
        zones.forEach(zone => {
            ctx.fillStyle = zone.fill;
            ctx.fillRect(zone.x1, zone.y1, zone.x2 - zone.x1, zone.y2 - zone.y1);
        });

        ctx.strokeStyle = 'rgba(107, 114, 128, 0.5)';
        ctx.lineWidth = 1.5;
        ctx.setLineDash([5, 4]);
        ctx.beginPath();
        ctx.moveTo(xZero, top);
        ctx.lineTo(xZero, bottom);
        ctx.stroke();
        ctx.beginPath();
        ctx.moveTo(left, yMid);
        ctx.lineTo(right, yMid);
        ctx.stroke();
        ctx.setLineDash([]);
        ctx.restore();
    },
    afterDraw(chart) {
        const { ctx, chartArea, scales } = chart;
        if (!chartArea || !scales.x || !scales.y) return;

        const xZero = scales.x.getPixelForValue(0);
        const yMidValue = (scales.y.min + scales.y.max) / 2;
        const yMid = scales.y.getPixelForValue(yMidValue);
        const { left, right, top, bottom } = chartArea;
        const pad = 10;
        const quadrantWidth = (xZero - left - pad * 2);
        const maxTextWidth = Math.max(96, Math.min(200, quadrantWidth));

        const quadrants = [
            {
                title: 'Rápidas sin bugs',
                description: 'Izquierda abajo · A tiempo o antes y con pocos errores. Mejor escenario.',
                anchorX: left + pad,
                anchorY: bottom - pad,
                align: 'bottom-left',
                titleColor: 'rgba(5, 150, 105, 0.98)',
                descColor: 'rgba(4, 120, 87, 0.92)',
                bg: 'rgba(236, 253, 245, 0.92)'
            },
            {
                title: 'Demoradas limpias',
                description: 'Derecha abajo · Superaron el tiempo estimado, pero con calidad aceptable.',
                anchorX: right - pad,
                anchorY: bottom - pad,
                align: 'bottom-right',
                titleColor: 'rgba(37, 99, 235, 0.98)',
                descColor: 'rgba(29, 78, 216, 0.92)',
                bg: 'rgba(239, 246, 255, 0.92)'
            },
            {
                title: 'Rápidas con bugs',
                description: 'Izquierda arriba · Cumplieron el plazo, pero con defectos o reprocesos.',
                anchorX: left + pad,
                anchorY: top + 26,
                align: 'top-left',
                titleColor: 'rgba(217, 119, 6, 0.98)',
                descColor: 'rgba(180, 83, 9, 0.92)',
                bg: 'rgba(255, 251, 235, 0.92)'
            },
            {
                title: 'Retrasadas con bugs',
                description: 'Derecha arriba · Demora y baja calidad. Requiere atención prioritaria.',
                anchorX: right - pad,
                anchorY: top + 26,
                align: 'top-right',
                titleColor: 'rgba(220, 38, 38, 0.98)',
                descColor: 'rgba(185, 28, 28, 0.92)',
                bg: 'rgba(254, 242, 242, 0.92)'
            }
        ];

        ctx.save();
        quadrants.forEach(q => drawQuadrantLegend(ctx, q, maxTextWidth));

        ctx.font = '600 9px system-ui, sans-serif';
        ctx.fillStyle = 'rgba(107, 114, 128, 0.9)';
        ctx.textAlign = 'center';
        ctx.textBaseline = 'bottom';
        ctx.fillText('Estimación perfecta (0%)', xZero, top + 12);

        ctx.font = '400 8px system-ui, sans-serif';
        ctx.fillStyle = 'rgba(107, 114, 128, 0.75)';
        ctx.textBaseline = 'top';
        ctx.fillText('Línea horizontal: mitad de bugs · Desviación ajustada por ausentismos', (left + right) / 2, top + 14);
        ctx.restore();
    }
};

function wrapCanvasText(ctx, text, maxWidth) {
    const words = String(text).split(/\s+/);
    const lines = [];
    let line = '';
    words.forEach(word => {
        const test = line ? `${line} ${word}` : word;
        if (ctx.measureText(test).width > maxWidth && line) {
            lines.push(line);
            line = word;
        } else {
            line = test;
        }
    });
    if (line) lines.push(line);
    return lines;
}

function drawQuadrantLegend(ctx, quadrant, maxTextWidth) {
    const innerPad = 8;
    const titleLineHeight = 13;
    const descLineHeight = 11;
    const gap = 3;

    ctx.font = '600 11px system-ui, sans-serif';
    const titleWidth = ctx.measureText(quadrant.title).width;
    ctx.font = '400 9px system-ui, sans-serif';
    const descLines = wrapCanvasText(ctx, quadrant.description, maxTextWidth);
    const descWidths = descLines.map(line => ctx.measureText(line).width);
    const contentWidth = Math.min(maxTextWidth, Math.max(titleWidth, ...descWidths, 0));
    const boxWidth = contentWidth + innerPad * 2;
    const boxHeight = innerPad * 2 + titleLineHeight + gap + descLines.length * descLineHeight;

    let boxX = quadrant.anchorX;
    let boxY = quadrant.anchorY;

    if (quadrant.align.includes('right')) boxX -= boxWidth;
    if (quadrant.align.includes('bottom')) boxY -= boxHeight;

    ctx.fillStyle = quadrant.bg;
    if (typeof ctx.roundRect === 'function') {
        ctx.beginPath();
        ctx.roundRect(boxX, boxY, boxWidth, boxHeight, 6);
        ctx.fill();
    } else {
        ctx.fillRect(boxX, boxY, boxWidth, boxHeight);
    }

    const textX = quadrant.align.includes('right')
        ? boxX + boxWidth - innerPad
        : boxX + innerPad;
    let textY = quadrant.align.includes('bottom')
        ? boxY + innerPad + titleLineHeight - 4
        : boxY + innerPad + titleLineHeight - 2;

    ctx.textAlign = quadrant.align.includes('right') ? 'right' : 'left';
    ctx.textBaseline = 'top';
    ctx.font = '600 11px system-ui, sans-serif';
    ctx.fillStyle = quadrant.titleColor;
    ctx.fillText(quadrant.title, textX, textY);

    textY += titleLineHeight + gap;
    ctx.font = '400 9px system-ui, sans-serif';
    ctx.fillStyle = quadrant.descColor;
    descLines.forEach(line => {
        ctx.fillText(line, textX, textY);
        textY += descLineHeight;
    });
}

function buildPerformanceDatasets(employees, tasks) {
    return employees.map(emp => ({
        label: emp.userName,
        data: tasks
            .filter(t => t.userId === emp.userId)
            .map(t => ({
                x: t.deviationPercent,
                y: t.bugCount,
                userId: t.userId,
                taskId: t.taskId,
                taskDescription: t.taskDescription,
                projectName: t.projectName,
                plannedHours: t.plannedHours,
                actualHours: t.actualHours,
                deviationPercent: t.deviationPercent,
                bugCount: t.bugCount,
                userName: t.userName
            })),
        backgroundColor: emp.color,
        borderColor: emp.color,
        pointRadius: 8,
        pointHoverRadius: 10,
        pointBorderWidth: 3,
        pointBorderColor: '#ffffff',
        pointStyle: 'circle'
    }));
}

function computeSymmetricXRange(tasks) {
    if (!tasks || tasks.length === 0) return { min: -50, max: 50 };
    const maxAbs = Math.max(...tasks.map(t => Math.abs(t.deviationPercent)), 10);
    const padded = Math.ceil(maxAbs / 10) * 10 + 10;
    return { min: -padded, max: padded };
}

function applyPerformanceChartVisibility(visibleUserIds) {
    if (!performanceChartInstance) return false;

    const ids = new Set((visibleUserIds || []).map(id => Number(id)));
    const employeeIds = performanceChartInstance._employeeIds || [];
    const total = performanceChartInstance.data.datasets.length;

    employeeIds.forEach((empUserId, index) => {
        const visible = ids.size === 0 || ids.has(Number(empUserId));
        performanceChartInstance.setDatasetVisibility(index, visible);
    });

    performanceFocusedDatasetIndex = ids.size === 1
        ? employeeIds.findIndex(id => ids.has(Number(id)))
        : null;

    if (ids.size === total && total > 0) {
        performanceFocusedDatasetIndex = null;
    }

    performanceChartInstance.update('none');
    return true;
}

window.performanceChartIsReady = () => !!performanceChartInstance;

window.renderPerformanceChart = (canvasTarget, config) => {
    const canvas = resolveCanvas(canvasTarget);
    if (!canvas || typeof Chart === 'undefined') {
        console.warn('Performance chart: canvas or Chart.js not available.');
        return false;
    }

    if (performanceChartInstance) {
        performanceChartInstance.destroy();
        performanceChartInstance = null;
    }
    performanceFocusedDatasetIndex = null;

    const tasks = config.tasks || [];
    const visibleUserIds = config.visibleUserIds || null;
    const visibilityNotifier = config.visibilityNotifier || null;
    const xRange = computeSymmetricXRange(tasks);
    const maxBugs = Math.max(...tasks.map(t => t.bugCount), 0);
    const yMax = Math.max(Math.ceil(maxBugs * 1.25) + 1, 4);

    const datasets = buildPerformanceDatasets(config.employees, tasks);
    performanceChartInstance = new Chart(canvas.getContext('2d'), {
        type: 'scatter',
        data: { datasets },
        plugins: [performanceQuadrantPlugin],
        options: {
            responsive: true,
            maintainAspectRatio: false,
            layout: {
                padding: { top: 28, bottom: 8 }
            },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        boxWidth: 10,
                        boxHeight: 10,
                        font: { size: 11 },
                        usePointStyle: true,
                        padding: 14
                    },
                    onClick: (evt, legendItem, legend) => {
                        const chart = legend.chart;
                        const clickedIndex = legendItem.datasetIndex;
                        const employeeIds = chart._employeeIds || [];
                        const clickedUserId = employeeIds[clickedIndex];
                        if (clickedUserId == null) return;

                        const visibleIndexes = employeeIds
                            .map((_, index) => index)
                            .filter(index => chart.isDatasetVisible(index));
                        const onlyClickedVisible = visibleIndexes.length === 1 && visibleIndexes[0] === clickedIndex;

                        const nextVisibleUserIds = onlyClickedVisible
                            ? employeeIds.slice()
                            : [clickedUserId];

                        pendingVisibleUserIds = nextVisibleUserIds;
                        applyPerformanceChartVisibility(nextVisibleUserIds);

                        if (visibilityNotifier) {
                            visibilityNotifier.invokeMethodAsync('OnChartVisibilityChanged', nextVisibleUserIds);
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        title: (items) => {
                            const raw = items[0]?.raw;
                            return raw ? `Tarea #${raw.taskId}` : '';
                        },
                        label: (ctx) => {
                            const p = ctx.raw;
                            if (!p) return '';
                            return [
                                p.taskDescription,
                                `Proyecto: ${p.projectName}`,
                                `Desarrollador: ${p.userName}`,
                                `Planificado: ${p.plannedHours} h`,
                                `Real: ${p.actualHours} h`,
                                `Desviación: ${p.deviationPercent}%`,
                                `Bugs: ${p.bugCount}`
                            ];
                        }
                    }
                }
            },
            scales: {
                x: {
                    type: 'linear',
                    min: xRange.min,
                    max: xRange.max,
                    title: {
                        display: true,
                        text: 'Desviación de tiempo (%)',
                        font: { size: 12, weight: '600' }
                    },
                    ticks: {
                        callback: (value) => `${value}%`,
                        font: { size: 10 }
                    },
                    grid: { color: 'rgba(229, 231, 235, 0.6)' }
                },
                y: {
                    beginAtZero: true,
                    max: yMax,
                    title: {
                        display: true,
                        text: 'Bugs por tarea',
                        font: { size: 12, weight: '600' }
                    },
                    ticks: {
                        stepSize: 1,
                        font: { size: 10 }
                    },
                    grid: { color: 'rgba(229, 231, 235, 0.6)' }
                }
            }
        }
    });

    performanceChartInstance._employeeIds = config.employees.map(e => e.userId);

    const initialVisibleUserIds = visibleUserIds
        ?? pendingVisibleUserIds
        ?? performanceChartInstance._employeeIds;

    applyPerformanceChartVisibility(initialVisibleUserIds);
    pendingVisibleUserIds = null;
    return true;
};

window.getPerformanceChartVisibleUserIds = () => {
    if (!performanceChartInstance) {
        return pendingVisibleUserIds ? pendingVisibleUserIds.slice() : [];
    }

    const employeeIds = performanceChartInstance._employeeIds || [];
    return employeeIds.filter((_, index) => performanceChartInstance.isDatasetVisible(index));
};

window.setPerformanceChartVisibility = (visibleUserIds) => {
    pendingVisibleUserIds = (visibleUserIds || []).map(id => Number(id));

    if (!performanceChartInstance) {
        return;
    }

    applyPerformanceChartVisibility(pendingVisibleUserIds);
    pendingVisibleUserIds = null;
};

window.disposePerformanceChart = () => {
    if (performanceChartInstance) {
        performanceChartInstance.destroy();
        performanceChartInstance = null;
    }
    performanceFocusedDatasetIndex = null;
    pendingVisibleUserIds = null;
};
