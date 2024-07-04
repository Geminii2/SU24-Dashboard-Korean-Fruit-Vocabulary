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
        var checkStart = new Date(startDate);
        var checkEnd = new Date(endDate);

        if (checkStart.getTime() > checkEnd.getTime()) {
            document.getElementById('checkCustomType').innerHTML = "loi neeeeeeeeeeeeeeeee ";
            return;
        }

        document.getElementById('checkCustomType').innerHTML = '';



    }
    if (yearSelect === 'custom') {
        startDate = "01-01-" + document.getElementById('start-year').value;
        endDate = "12-31-" + document.getElementById('end-year').value;
        var checkStart = new Date(startDate);
        var checkEnd = new Date(endDate);

        if (checkStart.getTime() > checkEnd.getTime()) {
            document.getElementById('checkCustomYear').innerHTML = "loi neeeeeeeeeeeeeeeee ";
            return;
        }
        document.getElementById('checkCustomYear').innerHTML = '';
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
    formData.append('yearSelect', 'all');
    formData.append('typeSelect', 'year');
    formData.append('ageSelect', '0-99');

    const response = await fetch('/Dashboard/GetAccountDatabyYearAndCustom', {
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
