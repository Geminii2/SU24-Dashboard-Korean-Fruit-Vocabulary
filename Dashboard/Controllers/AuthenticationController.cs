﻿using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.AccountRepo;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Dashboard.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IAccountRepository _accRepository;
        public AuthenticationController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;

        }
        public IActionResult Index()
        {
            var accID = HttpContext.Session.GetInt32("Id");
            ViewBag.Id = accID;
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            var returnUrl = HttpContext.Request.Cookies["returnUrl"];
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Admin account, string returnUrl)
        {
            try
            {

                var loggedInAccount = await _accRepository.GetByEmail(account.Email);
                string md5pass = GetMD5(account.Pwd);
                if (loggedInAccount.Status == 3)
                {
                    ViewData["isBaned"]= "The account is baned";
                    return View();
                }

                var isValidCredentials = await _accRepository.Login(account.Email, md5pass);

                if (isValidCredentials)
                {
                    loggedInAccount = await _accRepository.GetByEmail(account.Email);

                    // Set a cookie to store the user ID (log the user in)
                    HttpContext.Session.SetInt32("Id", loggedInAccount.Id);
                    HttpContext.Session.SetString("AvatarImg", loggedInAccount.Avatar_img);
                    HttpContext.Session.SetString("Fullname", loggedInAccount.Fullname);
                    HttpContext.Session.SetInt32("RoleID", loggedInAccount.Role_id);

                    if (loggedInAccount.Email != null)
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Dashboard");
                }

                ViewData["Invalid"]="Invalid email or password";

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


            return View(account);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            // Get the logged-in user's AccountID from the session
            var accountId = HttpContext.Session.GetInt32("Id");

            if (accountId != null)
            {
                // Call the modified Logout method in the repository
                _accRepository.Logout(HttpContext);
            }

            // Redirect to the login page or another appropriate page
            return RedirectToAction("Login", "Authentication");
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
