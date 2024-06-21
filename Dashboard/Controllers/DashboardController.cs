using BusinessObject;
using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Repository.AccountRepo;

namespace Dashboard.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAccountRepository _accRepository;
        public DashboardController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;
        }

        public async Task<IActionResult> Index()
        {
            var age1 = await _accRepository.statisticsItems(2024, "5-10");
            var age2 = await _accRepository.statisticsItems(2024, "10-15");
            var age3 = await _accRepository.statisticsItems(2024, "15-20");
            var age4 = await _accRepository.statisticsItems(2024, "20-25");

            ViewData["age1Total"] = age1.Select(x => x.Total).ToList();
            ViewData["age1Male"] = age1.Select(x => x.MaleCount).ToList();
            ViewData["age1Female"] = age1.Select(x => x.FemaleCount).ToList();

            ViewData["age2Total"] = age2.Select(x => x.Total).ToList();
            ViewData["age2Male"] = age2.Select(x => x.MaleCount).ToList();
            ViewData["age2Female"] = age2.Select(x => x.FemaleCount).ToList();

            ViewData["age3Total"] = age3.Select(x => x.Total).ToList();
            ViewData["age3Male"] = age3.Select(x => x.MaleCount).ToList();
            ViewData["age3Female"] = age3.Select(x => x.FemaleCount).ToList();

            ViewData["age4Total"] = age4.Select(x => x.Total).ToList();
            ViewData["age4Male"] = age4.Select(x => x.MaleCount).ToList();
            ViewData["age4Female"] = age4.Select(x => x.FemaleCount).ToList();

            return View();
        }

    }
}
