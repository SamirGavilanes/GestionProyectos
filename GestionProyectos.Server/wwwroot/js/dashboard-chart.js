const dashboardChartInstances = {};

function resolveDashboardCanvas(target) {
    if (!target) return null;
    if (target instanceof HTMLCanvasElement) return target;
    if (typeof target === 'string') return document.getElementById(target);
    return null;
}

window.renderDashboardStatusChart = function (canvasRef, payload, instanceKey) {
    const key = instanceKey || 'default';
    const canvas = resolveDashboardCanvas(canvasRef);
    if (!canvas || !window.Chart) return;

    if (dashboardChartInstances[key]) {
        dashboardChartInstances[key].destroy();
        dashboardChartInstances[key] = null;
    }

    const labels = payload?.labels ?? [];
    const values = payload?.values ?? [];
    const colors = payload?.colors ?? [];

    if (labels.length === 0) return;

    dashboardChartInstances[key] = new Chart(canvas, {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data: values,
                backgroundColor: colors,
                borderWidth: 2,
                borderColor: '#ffffff',
                hoverOffset: 6
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '62%',
            plugins: {
                legend: {
                    display: false
                },
                tooltip: {
                    callbacks: {
                        label(ctx) {
                            const total = ctx.dataset.data.reduce((a, b) => a + b, 0);
                            const value = ctx.parsed ?? 0;
                            const pct = total > 0 ? Math.round((value / total) * 1000) / 10 : 0;
                            const valueLabel = Number.isInteger(value) ? String(value) : value.toFixed(2).replace(/\.?0+$/, '');
                            const unit = Number.isInteger(value) ? '' : ' h';
                            return ` ${ctx.label}: ${valueLabel}${unit} (${pct}%)`;
                        }
                    }
                }
            }
        }
    });
};

window.disposeDashboardStatusChart = function (instanceKey) {
    if (instanceKey) {
        if (dashboardChartInstances[instanceKey]) {
            dashboardChartInstances[instanceKey].destroy();
            delete dashboardChartInstances[instanceKey];
        }
        return;
    }

    Object.keys(dashboardChartInstances).forEach(key => {
        if (dashboardChartInstances[key]) {
            dashboardChartInstances[key].destroy();
        }
    });
    dashboardChartInstances = {};
};
