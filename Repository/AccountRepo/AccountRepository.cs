using BusinessObject;
using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.AccountRepo
{
    public class AccountRepository : IAccountRepository
    {
        public async Task<IEnumerable<Account>> GetAll() => await AccountDAO.GetInstance.GetAll();

        public async Task<Account> GetById(int id) => await AccountDAO.GetInstance.GetById(id);
        public async Task<int> GenerateNewId() => await AccountDAO.GetInstance.IncreaseId();
        public async Task AddUser(Account acc) => await AccountDAO.GetInstance.SaveData(acc);
        public async Task UpdateUser(Account acc) => await AccountDAO.GetInstance.SaveData(acc);
        public async Task DeleteUser(Account acc) => await AccountDAO.GetInstance.SaveData(acc);
        public async Task<bool> Login(string email, string password) => await AccountDAO.GetInstance.Login(email, password);
        public async Task<Admin> GetByEmail(string email) => await AccountDAO.GetInstance.GetAccountByEmail(email);
        public async Task<string> GetAvatarImg(int id) => await AccountDAO.GetInstance.GetAvatarImg(id);

        public void Logout(HttpContext httpContext) => AccountDAO.GetInstance.Logout(httpContext);

        public async Task<IEnumerable<Admin>> GetAllAdmin() => await AccountDAO.GetInstance.GetAllAdmin();
        public async Task<bool> CheckExisted(string email) => await AccountDAO.GetInstance.IsAccountExisted(email);
        public async Task AddAdmin(Admin ad) => await AccountDAO.GetInstance.SaveDataAd(ad);
        public async Task<string> AddAvatarImg(int id, IFormFile imageFile)
        => await AccountDAO.GetInstance.AddAvatarImg(id, imageFile);

        public async Task<Admin> GetAdminById(int id) => await AccountDAO.GetInstance.GetAdminById(id);
        public async Task DeleteAdmin(Admin ad) => await AccountDAO.GetInstance.SaveDataAd(ad);
        public async Task UpdateAdmin(Admin ad) => await AccountDAO.GetInstance.SaveDataAd(ad);
        public async Task UpdateFirebasePassword(string email, string pwd) => await AccountDAO.GetInstance.UpdateFirebasePassword(email, pwd);

        public async Task<List<StatisticsItem>> statisticsItems(int year, string age) => await AccountDAO.GetInstance.CountAccountbyAge(year, age);


    }
}
