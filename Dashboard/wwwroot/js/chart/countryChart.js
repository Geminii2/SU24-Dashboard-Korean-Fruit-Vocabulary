
let chart;

//function updateYearSelect() {
//    const yearSelect = document.getElementById('yearSelect').value;
//    const customYear = document.getElementById('customYear');
//    if (yearSelect === 'custom') {
//        customYear.style.display = 'block';
//    } else {
//        customYear.style.display = 'none';
//        fetchData();
//    }
//}

async function fetchData() {
    const yearSelect = document.getElementById('yearSelect').value;
    let year = yearSelect;
    let customYears = 0;
    if (yearSelect === 'custom') {
        customYears = parseInt(document.getElementById('customYear').value);
        year = customYears;
    }
    const formData = new FormData();
    formData.append('yearSelect', yearSelect);
    formData.append('customYears', customYears);

    const response = await fetch('/Dashboard/GetAccountDataCountry', {
        method: 'POST',
        body: formData
    });
    const data = await response.json();
    
    updateChart(data, year);
}
function updateChart(data, year) {
    const labels = [year];
    const newCountries = data[0];
    const newAccounts = data[1];

    // Cập nhật dữ liệu của biểu đồ hiện tại mà không tạo lại biểu đồ mới
    //chart.data.labels = labels;
    //chart.data.datasets[0].label = dataLabel;
    //chart.data.datasets[0].data = dataTotal;
    const newDatasets = newCountries.map((country, index) => ({
        label: country,
        data: [Number(newAccounts[index])],
        backgroundColor: getRandomColor(),
        borderColor: getRandomColor().replace('0.2', '1'),
        borderWidth: 1
    }));
    chart.data.labels = labels;
    chart.data.datasets = newDatasets

    chart.update();
}
// Hàm tạo màu ngẫu nhiên
function getRandomColor() {
    const r = Math.floor(Math.random() * 255);
    const g = Math.floor(Math.random() * 255);
    const b = Math.floor(Math.random() * 255);
    return `rgba(${r}, ${g}, ${b}, 0.2)`;
}
function createChart(labels) {
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
                    label: 'Vietnam',
                    data: [5], // Số lượng tài khoản
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Belgium',
                    data: [1],
                    backgroundColor: 'rgba(54, 162, 235, 0.2)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Guatemala',
                    data: [1],
                    backgroundColor: 'rgba(255, 206, 86, 0.2)',
                    borderColor: 'rgba(255, 206, 86, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Antigua and Barbuda',
                    data: [1],
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255, 99, 132, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Bahrain',
                    data: [1],
                    backgroundColor: 'rgba(153, 102, 255, 0.2)',
                    borderColor: 'rgba(153, 102, 255, 1)',
                    borderWidth: 1
                },
                {
                    label: 'Cambodia',
                    data: [1],
                    backgroundColor: 'rgba(255, 159, 64, 0.2)',
                    borderColor: 'rgba(255, 159, 64, 1)',
                    borderWidth: 1
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    min: 0,
                    max: 100
                }
            },
            legend: {
                position: 'bottom'
            },

        }
    });
}


//document.addEventListener('DOMContentLoaded', fetchData);

//const labels = [
//    'January', 'February', 'March', 'April', 'May', 'June',
//    'July', 'August', 'September', 'October', 'November', 'December'
//];

const dataByYearAndAge = {
    '2024': {
        'Country': [
            "Vietnam",
            "Belgium",
            "Guatemala",
            "Antigua and Barbuda",
            "Bahrain",
            "Cambodia"
        ],
        'Total': [26, 34, 7, 12, 3, 15, 42, 19, 38, 68, 58, 39]
        
    }

};

// Tải dữ liệu mặc định khi trang được tải lần đầu
document.addEventListener('DOMContentLoaded', function () {
    createChart([2024]);
});
