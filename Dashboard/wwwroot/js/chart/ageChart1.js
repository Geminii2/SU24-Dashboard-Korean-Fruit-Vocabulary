let chart;


async function fetchData() {
    const yearSelect = document.getElementById('yearSelect').value;
    const typeSelect = document.getElementById('typeSelect').value;
    const ageSelect = document.getElementById('ageSelect').value;
    let customYears = 0;
    if (yearSelect === 'custom') {
        customYears = parseInt(document.getElementById('customYear').value);
        if (customYears < 1900 || customYears > 3000)
        {
            document.getElementById('checkCustomYear').innerHTML = "Enter the year range from 1900 to 3000";
            return;
        }
        document.getElementById('checkCustomYear').innerHTML = "";
    }
    const formData = new FormData();
    formData.append('yearSelect', yearSelect);
    formData.append('customYears', customYears);
    formData.append('typeSelect', typeSelect);
    formData.append('ageSelect', ageSelect);

    const response = await fetch('/Dashboard/GetAccountData', {
        method: 'POST',
        body: formData
    });
    const data = await response.json();
    updateChart(data);
}
function updateChart(data) {
    const labels = data[0];
    const dataTotal = data[1];
    const dataMale = data[2];
    const dataFemale = data[3];

    // Cập nhật dữ liệu của biểu đồ hiện tại mà không tạo lại biểu đồ mới
    chart.data.labels = labels
    chart.data.datasets[0].data = dataTotal;
    chart.data.datasets[1].data = dataMale;
    chart.data.datasets[2].data = dataFemale;
    chart.update();
}
function createChart(labels, dataTotal, dataMale, dataFemale) {
    const ctx = document.getElementById('myChart').getContext('2d');
    if (chart) {
        chart.destroy();
    }
    chart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Total',
                    data: dataTotal,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Male',
                    data: dataMale,
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Female',
                    data: dataFemale,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                yAxes: [{
                    display: true,
                    ticks: {
                        beginAtZero: true,
                        min: 0,
                        precision: 0
                    }
                }]
            },
            legend: {
                position: 'bottom'
            },

        }
    });
}


// Tải dữ liệu mặc định khi trang được tải lần đầu
document.addEventListener('DOMContentLoaded', async function () {
    

    const formData = new FormData();
    formData.append('yearSelect', '2023');
    formData.append('typeSelect', 'month');
    formData.append('ageSelect', '0-99');

    const response = await fetch('/Dashboard/GetAccountData', {
        method: 'POST',
        body: formData
    });
    const data = await response.json();

    const labels = data[0];
    const dataTotal = data[1];
    const dataMale = data[2];
    const dataFemale = data[3];

    createChart(labels, dataTotal, dataMale, dataFemale);
});
