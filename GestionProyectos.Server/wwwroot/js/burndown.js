let burndownChartInstance = null;

function resolveCanvas(target) {
    if (!target) return null;
    if (target instanceof HTMLCanvasElement) return target;
    if (typeof target === 'string') return document.getElementById(target);
    return null;
}

window.renderBurndownChart = (canvasTarget, config) => {
    const canvas = resolveCanvas(canvasTarget);
    if (!canvas || typeof Chart === 'undefined') return;

    if (burndownChartInstance) {
        burndownChartInstance.destroy();
        burndownChartInstance = null;
    }

    burndownChartInstance = new Chart(canvas.getContext('2d'), {
        type: 'line',
        data: {
            labels: config.labels,
            datasets: [
                {
                    label: 'Estimado',
                    data: config.estimated,
                    borderColor: '#2563eb',
                    backgroundColor: 'rgba(37, 99, 235, 0.12)',
                    borderWidth: 2.5,
                    pointRadius: 2,
                    pointHoverRadius: 4,
                    tension: 0.2,
                    fill: true
                },
                {
                    label: 'Real',
                    data: config.actual,
                    borderColor: '#4F4F4F',
                    backgroundColor: 'rgba(112 , 112, 112, 0.1)',
                    borderWidth: 2.5,
                    pointRadius: 2,
                    pointHoverRadius: 4,
                    tension: 0.2,
                    fill: true
                },
                {
                    label: 'Óptimo (-10%)',
                    data: config.optimal,
                    borderColor: '#00CC4F',
                    borderWidth: 1.5,
                    borderDash: [5, 5],
                    pointRadius: 0,
                    tension: 0.2,
                    fill: false
                },
                {
                    label: 'Límite problema (+15%)',
                    data: config.problemLimit,
                    borderColor: '#ef4444',
                    borderWidth: 1.5,
                    borderDash: [5, 5],
                    pointRadius: 0,
                    tension: 0.2,
                    fill: false
                }
            ]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            interaction: { mode: 'index', intersect: false },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: { boxWidth: 12, font: { size: 11 } }
                },
                tooltip: {
                    callbacks: {
                        label: (ctx) => `${ctx.dataset.label}: ${ctx.parsed.y} h`
                    }
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Horas restantes' },
                    ticks: { font: { size: 11 } }
                },
                x: {
                    ticks: {
                        maxRotation: 45,
                        minRotation: 0,
                        font: { size: 10 },
                        autoSkip: true,
                        maxTicksLimit: 14
                    }
                }
            }
        }
    });
};

window.disposeBurndownChart = () => {
    if (burndownChartInstance) {
        burndownChartInstance.destroy();
        burndownChartInstance = null;
    }
};
