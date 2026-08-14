const projectMetricsChartInstances = {};

function resolveProjectMetricsCanvas(target) {
    if (!target) return null;
    if (target instanceof HTMLCanvasElement) return target;
    if (typeof target === 'string') return document.getElementById(target);
    return null;
}

function destroyProjectMetricsChart(instanceKey) {
    if (!instanceKey || !projectMetricsChartInstances[instanceKey]) return;
    projectMetricsChartInstances[instanceKey].destroy();
    delete projectMetricsChartInstances[instanceKey];
}

window.renderProjectProgressGauge = function (canvasTarget, config, instanceKey) {
    const key = instanceKey || 'project-progress-gauge';
    const canvas = resolveProjectMetricsCanvas(canvasTarget);
    if (!canvas || !window.Chart) return;

    destroyProjectMetricsChart(key);

    const value = Math.min(100, Math.max(0, Number(config?.percent ?? 0)));
    const remainder = Math.max(0, 100 - value);

    projectMetricsChartInstances[key] = new Chart(canvas, {
        type: 'doughnut',
        data: {
            datasets: [{
                data: [value, remainder],
                backgroundColor: ['#7c3aed', '#f3f4f6'],
                borderWidth: 0,
                borderRadius: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            rotation: -90,
            circumference: 180,
            cutout: '72%',
            plugins: {
                legend: { display: false },
                tooltip: { enabled: false }
            },
            animation: {
                duration: 450
            }
        }
    });
};

window.renderProjectTaskStatusBarChart = function (canvasTarget, config, instanceKey) {
    const key = instanceKey || 'project-task-status-bar';
    const canvas = resolveProjectMetricsCanvas(canvasTarget);
    if (!canvas || !window.Chart) return;

    destroyProjectMetricsChart(key);

    const labels = config?.labels ?? [];
    const values = config?.values ?? [];
    const colors = config?.colors ?? [];

    if (labels.length === 0) return;

    projectMetricsChartInstances[key] = new Chart(canvas, {
        type: 'bar',
        data: {
            labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderRadius: 4,
                maxBarThickness: 48
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label(ctx) {
                            const value = ctx.parsed?.y ?? 0;
                            const total = values.reduce((sum, item) => sum + item, 0);
                            const pct = total > 0 ? Math.round((value / total) * 100) : 0;
                            return ` ${value} (${pct}%)`;
                        }
                    }
                }
            },
            scales: {
                x: {
                    grid: { display: false },
                    ticks: {
                        font: { size: 10 },
                        maxRotation: 0,
                        autoSkip: false
                    }
                },
                y: {
                    beginAtZero: true,
                    ticks: {
                        stepSize: 1,
                        font: { size: 10 },
                        precision: 0
                    },
                    grid: { color: '#f3f4f6' }
                }
            }
        }
    });
};

window.disposeProjectMetricsCharts = function (instanceKey) {
    if (instanceKey) {
        destroyProjectMetricsChart(instanceKey);
        return;
    }

    Object.keys(projectMetricsChartInstances).forEach(key => destroyProjectMetricsChart(key));
};
