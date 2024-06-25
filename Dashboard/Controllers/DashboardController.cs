using BusinessObject;
using BusinessObject.Models;
using DataAccess;
using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.AccountRepo;
using System.Security.Principal;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAccountRepository _accRepository;
        public DashboardController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;
        }

        public IActionResult Index()
        {
            //var age1 = await _accRepository.statisticsItems(2024, "5-10");
            //var age2 = await _accRepository.statisticsItems(2024, "10-15");
            //var age3 = await _accRepository.statisticsItems(2024, "15-20");
            //var age4 = await _accRepository.statisticsItems(2024, "20-25");

            //ViewData["age1Total"] = age1.Select(x => x.Total).ToList();
            //ViewData["age1Male"] = age1.Select(x => x.Male).ToList();
            //ViewData["age1Female"] = age1.Select(x => x.Female).ToList();

            //ViewData["age2Total"] = age2.Select(x => x.Total).ToList();
            //ViewData["age2Male"] = age2.Select(x => x.Male).ToList();
            //ViewData["age2Female"] = age2.Select(x => x.Female).ToList();

            //ViewData["age3Total"] = age3.Select(x => x.Total).ToList();
            //ViewData["age3Male"] = age3.Select(x => x.Male).ToList();
            //ViewData["age3Female"] = age3.Select(x => x.Female).ToList();

            //ViewData["age4Total"] = age4.Select(x => x.Total).ToList();
            //ViewData["age4Male"] = age4.Select(x => x.Male).ToList();
            //ViewData["age4Female"] = age4.Select(x => x.Female).ToList();

            //ViewData["age4"] = age4;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountData(string yearSelect, int customYears, string typeSelect)
        {
            var obj = new List<StatisticsItem>();

            if (yearSelect != "custom")
            {
                obj = await _accRepository.statisticsItems(int.Parse(yearSelect), "0-100");
            }
            else if (customYears != 0 && customYears >0)
            {
                obj = await _accRepository.statisticsItems(customYears, "0-100");
            }
            if (typeSelect =="month")
            {
                var month = new List<string>() { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                var total = new List<string>();
                var male = new List<string>();
                var female = new List<string>();

                foreach (var item in obj)
                {
                    total.Add(item.Total.ToString());
                    male.Add(item.Male.ToString());
                    female.Add(item.Female.ToString());
                }

                List<List<string>> data = new List<List<string>>() { month, total, male, female };
                return Ok(data);
            }
            var q1Total = obj[0].Total + obj[1].Total + obj[2].Total;
            var q1Male = obj[0].Male + obj[1].Male + obj[2].Male;
            var q1Female = obj[0].Male + obj[1].Male + obj[2].Male;

            var q2Total = obj[3].Total + obj[4].Total + obj[5].Total;
            var q2Male = obj[3].Male + obj[4].Male + obj[5].Male;
            var q2Female = obj[3].Female + obj[4].Female + obj[5].Female;

            var q3Total = obj[6].Total + obj[7].Total + obj[8].Total;
            var q3Male = obj[6].Male + obj[7].Male + obj[8].Male;
            var q3Female = obj[6].Female + obj[7].Female + obj[8].Female;

            var q4Total = obj[9].Total + obj[10].Total + obj[11].Total;
            var q4Male = obj[9].Male + obj[10].Male + obj[11].Male;
            var q4Female = obj[9].Female + obj[10].Female + obj[11].Female;

            var labels = new List<string>() { "Q1", "Q2", "Q3", "Q4" };
            var qtotal = new List<string>() { q1Total.ToString(), q2Total.ToString(), q3Total.ToString(), q4Total.ToString() };
            var qmale = new List<string>() { q1Male.ToString(), q2Male.ToString(), q3Male.ToString(), q4Male.ToString() };
            var qfemale = new List<string>() { q1Female.ToString(), q2Female.ToString(), q3Female.ToString(), q4Female.ToString() };

            List<List<string>> qdata = new List<List<string>>() { labels, qtotal, qmale, qfemale };
            return Ok(qdata);

            //var list = obj.Select(item => new
            //{
            //    month = item.Month,
            //    total = item.Total,
            //    male = item.Male,
            //    female = item.Female,
            //});


        }
    }
}
