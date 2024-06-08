using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.AccountRepo
{
    public interface IAccountRepository
    {
        Task<IEnumerable<Account>> GetAll();
        Task<Account> GetById(int id);
        Task<int> GenerateNewId();
        Task AddUser(Account acc);
        
        Task DeleteUser(Account acc);
        Task<bool> Login(string email, string password);
        Task<Admin> GetByEmail(string email);
        Task<string> GetAvatarImg(int id);
        void Logout(HttpContext httpContext);


        Task<IEnumerable<Admin>> GetAllAdmin();
        Task<Admin> GetAdminById(int id);
        Task<bool> CheckExisted(string email);

        Task AddAdmin(Admin ad);
        Task<string> AddAvatarImg(int id, IFormFile imgFile);
        Task UpdateAdmin(Admin ad);
        Task DeleteAdmin(Admin ad);
        Task UpdateFirebasePassword(string email, string password);
    }
}
