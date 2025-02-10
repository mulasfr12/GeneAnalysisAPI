window.canvasExists = (canvasId) => {
    return document.getElementById(canvasId) !== null;
};
console.log("✅ chartInterop.js has been executed!");

window.createGeneChart = (canvasId, labels, data) => {
    var ctx = document.getElementById(canvasId)?.getContext('2d');

    if (!ctx) {
        console.error(`Canvas element with ID '${canvasId}' not found.`);
        return;
    }

    // ✅ Prevent chart from infinitely re-creating
    if (window[canvasId] instanceof Chart) {
        console.log("Destroying previous chart instance...");
        window[canvasId].destroy();
    }

    console.log("Creating new chart...");
    window[canvasId] = new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Gene Expression Value',
                data: data,
                borderColor: 'rgba(75, 192, 192, 1)',
                backgroundColor: 'rgba(75, 192, 192, 0.2)',
                borderWidth: 2
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // ✅ Prevents Chart.js auto-scaling
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};
