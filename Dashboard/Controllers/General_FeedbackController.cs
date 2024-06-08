using Microsoft.AspNetCore.Mvc;
using Repository.AccountRepo;
using Repository.Feedback_VocaRepo;
using Repository.General_FeedbackRepo;

namespace Dashboard.Controllers
{
    public class General_FeedbackController : Controller
    {
        private readonly IGeneral_FeedbackRepository _general_FeedbackRepository;
        private readonly IAccountRepository _accountRepository;

        public General_FeedbackController(IGeneral_FeedbackRepository general_FeedbackRepository, IAccountRepository accountRepository)
        {
            _general_FeedbackRepository = general_FeedbackRepository;
            _accountRepository = accountRepository;

        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            try
            {
                var obj = await _general_FeedbackRepository.GetAll();
                var account = await _accountRepository.GetAll();
                var list = obj.Select(item => new
                {
                    id = item.Id,
                    email = account.FirstOrDefault(e => e.Id== item.Account_Id)?.Email,
                    type= item.Type,
                    description = item.Description,
                    created = item.Created_date,
                    status = item.Status,
                });

                return Json(new { data = list });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IActionResult> Index()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var gen_feedback = await _general_FeedbackRepository.GetAll();
            return View(gen_feedback);
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var accID = HttpContext.Session.GetInt32("Id");
                if (accID == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }

                var feed = await _general_FeedbackRepository.GetById(id);
                var acc = await _accountRepository.GetById(feed.Account_Id);
                if (feed != null)
                {
                    ViewData["Image"] = feed.Image;
                    ViewData["Email"]= acc.Email;
                    return View(feed);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
    }
}
