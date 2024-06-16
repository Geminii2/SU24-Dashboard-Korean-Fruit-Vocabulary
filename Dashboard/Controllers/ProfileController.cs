using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.AccountRepo;
using System.Globalization;
using System.Security.Principal;

namespace Dashboard.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IAccountRepository _accRepository;

        public ProfileController(IAccountRepository accRepository)
        {
            _accRepository = accRepository;

        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var accID = HttpContext.Session.GetInt32("Id");
                if (accID == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }

                var acc = await _accRepository.GetAdminById(accID.Value);

                if (acc != null)
                {
                    //var dob = DateTime.Parse(acc.Dob);
                    //ViewData["BirthDate"]=dob.ToString("dd-MM-yyyy");
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

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                //Get the logged-in user's AccountID from the session
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
        public async Task<IActionResult> Edit(Admin admin, IFormFile imageFile)
        {
            string avatar;
            var accID = HttpContext.Session.GetInt32("Id");

            if (accID == null)
            {
                return RedirectToAction("Login", "Authentication");
            }
            if(imageFile == null)
            {
                avatar = admin.Avatar_img;
            }
            else
            {
                avatar = await _accRepository.AddAvatarImg(admin.Id, imageFile);
            }
            DateTime dob = DateTime.Parse(admin.Dob);
            Admin ad = new Admin
                {
                    Id=admin.Id,
                    Avatar_img= avatar,
                    Country=admin.Country,
                    Dob=dob.ToString("dd/MM/yyyy"),
                    Email=admin.Email,
                    Fullname=admin.Fullname,
                    Gender=admin.Gender,
                    Phone=admin.Phone,
                    Pwd=admin.Pwd,
                    Role_id=admin.Role_id,
                    Status = admin.Status
                };
            await _accRepository.UpdateAdmin(ad);
            return RedirectToAction("Index");

        }
    }
}
