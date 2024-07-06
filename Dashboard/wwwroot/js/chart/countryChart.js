
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
        if (customYears < 1900 || customYears > 3000) {
            document.getElementById('checkCustomYear').innerHTML = "Enter the year range from 1900 to 3000";
            return;
        }
        document.getElementById('checkCustomYear').innerHTML = "";
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
function createChart(datas) {
    const ctx = document.getElementById('myChart').getContext('2d');
    if (chart) {
        chart.destroy();
    }
    chart = new Chart(ctx, {
        type: 'bar',
        data: datas,
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

    const response = await fetch('/Dashboard/GetAccountDataCountry', {
        method: 'POST',
        body: formData
    });
    const data = await response.json();
    const countries = data[0];
    const accounts = data[1];

    const accountNumbers = accounts.map(Number);
    const datas = {
        labels: ['2023'], // Chỉ một nhãn duy nhất
        datasets: countries.map((country, index) => ({
            label: country,
            data: [accountNumbers[index]], // Số lượng tài khoản
            backgroundColor: getRandomColor(),
            borderColor: getRandomColor().replace('0.2', '1'), // Đổi độ mờ từ 0.2 sang 1 để lấy màu viền đậm hơn
            borderWidth: 1
        }))
    };

    createChart(datas);
});
