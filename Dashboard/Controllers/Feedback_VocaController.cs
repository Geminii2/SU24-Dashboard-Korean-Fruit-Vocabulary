using Microsoft.AspNetCore.Mvc;
using Repository.AccountRepo;
using Repository.Feedback_VocaRepo;
using Repository.VocabularyRepo;

namespace Dashboard.Controllers
{
    public class Feedback_VocaController : Controller
    {
        private readonly IFeedback_VocaRepository _feedback_VocaRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IVocabularyRepository _vocabularyRepository;

        public Feedback_VocaController(IFeedback_VocaRepository feedback_VocaRepository,
            IAccountRepository accountRepository, IVocabularyRepository vocabularyRepository)
        {
            _feedback_VocaRepository = feedback_VocaRepository;
            _accountRepository = accountRepository;
            _vocabularyRepository = vocabularyRepository;
        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            try
            {
                var obj = await _feedback_VocaRepository.GetAll();
                var account = await _accountRepository.GetAll();
                var voca = await _vocabularyRepository.GetAll();
                var list = obj.Select(item => new
                {
                    id = item.Id,
                    email = account.FirstOrDefault(e=> e.Id== item.Account_Id)?.Email,
                    vocabulary = voca.FirstOrDefault(v=> v.Id == item.Vocabulary_Id)?.English,
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

            var feedback_Vocas = await _feedback_VocaRepository.GetAll();
            return View(feedback_Vocas);
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

                var feed = await _feedback_VocaRepository.GetById(id);
                var acc = await _accountRepository.GetById(feed.Account_Id);
                var voca = await _vocabularyRepository.GetById(feed.Vocabulary_Id);
                if (feed != null)
                {
                    ViewData["Image"] = feed.Image;
                    ViewData["Email"]= acc.Email;
                    ViewData["Voca"]= voca.English;
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
