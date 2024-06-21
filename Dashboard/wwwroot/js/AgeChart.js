const ctx = document.getElementById('userChart').getContext('2d');
let userChart;


var dataContainer = document.getElementById('dataContainer');

var age1Total = JSON.parse(dataContainer.getAttribute('data-age1-Total'));
var age1Male = JSON.parse(dataContainer.getAttribute('data-age1-Male'));
var age1Female = JSON.parse(dataContainer.getAttribute('data-age1-Female'));

var age2Total = JSON.parse(dataContainer.getAttribute('data-age2-Total'));
var age2Male = JSON.parse(dataContainer.getAttribute('data-age2-Male'));
var age2Female = JSON.parse(dataContainer.getAttribute('data-age2-Female'));

var age3Total = JSON.parse(dataContainer.getAttribute('data-age3-Total'));
var age3Male = JSON.parse(dataContainer.getAttribute('data-age3-Male'));
var age3Female = JSON.parse(dataContainer.getAttribute('data-age3-Female'));

var age4Total = JSON.parse(dataContainer.getAttribute('data-age4-Total'));
var age4Male = JSON.parse(dataContainer.getAttribute('data-age4-Male'));
var age4Female = JSON.parse(dataContainer.getAttribute('data-age4-Female'));


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
    },
    '2024': {
        '5-10': {
            'Total': age1Total,
            'Male': age1Male,
            'Female': age1Female
        },
        '10-15': {
            'Total': age2Total,
            'Male': age2Male,
            'Female': age2Female
        },
        '15-20': {
            'Total': age3Total,
            'Male': age3Male,
            'Female': age3Female
        },
        '20-25': {
            'Total': age4Total,
            'Male': age4Male,
            'Female': age4Female
        },
    }
};


function createChart(dataTotal, dataMale, dataFemale, labels) {
    if (userChart) {
        userChart.destroy();
    }
    userChart = new Chart(ctx, {
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



function updateChart() {
    const year = document.getElementById('yearSelect').value;
    const ageRange = document.getElementById('ageSelect').value;
    const dataTotal = dataByYearAndAge[year][ageRange]['Total'];
    const dataMale = dataByYearAndAge[year][ageRange]['Male'];
    const dataFemale = dataByYearAndAge[year][ageRange]['Female'];

    // Cập nhật dữ liệu của biểu đồ hiện tại mà không tạo lại biểu đồ mới
    userChart.data.datasets[0].data = dataTotal;
    userChart.data.datasets[1].data = dataMale;
    userChart.data.datasets[2].data = dataFemale;
    userChart.update();
}

// Tải dữ liệu mặc định khi trang được tải lần đầu
document.addEventListener('DOMContentLoaded', function () {
    const year = document.getElementById('yearSelect').value;
    const ageRange = document.getElementById('ageSelect').value;
    const dataTotal = dataByYearAndAge[year][ageRange]['Total'];
    const dataMale = dataByYearAndAge[year][ageRange]['Male'];
    const dataFemale = dataByYearAndAge[year][ageRange]['Female'];

    createChart(dataTotal, dataMale, dataFemale, labels);
});
