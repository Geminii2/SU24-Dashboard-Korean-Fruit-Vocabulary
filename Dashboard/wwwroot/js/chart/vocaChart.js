
let chart;

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


    const response = await fetch('/Dashboard/GetVocabylaryDataFailed', {
        method: 'GET',
        body: null
    });

    const data = await response.json();


    const vocas = data[0];
    const total = data[1];
    const totalNumbers = total.map(Number);

    const datas = {
        labels: ['All'], // Chỉ một nhãn duy nhất
        datasets: vocas.map((voca, index) => ({
            label: voca,
            data: [totalNumbers[index]], // Số lượng tài khoản
            backgroundColor: getRandomColor(),
            borderColor: getRandomColor().replace('0.2', '1'), // Đổi độ mờ từ 0.2 sang 1 để lấy màu viền đậm hơn
            borderWidth: 1
        }))
    };
    createChart(datas);
});
