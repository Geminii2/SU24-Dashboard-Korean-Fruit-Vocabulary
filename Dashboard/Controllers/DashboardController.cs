using BusinessObject;
using BusinessObject.Models;
using DataAccess;
using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.AccountRepo;
using Repository.VocabularyRepo;
using System.Diagnostics.Metrics;
using System.Reflection.Emit;
using System.Security.Principal;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAccountRepository _accRepository;
        private readonly IVocabularyRepository _vocabularyRepository;
        public DashboardController(IAccountRepository accRepository, IVocabularyRepository vocabularyRepository)
        {
            _accRepository = accRepository;
            _vocabularyRepository = vocabularyRepository;
        }

        public IActionResult Index()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountData(string yearSelect, int customYears, string typeSelect, string ageSelect)
        {
            var obj = new List<StatisticsItem>();

            if (yearSelect != "custom")
            {
                obj = await _accRepository.statisticsAgebyMonthAndQ(int.Parse(yearSelect), ageSelect);
            }
            else if (customYears != 0 && customYears >0)
            {
                obj = await _accRepository.statisticsAgebyMonthAndQ(customYears, ageSelect);
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

            var labels = new List<string>() { "Quarter 1", "Quarter 2", "Quarter 3", "Quarter 4" };
            var qtotal = new List<string>() { q1Total.ToString(), q2Total.ToString(), q3Total.ToString(), q4Total.ToString() };
            var qmale = new List<string>() { q1Male.ToString(), q2Male.ToString(), q3Male.ToString(), q4Male.ToString() };
            var qfemale = new List<string>() { q1Female.ToString(), q2Female.ToString(), q3Female.ToString(), q4Female.ToString() };

            List<List<string>> qdata = new List<List<string>>() { labels, qtotal, qmale, qfemale };
            return Ok(qdata);

        }

        public IActionResult YearAndCustom()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GetAccountDatabyYearAndCustom(string yearSelect, string startDate, string endDate, string ageSelect, string typeSelect)
        {
            var obj = new List<StatisticsItem>();
            if (typeSelect== "year")
            {
                var date = DateTime.Now;
                if (yearSelect == "all" && startDate == null && endDate == null)
                {
                    startDate = "2020-01-01";
                    endDate = date.ToString();
                }
                if (yearSelect == "2year" && startDate == null && endDate == null)
                {
                    startDate = date.AddYears(-2).ToString();
                    endDate = date.ToString();
                }

                obj = await _accRepository.statisticsAgebyYear(ageSelect, startDate, endDate);

                var label = new List<string>();
                var total = new List<string>();
                var male = new List<string>();
                var female = new List<string>();

                foreach (var item in obj)
                {
                    label.Add(item.Label.ToString());
                    total.Add(item.Total.ToString());
                    male.Add(item.Male.ToString());
                    female.Add(item.Female.ToString());
                }
                List<List<string>> data = new List<List<string>>() { label, total, male, female };
                return Ok(data);
            }
            var obj2 = await _accRepository.statisticsAgebyCustom(ageSelect, startDate, endDate);
            var label1 = DateTime.Parse(startDate);
            var label2 = DateTime.Parse(endDate);

            obj2.Label= label1.ToString("dd/MM/yyyy") +" to "+ label2.ToString("dd/MM/yyyy");
            obj2.Total.ToString();
            obj2.Male.ToString();
            obj2.Female.ToString();

            return Ok(obj2);
        }
        public IActionResult StatisticsCountry()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAccountDataCountry(string yearSelect, int customYears)
        {
            string year = yearSelect;
            if (yearSelect== "custom")
            {
                year= customYears.ToString();
            }

            var obj = await _accRepository.CountCountry(int.Parse(year));

            var label = yearSelect;
            var country = new List<string>();
            var count = new List<string>();

            foreach (var item in obj)
            {
                country.Add(item.Country.ToString());
                count.Add(item.UserCount.ToString());
            }

            List<List<string>> data = new List<List<string>>() { country, count };
            return Ok(data);
        }

        public async Task<IActionResult> StatisticsVocaFailed()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetVocabylaryDataFailed()
        {
            var voca = await _vocabularyRepository.GetTopIncorrectVocabularies();

            var label = voca.labels.ToList();
            var total = voca.totals.ToList();

            List<List<string>> data = new List<List<string>>() { label, total };
            return Ok(data);
        }
    }
}
