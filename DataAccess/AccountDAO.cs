﻿using BusinessObject;
using BusinessObject.Models;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class AccountDAO
    {
        // Get firebaseSetup
        FirebaseSetup firebaseSetup = new FirebaseSetup();
        // Singleton
        private static AccountDAO instance;
        private static readonly object instanceLock = new object();
        public static LocalDAO localDAO = new LocalDAO();
        private readonly HttpClient _httpClient;
        private readonly string _firebaseApiKey;
        string dtb;
        string dtbad;
        string bucket;
        public static AccountDAO GetInstance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new AccountDAO();
                    }
                    return instance;
                }
            }
        }

        public AccountDAO()
        {
            dtb = firebaseSetup.databaseURL + "/Account/";
            dtbad = firebaseSetup.databaseURL + "/Admin/";
            _httpClient = new HttpClient();
            _firebaseApiKey = firebaseSetup.apiKey;
            bucket = firebaseSetup.storageBucket;
        }

        public async Task<List<Account>> GetAll()
        {
            string databaseURL = dtb + ".json";
            List<Account> accounts = await localDAO.GetAll<Account>(databaseURL);
            return accounts;
        }

        public async Task<Account> GetById(int id)
        {
            string databaseURL = dtb + id + ".json";
            return await localDAO.GetById<Account>(databaseURL);
        }

        public async Task<int> IncreaseId()
        {
            string databaseURL = dtbad + ".json";
            return await localDAO.IncreaseId<Admin>(databaseURL, "Id");
        }

        public async Task SaveData(Account acc)
        {
            string databaseURL = dtb + acc.Id + ".json";
            await localDAO.SaveData(databaseURL, acc);
        }

        public async Task DeleteData(Account acc)
        {
            string databaseURL = dtb + acc.Id + ".json";
            await localDAO.DeleteData(databaseURL, acc);
        }


        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var request = new
                {
                    email,
                    password,
                    returnSecureToken = true
                };

                var jsonRequest = JsonConvert.SerializeObject(request);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(
                    $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_firebaseApiKey}",
                    content
                );

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);

                // If the request is successful and a token is returned, the user's credentials are valid
                if (!string.IsNullOrEmpty(tokenResponse?.IdToken))
                {
                    return true;
                }
            }
            catch (HttpRequestException)
            {
                // Handle request exceptions (e.g., network issues, invalid API key, etc.)
                return false;
            }

            return false; // Trả về false nếu xác thực thất bại
        }

        private class TokenResponse
        {
            public string IdToken { get; set; }
        }
        public async Task<Admin> GetAccountByEmail(string email)
        {
            try
            {
                string databaseURL = dtbad + ".json";
                List<Admin> allAccounts = await localDAO.GetAll<Admin>(databaseURL);
                Admin? ad = allAccounts.FirstOrDefault(acc => acc.Email == email);

                return ad;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null; // Return null if account not found
        }
        public async Task<string> GetAvatarImg(int accountId)
        {
            try
            {
                string databaseURL = dtb + accountId + ".json"; // Assuming you have the avatarLink stored in the database

                // Retrieve the account data from the database
                var account = await localDAO.GetById<Account>(databaseURL);

                if (account != null)
                {
                    // Assuming the Account class has an Avatarlink property
                    return account.Avatar;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null; // Return null if the account or avatarLink is not found
        }

        public async Task<bool> IsAccountExisted(string email)
        {
            try
            {
                // Use Firebase Authentication to check if the email is in use
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                // If a user with this email exists, return true
                return user != null;
            }
            catch (FirebaseAuthException ex)
            {
                return false; // Assume the email is not in use
            }
        }
        public void Logout(HttpContext httpContext)
        {
            httpContext.Session.Clear();
        }

        public async Task<List<Admin>> GetAllAdmin()
        {
            string databaseURL = dtbad + ".json";
            List<Admin> admins = await localDAO.GetAll<Admin>(databaseURL);
            var admin = admins.Where(ad => ad.Role_id == 2).ToList();
            return admin;
        }
        public async Task<Admin> GetAdminById(int id)
        {
            string databaseURL = dtbad + id + ".json";
            return await localDAO.GetById<Admin>(databaseURL);
        }
        public async Task SaveDataAd(Admin ad)
        {
            string databaseURL = dtbad + ad.Id + ".json";
            await localDAO.SaveData(databaseURL, ad);
        }
        public async Task<string> AddAvatarImg(int Id, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string path = $"Avatar_img/user_{Id}.png";
                return await localDAO.SaveToStorage(bucket, path, imageFile);
            }
            else return string.Empty;
        }
        public async Task UpdateFirebasePassword(string email, string newPassword)
        {
            try
            {
                // Get the user by email
                var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

                // Set the new password
                var args = new UserRecordArgs
                {
                    Uid = user.Uid,
                    Password = newPassword,
                };

                // Update the user's password
                await FirebaseAuth.DefaultInstance.UpdateUserAsync(args);
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine(ex.Message);
                // Handle Firebase Authentication exception as needed
            }
        }


        public async Task<List<StatisticsItem>> CountAccountbyAge(int year, string ageRange)
        {
            var accounts = await GetAll();

            var minAge = int.Parse(ageRange.Split('-')[0]);
            var maxAge = int.Parse(ageRange.Split('-')[1]);

            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31);

            var statistics = new List<StatisticsItem>();
            for (int i = 1; i<=12; i++)
            {
                statistics.Add(new StatisticsItem
                {
                    Label = i.ToString(),
                    Total = 0,
                    Female = 0,
                    Male=0
                });
            }
            foreach (var account in accounts)
            {
                var dob = new DateTime();
                if (account.Dob != null && account.Dob != "")
                {
                    dob = DateTime.ParseExact(account.Dob, "dd-MM-yyyy", null);
                }
                //var createdDate = DateTime.ParseExact(account.Created_date, "dd-MM-yyyy", null);
                var createdDate = new DateTime();
                if (account.Created_date != null && account.Created_date != "")
                {
                    createdDate = DateTime.ParseExact(account.Created_date, "dd-MM-yyyy", null);
                }

                if (createdDate >= startDate && createdDate <= endDate)
                {
                    var age = CalculateAge(dob, startDate);
                    if (age >= minAge && age <= maxAge)
                    {
                        var month = createdDate.Month;
                        var statItem = statistics.FirstOrDefault(s => s.Label == month.ToString());
                        if (statItem != null)
                        {
                            // Cập nhật số liệu
                            statItem.Total++;
                            if (account.Gender == "Male")
                                statItem.Male++;
                            else if (account.Gender == "Female")
                                statItem.Female++;
                        }
                    }
                }
            }
            return statistics;
        }

        public int CalculateAge(DateTime birthDate, DateTime referenceDate)
        {
            int age = referenceDate.Year - birthDate.Year;
            return age;
        }


        public async Task<List<StatisticsItem>> CountAgebyYear(string ageRange, string startDate, string endDate)
        {
            var accounts = await GetAll();

            var minAge = int.Parse(ageRange.Split('-')[0]);
            var maxAge = int.Parse(ageRange.Split('-')[1]);

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var statistics = new List<StatisticsItem>();
            for (int i = start.Year; i<=end.Year; i++)
            {
                statistics.Add(new StatisticsItem
                {
                    Label = i.ToString(),
                    Total = 0,
                    Female = 0,
                    Male=0
                });
            }
            foreach (var account in accounts)
            {
                var dob = new DateTime();
                if (account.Dob != null && account.Dob != "")
                {
                    dob = DateTime.ParseExact(account.Dob, "dd-MM-yyyy", null);
                }
                //var createdDate = DateTime.ParseExact(account.Created_date, "dd-MM-yyyy", null);
                var createdDate = new DateTime();
                if (account.Created_date != null && account.Created_date != "")
                {
                    createdDate = DateTime.Parse(account.Created_date);
                }

                if (createdDate >= start && createdDate <= end)
                {
                    var age = CalculateAge(dob, start);
                    if (age >= minAge && age <= maxAge)
                    {
                        var year = createdDate.Year;
                        var statItem = statistics.FirstOrDefault(s => s.Label == year.ToString());
                        if (statItem != null)
                        {
                            // Cập nhật số liệu
                            statItem.Total++;
                            if (account.Gender == "Male")
                                statItem.Male++;
                            else if (account.Gender == "Female")
                                statItem.Female++;
                        }
                    }
                }
            }
            return statistics;
        }

        public async Task<StatisticsItem> CountAgebyCustom(string ageRange, string startDate, string endDate)
        {
            var accounts = await GetAll();

            var minAge = int.Parse(ageRange.Split('-')[0]);
            var maxAge = int.Parse(ageRange.Split('-')[1]);

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            var statistics = new StatisticsItem();
            //for (var i = start.Year; i<=end.Year; i++)
            //{
            //    statistics.Add(new StatisticsItem
            //    {
            //        Label = i.ToString(),
            //        Total = 0,
            //        Female = 0,
            //        Male=0
            //    });
            //}
            foreach (var account in accounts)
            {
                var dob = new DateTime();
                if (account.Dob != null && account.Dob != "")
                {
                    dob = DateTime.ParseExact(account.Dob, "dd-MM-yyyy", null);
                }
                //var createdDate = DateTime.ParseExact(account.Created_date, "dd-MM-yyyy", null);
                var createdDate = new DateTime();
                if (account.Created_date != null && account.Created_date != "")
                {
                    createdDate = DateTime.Parse(account.Created_date);
                }

                if (createdDate >= start && createdDate <= end)
                {
                    var age = CalculateAge(dob, start);
                    if (age >= minAge && age <= maxAge)
                    {
                        statistics.Total++;
                        if (account.Gender == "Male")
                            statistics.Male++;
                        else if (account.Gender == "Female")
                            statistics.Female++;


                        //var date = createdDate;
                        //var statItem = statistics.FirstOrDefault(s => s.Label == date.ToString());
                        //if (statItem != null)
                        //{
                        //    // Cập nhật số liệu
                        //    statItem.Total++;
                        //    if (account.Gender == "Male")
                        //        statItem.Male++;
                        //    else if (account.Gender == "Female")
                        //        statItem.Female++;
                        //}
                    }
                }
            }
            return statistics;
        }


        public async Task<List<CountryCount>> CountCountry(int year)
        {
            var accounts = await GetAll();

            var countryCounts = accounts.Where(a => a.Created_date != null && IsInDateRange(a.Created_date, year))
                .GroupBy(a => a.Country).Select(g => new CountryCount
                {
                    Country = g.Key,
                    UserCount = g.Count()
                }).OrderByDescending(c => c.UserCount).ToList();


            return countryCounts;

        }

        private bool IsInDateRange(string createdDate, int year)
        {
            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = new DateTime(year, 12, 31);
            var date = new DateTime();

            if (createdDate!= null || createdDate!="")
            {
                date = DateTime.ParseExact(createdDate, "dd-MM-yyyy", null);
            }
            if (date >= startDate && date <= endDate)
            {
                return true;
            }
            return false;
        }
    }
}
