let chart;

async function fetchData() {
    const yearSelect = document.getElementById('yearSelect').value;
    const typeSelect = document.getElementById('typeSelect').value;
    const ageSelect = document.getElementById('ageSelect').value;
    let startDate = "";
    let endDate = "";

    if (typeSelect === 'custom') {
        startDate = document.getElementById('start-date').value;
        endDate = document.getElementById('end-date').value;
    }
    if (yearSelect === 'custom') {
        startDate = "01-01-" + document.getElementById('start-year').value;
        endDate = "31-12-" + document.getElementById('end-year').value;
    }
    const formData = new FormData();
    formData.append('yearSelect', yearSelect);
    formData.append('startDate', startDate);
    formData.append('endDate', endDate);
    formData.append('typeSelect', typeSelect);
    formData.append('ageSelect', ageSelect);

    const response = await fetch('/Dashboard/GetAccountDatabyYearAndCustom', {
        method: 'POST',
        body: formData
    });
    const data = await response.json();
    if (typeSelect === 'custom') {
        const stringData = Object.keys(data).reduce((acc, key) => {
            acc[key] = String(data[key]);
            return acc;
        }, {});
        updateChart2(stringData);
    } else
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
function updateChart2(data) {
    const labels = [data.label];
    const dataTotal = [data.total];
    const dataMale = [data.male];
    const dataFemale = [data.female];

    // Cập nhật dữ liệu của biểu đồ hiện tại mà không tạo lại biểu đồ mới
    chart.data.labels = labels;
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
                y: {
                    beginAtZero: true
                }
            },
            legend: {
                position: 'bottom'
            },

        }
    });
}


//document.addEventListener('DOMContentLoaded', fetchData);

const labels = [
    'January', 'February', 'March', 'April', 'May', 'June',
    'July', 'August', 'September', 'October', 'November', 'December'
];

const dataByYearAndAge = {
    '2023': {
        '5-10': {
            'Total': [22, 34, 7, 11, 5, 7, 38, 27, 42, 58, 49, 35],
            'Male': [12, 19, 3, 5, 2, 3, 20, 15, 22, 30, 25, 18],
            'Female': [10, 15, 4, 6, 3, 4, 18, 12, 20, 28, 24, 17]
        },
        '10-15': {
            'Total': [26, 34, 7, 12, 3, 15, 42, 19, 38, 68, 58, 39],
            'Male': [14, 18, 4, 6, 2, 8, 22, 10, 20, 35, 30, 20],
            'Female': [12, 16, 3, 6, 1, 7, 20, 9, 18, 33, 28, 19]
        },
        '15-20': {
            'Total': [19, 29, 9, 13, 5, 9, 34, 23, 48, 62, 54, 43],
            'Male': [10, 15, 5, 7, 3, 5, 18, 12, 25, 32, 28, 22],
            'Female': [9, 14, 4, 6, 2, 4, 16, 11, 23, 30, 26, 21]
        },
        '20-25': {
            'Total': [15, 23, 6, 8, 4, 8, 28, 19, 34, 54, 48, 38],
            'Male': [8, 12, 3, 4, 2, 4, 15, 10, 18, 28, 25, 20],
            'Female': [7, 11, 3, 4, 2, 4, 13, 9, 16, 26, 23, 18]
        },
    }

};

// Tải dữ liệu mặc định khi trang được tải lần đầu
document.addEventListener('DOMContentLoaded', function () {
    const yearSelect = document.getElementById('yearSelect').value;
    const dataTotal = dataByYearAndAge[2023]['10-15']['Total'];
    const dataMale = dataByYearAndAge[2023]['10-15']['Male'];
    const dataFemale = dataByYearAndAge[2023]['10-15']['Female'];

    createChart(labels, dataTotal, dataMale, dataFemale);
});
