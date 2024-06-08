using BusinessObject.Models;
using Google.Api.Gax;
using Microsoft.AspNetCore.Mvc;
using Repository.General_FeedbackRepo;
using Repository.VocabularyRepo;
using System.Net.Sockets;

namespace Dashboard.Controllers
{
    public class VocabularyController : Controller
    {
        private readonly IVocabularyRepository _vocabularyRepository;

        public VocabularyController(IVocabularyRepository vocabularyRepository)
        {
            _vocabularyRepository = vocabularyRepository;

        }
        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            try
            {
                var obj = await _vocabularyRepository.GetAll();

                var list = obj.Select(item => new
                {
                    id = item.Id,
                    english = item.English,
                    korean = item.Korean,
                    vietnamese = item.Vietnamese,
                    description = item.Example_EN,
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

            var voca = await _vocabularyRepository.GetAll();
            return View(voca);
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

                var voca = await _vocabularyRepository.GetById(id);
                var img = await _vocabularyRepository.GetFruitImg(id);

                if (voca != null)
                {
                    ViewData["FruitImg"] = voca.Fruits_img;
                    return View(voca);
                }

                return RedirectToAction("Index", "Account");
            }
            catch (Exception ex)
            {
                // Handle exceptions
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        public IActionResult Add()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Vocabulary? voca, IFormFile imgFile, IFormFile voiceVieFile, IFormFile voiceEngFile, IFormFile voiceKorFile)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int newid = await _vocabularyRepository.GenerateNewId();
            var fruitImg = await _vocabularyRepository.AddFruitImg(newid, voca.English, imgFile);
            var voiceVie = await _vocabularyRepository.AddVoiceVie(newid, voca.Vietnamese, voiceVieFile);
            var voiceEng = await _vocabularyRepository.AddVoiceEng(newid, voca.English, voiceEngFile);
            var voiceKor = await _vocabularyRepository.AddVoiceKor(newid, voca.Korean, voiceKorFile);
            if (ModelState.IsValid)
            {
                try
                {
                    Vocabulary vo = new Vocabulary
                    {
                        Id= newid,
                        Fruits_img=fruitImg,
                        Vietnamese= voca.Vietnamese,
                        English= voca.English,
                        Korean= voca.Korean,
                        Spelling= voca.Spelling,
                        Voice_EN= voiceEng,
                        Voice_KR= voiceKor,
                        Voice_VN= voiceVie,
                        Example_VN= voca.Example_VN,
                        Example_EN= voca.Example_EN,
                        Example_KR= voca.Example_KR,
                        Status=1
                    };
                    await _vocabularyRepository.AddVoca(vo);
                    return RedirectToAction("Index");
                }
                catch (HttpRequestException ex)
                {
                    ModelState.AddModelError("", $"HttpRequestException: {ex.Message}");
                    // Log or handle the exception
                    //Console.WriteLine($"HttpRequestException: {ex.Message}");
                }
                catch (SocketException ex)
                {
                    ModelState.AddModelError("", $"SocketException: {ex.Message}");
                    // Log or handle the exception
                    //Console.WriteLine($"SocketException: {ex.Message}");
                }
            }
          return View(voca);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var obj = await _vocabularyRepository.GetById(id);
            ViewData["FruitImg"] = obj.Fruits_img;
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Vocabulary? voca, IFormFile imgFile, IFormFile voiceVieFile, IFormFile voiceEngFile, IFormFile voiceKorFile)
        {
            var accountId = HttpContext.Session.GetInt32("Id");

            if (accountId == null)
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Authentication");
            }

            var fruitImg = await _vocabularyRepository.AddFruitImg(voca.Id, voca.English, imgFile);
            var voiceVie = await _vocabularyRepository.AddVoiceVie(voca.Id, voca.Vietnamese, voiceVieFile);
            var voiceEng = await _vocabularyRepository.AddVoiceEng(voca.Id, voca.English, voiceEngFile);
            var voiceKor = await _vocabularyRepository.AddVoiceKor(voca.Id, voca.Korean, voiceKorFile);
            if (ModelState.IsValid)
            {
                Vocabulary vo = new Vocabulary
                {
                    Id= voca.Id,
                    Fruits_img=fruitImg,
                    Vietnamese= voca.Vietnamese,
                    English= voca.English,
                    Korean= voca.Korean,
                    Spelling= voca.Spelling,
                    Voice_EN= voiceEng,
                    Voice_KR= voiceKor,
                    Voice_VN= voiceVie,
                    Example_VN= voca.Example_VN,
                    Example_EN= voca.Example_EN,
                    Example_KR= voca.Example_KR,
                    Status=1
                };

                await _vocabularyRepository.UpdateVoca(vo);
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


            [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                var obj = await _vocabularyRepository.GetById(id);

                if (obj != null)
                {
                    obj.Status = 2;

                    await _vocabularyRepository.DeleteVoca(obj);
                    return RedirectToAction("Index");
                }
                return View("Error");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

        }
    }
}
