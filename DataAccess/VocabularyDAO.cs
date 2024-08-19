using BusinessObject;
using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class VocabularyDAO
    {
        // Get firebaseSetup
        FirebaseSetup firebaseSetup = new FirebaseSetup();
        // Singleton
        private static VocabularyDAO instance;
        private static readonly object instanceLock = new object();
        public static LocalDAO localDAO = new LocalDAO();
        string dtb;
        string dtbQ;
        string dtbH;
        string bucket;
        public static VocabularyDAO GetInstance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new VocabularyDAO();
                    }
                    return instance;
                }
            }
        }

        public VocabularyDAO()
        {
            dtb = firebaseSetup.databaseURL + "/Vocabulary/";
            dtbQ= firebaseSetup.databaseURL +"/Question/";
            dtbH= firebaseSetup.databaseURL +"/History/";
            bucket = firebaseSetup.storageBucket;
        }

        public async Task<List<Vocabulary>> GetAll()
        {
            string databaseURL = dtb + ".json";
            List<Vocabulary> voca = await localDAO.GetAll<Vocabulary>(databaseURL);
            var vocaList = voca.Where(v => v.Status==1).ToList();
            return vocaList;
        }

        public async Task<Vocabulary> GetById(int id)
        {
            string databaseURL = dtb + id + ".json";
            return await localDAO.GetById<Vocabulary>(databaseURL);
        }

        public async Task<int> IncreaseId()
        {
            string databaseURL = dtb + ".json";
            return await localDAO.IncreaseId<Vocabulary>(databaseURL, "Id");
        }

        public async Task SaveData(Vocabulary acc)
        {
            string databaseURL = dtb + acc.Id + ".json";
            await localDAO.SaveData(databaseURL, acc);
        }

        public async Task<string> GetFruitImg(int id)
        {
            try
            {
                string databaseURL = dtb + id + ".json"; // Assuming you have the avatarLink stored in the database

                // Retrieve the account data from the database
                var account = await localDAO.GetById<Vocabulary>(databaseURL);

                if (account != null)
                {
                    // Assuming the Account class has an Avatarlink property
                    return account.Fruits_img;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null; // Return null if the account or avatarLink is not found
        }

        public async Task<string> AddFruitImg(int Id, string name, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string path = $"Fruits_img/{name}_{Id}.png";
                return await localDAO.SaveToStorage(bucket, path, imageFile);
            }
            else return string.Empty;
        }
        public async Task<string> AddVoiceVie(int Id, string name, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string path = $"Voice/voice_vn/{name}_{Id}.mp3";
                return await localDAO.SaveToStorage(bucket, path, imageFile);
            }
            else return string.Empty;
        }
        public async Task<string> AddVoiceEng(int Id, string name, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string path = $"Voice/voice_en/{name}_{Id}.mp3";
                return await localDAO.SaveToStorage(bucket, path, imageFile);
            }
            else return string.Empty;
        }
        public async Task<string> AddVoiceKor(int Id, string name, IFormFile imageFile)
        {
            if (imageFile != null)
            {
                string path = $"Voice/voice_kr/{name}_{Id}.mp3";
                return await localDAO.SaveToStorage(bucket, path, imageFile);
            }
            else return string.Empty;
        }

        public async Task<(List<string> labels, List<string> totals)> GetTopIncorrectVocabularies()
        {
            string databaseVocaURL = dtb + ".json";
            List<Vocabulary> vocabularies = await localDAO.GetAll<Vocabulary>(databaseVocaURL);

            string databaseQuestURL = dtbQ + ".json";
            List<Question> questions = await localDAO.GetAllString<Question>(databaseQuestURL);

            string databaseHistoryURL = dtbH + ".json";
            List<History> histories = await localDAO.GetAllString<History>(databaseHistoryURL);

            var incorrectHistories = histories.Where(h => !h.Result).Select(h => h.Question_Id).ToList();

            // kiểm tra lỗi trong data
            //var missingQids = incorrectHistories
            //.Where(qid => !questions.Any(q => q.Id == qid))
            //.ToList();

            //foreach (var qid in missingQids)
            //{
            //    Console.WriteLine($"Không tìm thấy câu hỏi với Id: {qid}");
            //}

            var incorrectQuestionCounts = incorrectHistories.GroupBy(qid => questions.First(q => q.Id == qid).Vocabulary_Id)
                .Select(group => new
                {
                    VocabularyId = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(g => g.Count)
                .ToList();
            var topVocabularies = vocabularies.Where(v => incorrectQuestionCounts.Any(iqc => iqc.VocabularyId == v.Id))
                .ToDictionary(v => v.Id, v => v.English);

            var labels = incorrectQuestionCounts.Select(iqc => topVocabularies[iqc.VocabularyId]).ToList();
            var totals = incorrectQuestionCounts.Select(iqc => iqc.Count.ToString()).ToList();

            return (labels, totals);

        }

    }
}

