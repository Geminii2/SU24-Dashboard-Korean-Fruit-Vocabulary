using BusinessObject.Models;
using Firebase.Auth;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Mvc;
using Repository.AccountRepo;
using System.Net.Sockets;
using System.Security.Principal;

namespace Dashboard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountRepository _accRepository;

        public AccountController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;

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
                    dob=item.Dob,
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
                var img= await _accRepository.GetAvatarImg(id);

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
                    dob = item.Dob,
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
        public async Task<IActionResult> AddAdmin(Admin? ad, IFormFile imgFile)
        {
            var accID = HttpContext.Session.GetInt32("Id");
            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            int newid = await _accRepository.GenerateNewId();
            var avartar = await _accRepository.AddAvatarImg(newid, imgFile);
            if (ModelState.IsValid)
            {
                try
                {
                    // Check Email has existed
                    bool isEmailInUse = await _accRepository.CheckExisted(ad.Email);
                    if (isEmailInUse)
                    {
                        ModelState.AddModelError("", "This email is already registered.");
                        return View(ad);
                    }
                    // Check length Password and ConfirmPassword
                    if (ad.Pwd.Length < 6 )
                    {
                        ModelState.AddModelError("", "The length of password must be more than 6 characters!");
                        return View(ad);
                    }
                    // Create a new user with email and password
                    var user = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs()
                    {
                        Email = ad.Email,
                        Password = ad.Pwd, // Use the entered password for user creation
                    });
                    
                    Admin admin = new Admin(newid, ad.Email, ad.Pwd, avartar, ad.Fullname);

                    await _accRepository.AddAdmin(admin);
                    return RedirectToAction("Admins");
                }
                catch (Firebase.Auth.FirebaseAuthException ex)
                {
                    ModelState.AddModelError("", "An error occurred during registration.");
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
            return View(ad);
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
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            try
            {

                var obj = await _accRepository.GetAdminById(id);

                if (obj != null)
                {
                    obj.Status = 2;

                    await _accRepository.DeleteAdmin(obj);
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

            if (accountId == null)
            {
                // Redirect to the login page if the user is not logged in
                return RedirectToAction("Login", "Authentication");
            }
            if (ModelState.IsValid)
                {
                    await _accRepository.UpdateFirebasePassword(admin.Email, admin.Pwd);

                
                    await _accRepository.UpdateAdmin(admin);
                }
            return RedirectToAction("Admins");
        }

        
    }
}
