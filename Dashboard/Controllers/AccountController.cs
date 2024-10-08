﻿using BusinessObject;
using BusinessObject.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Google.Apis.Util;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;
using Org.BouncyCastle.Asn1.Pkcs;
using Repository.AccountRepo;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace Dashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accRepository;
        private readonly MailSettings _mailSettings;
        public AccountController(IAccountRepository accRepository, IOptions<MailSettings> mailSettingsOptions)
        {
            _accRepository = accRepository;
            _mailSettings = mailSettingsOptions.Value;
        }


        [HttpGet]
        public async Task<JsonResult> GetData()
        {
            try
            {
                var obj = await _accRepository.GetAll();

                var list = obj.Select(item => new
                {
                    id = item.Id,
                    fullname = item.Fullname,
                    email = item.Email,
                    gender = item.Gender,
                    dob = item.Dob.Replace("-", "/"),
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

            var accounts = await _accRepository.GetAll();
            return View(accounts);

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

                var acc = await _accRepository.GetById(id);
                var img = await _accRepository.GetAvatarImg(id);

                if (acc != null)
                {
                    ViewData["AvatarImg"] = acc.Avatar;
                    return View(acc);
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

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {

                var obj = await _accRepository.GetById(id);

                if (obj != null)
                {
                    obj.Status = 2;

                    await _accRepository.DeleteUser(obj);
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
        [HttpPost]
        public async Task<IActionResult> UnDelete(int id)
        {
            try
            {

                var obj = await _accRepository.GetById(id);

                if (obj != null)
                {
                    obj.Status = 1;

                    await _accRepository.DeleteUser(obj);
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

        [HttpGet]
        public async Task<JsonResult> GetDataAdmin()
        {
            try
            {
                var obj = await _accRepository.GetAllAdmin();

                var list = obj.Select(item => new
                {
                    id = item.Id,
                    fullname = item.Fullname,
                    email = item.Email,
                    gender = item.Gender,
                    dob = item.Dob.Replace("-", "/"),
                    status = item.Status,
                });

                return Json(new { data = list });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        public async Task<IActionResult> Admins()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var accounts = await _accRepository.GetAll();
            return View(accounts);

        }
        public IActionResult AddAdmin()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAdmin(Admin? ad)
        {
            var accID = HttpContext.Session.GetInt32("Id");

            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int newid = await _accRepository.GenerateNewId();
            //Random password
            string password = "123456";
            string md5pass = GetMD5(password);
            //if (!ModelState.IsValid)
            //{
            try
            {
                // Check Email has existed
                bool isEmailInUse = await _accRepository.CheckExisted(ad.Email);
                if (isEmailInUse)
                {
                    //ModelState.AddModelError("", "This email is already registered.");
                    ViewData["isEmail"] = "This email is already registered.";
                    return View(ad);
                }
                // Create a new user with email and password
                var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
                {
                    Email = ad.Email,
                    Password = md5pass, // Use the entered password for user creation
                });

                Admin admin = new Admin(newid, ad.Email, md5pass, ad.Fullname);
                await _accRepository.AddAdmin(admin);
                //Send Mail
                await SendMail(ad.Email, ad.Fullname, password);

                return RedirectToAction("Admins");
            }
            catch (Firebase.Auth.FirebaseAuthException ex)
            {
                //ModelState.AddModelError("", "An error occurred during registration.");
                return View("Error");
            }
            catch (HttpRequestException ex)
            {
                //ModelState.AddModelError("", $"HttpRequestException: {ex.Message}");
                return View("Error");
                // Log or handle the exception
                //Console.WriteLine($"HttpRequestException: {ex.Message}");
            }
            catch (SocketException ex)
            {
                //ModelState.AddModelError("", $"SocketException: {ex.Message}");
                return View("Error");
                // Log or handle the exception
                //Console.WriteLine($"SocketException: {ex.Message}");
            }
            //}
            //return View(ad);
        }

        public async Task<IActionResult> DetailAdmin(int id)
        {
            try
            {
                var accID = HttpContext.Session.GetInt32("Id");
                if (accID == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }

                var acc = await _accRepository.GetAdminById(id);

                if (acc != null)
                {
                    ViewData["AvatarImg"] = acc.Avatar_img;
                    return View(acc);
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

        [HttpPost]
        public async Task<IActionResult> ResetPasswordDefault(int id)
        {
            try
            {
                string pwd = "123456";
                string md5pass = GetMD5(pwd);
                var obj = await _accRepository.GetAdminById(id);
                if (obj != null)
                {
                    await _accRepository.UpdateFirebasePassword(obj.Email, md5pass);
                    obj.Pwd = md5pass;
                    obj.Status=1;
                    await _accRepository.UpdateAdmin(obj);
                    await SendMail(obj.Email, obj.Fullname, pwd);
                    return RedirectToAction("Admins");
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> ChangePwd()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var account = await _accRepository.GetAdminById(accID.Value);
            if (account == null)
            {
                // Handle the case where the account with the given ID doesn't exist
                return NotFound();
            }

            return View(account);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePwd(string currentPassword, string newPassword, string confirmPassword)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            var account = await _accRepository.GetAdminById(accID.Value);
            if (account == null)
            {
                // Handle the case where the account with the given ID doesn't exist
                return NotFound();
            }
            var invalid1 = "";
            var invalid2 = "";

            if (string.IsNullOrEmpty(currentPassword))
            {
                invalid1 ="Please current password.";
                ViewData["Invalid1"]=invalid1;
                ModelState.AddModelError("", "Current password is incorrect.");
                if (string.IsNullOrEmpty(newPassword))
                {
                    invalid2 ="Please new password.";
                    ViewData["Invalid2"]=invalid2;
                    return View(account);
                }
                return View(account); // Redirect back to the change password page with an error message
            }
            if (!string.IsNullOrEmpty(currentPassword) && currentPassword.Length<6)
            {
                invalid1 ="Current password minimum length of 6.";
                ViewData["Invalid1"]=invalid1;
                if (!string.IsNullOrEmpty(newPassword) && newPassword.Length<6)
                {
                    invalid2 ="New password minimum length of 6.";
                    ViewData["Invalid2"]=invalid2;
                    return View(account);
                }
                return View(account);
            }

            if (account.Pwd != GetMD5(currentPassword))
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(account); // Redirect back to the change password page with an error message
            }

            // Check if the new password and confirm password match
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "New password and confirm password do not match.");
                return View(account); // Redirect back to the change password page with an error message
            }
            string md5pass = GetMD5(newPassword);
            account.Pwd = md5pass;
            await _accRepository.UpdateFirebasePassword(account.Email, md5pass);
            await _accRepository.UpdateAdmin(account);
            return RedirectToAction("Index", "Dashboard");

        }


        [HttpPost]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {
                var obj = await _accRepository.GetAdminById(id);

                if (obj != null)
                {
                    obj.Status = 3;

                    await _accRepository.DeleteAdmin(obj);
                    return RedirectToAction("Admins");
                }
                return View("Error");

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }

        }

        public async Task<IActionResult> EditAdmin(int id)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var obj = await _accRepository.GetAdminById(id);
            ViewData["AvatarImg"] = obj.Avatar_img;
            return View(obj);
        }
        [HttpPost]
        public async Task<IActionResult> EditAdmin(Admin admin)
        {
            var accountId = HttpContext.Session.GetInt32("Id");
            string pwd = "123456";

            if (accountId == null)
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Authentication");
            }


            var ad = await _accRepository.GetAdminById(admin.Id);

            var acc = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(ad.Email);
            await FirebaseAuth.DefaultInstance.DeleteUserAsync(acc.Uid);
            ad.Email= admin.Email;
            ad.Fullname= admin.Fullname;
            ad.Pwd= admin.Pwd;
            await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
            {
                Email = ad.Email,
                Password = pwd, // Use the entered password for user creation
            });

            await _accRepository.UpdateAdmin(ad);
            await SendMail(ad.Email, ad.Fullname, pwd);


            return RedirectToAction("Admins");
        }

        public async Task<bool> SendMail(string ReceiverEmail, string ReceiverName, string pwd)
        {
            string Title = "New Account";
            string Body = $@"
                          <h1>Welcome to Our Service</h1>
                          <p>Dear {ReceiverName},</p>
                          <p>We are excited to inform you that your new account has been created successfully.</p>
                          <p><strong>Account Information:</strong></p>
                          <ul>
                              <li>Email: {ReceiverEmail}</li>
                              <li>Password: {pwd}</li>
                          </ul>
                          <p>Thank you for joining us!</p>
                         <p>Best regards,<br/>The Team</p>";


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

        public string GetMD5(string pass)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(pass));

            StringBuilder sbHash = new StringBuilder();
            foreach (byte b in bHash)
            {

                sbHash.Append(String.Format("{0:x2}", b));

            }

            return sbHash.ToString();
        }
    }
}
