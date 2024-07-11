using BusinessObject;
using BusinessObject.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Repository.AccountRepo;
using Repository.Feedback_VocaRepo;
using Repository.VocabularyRepo;
using System.Text.RegularExpressions;
using System.Xml;
namespace Dashboard.Controllers
{
    public class Feedback_VocaController : Controller
    {
        private readonly IFeedback_VocaRepository _feedback_VocaRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IVocabularyRepository _vocabularyRepository;
        private readonly MailSettings _mailSettings;

        public Feedback_VocaController(IFeedback_VocaRepository feedback_VocaRepository,
            IAccountRepository accountRepository, IVocabularyRepository vocabularyRepository,
            IOptions<MailSettings> mailSettingsOptions)
        {
            _feedback_VocaRepository = feedback_VocaRepository;
            _accountRepository = accountRepository;
            _vocabularyRepository = vocabularyRepository;
            _mailSettings = mailSettingsOptions.Value;
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
                    email = account.FirstOrDefault(e => e.Id== item.Account_Id)?.Email,
                    vocabulary = voca.FirstOrDefault(v => v.Id == item.Vocabulary_Id)?.English,
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

        public async Task<IActionResult> Detail(string id)
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
        [HttpGet]
        public async Task<IActionResult> Reply(string id)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var fb_voca = await _feedback_VocaRepository.GetById(id);
            var acc = await _accountRepository.GetById(fb_voca.Account_Id);
            ViewData["Email"]= acc.Email;
            ViewData["Fullname"]= acc.Fullname;
            return View(fb_voca);
        }
        [HttpPost]
        public async Task<IActionResult> Reply(string id, string email, string name, string title, string body)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            await SendMail(email, name, title, body);

            var fb_voca = await _feedback_VocaRepository.GetById(id);
            fb_voca.Status=2;
            await _feedback_VocaRepository.UpdateVoca(fb_voca);
            return RedirectToAction("Index");
        }

        public async Task<bool> SendMail(string ReceiverEmail, string ReceiverName, string Title, string Body)
        {
            using (MimeMessage emailMessage = new MimeMessage())
            {
                //Tạo 1 địa chỉ mail người gửi
                MailboxAddress emailFrom = new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail);
                //add địa chỉ mail người gửi vào mimemessage
                emailMessage.From.Add(emailFrom);
                //tạo 1 địa chỉ mail người nhận
                MailboxAddress emailTo = new MailboxAddress(ReceiverName, ReceiverEmail);
                //add địa chỉ mail người nhận vào mimemessage
                emailMessage.To.Add(emailTo);

                //Gán tiêu đề mail
                emailMessage.Subject = Title;
                //Tạo đối tượng chứa nội dung mail
                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.HtmlBody = Body;
                //Gán nội dung mail vào mimemessage
                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                //tạo đối tượng SmtpClient từ Mailkit.Net.Smtp namespace, không dùng  System.Net.Mail nhé
                using (SmtpClient mailClient = new SmtpClient())
                {
                    //Kết nối tới server smtp.gmail
                    mailClient.Connect(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                    //đăng nhập
                    mailClient.Authenticate(_mailSettings.SenderEmail, _mailSettings.Password);
                    //gửi mail
                    mailClient.Send(emailMessage);
                    //ngắt kết nối
                    mailClient.Disconnect(true);
                }
            }
            return true;
        }
    }
}
