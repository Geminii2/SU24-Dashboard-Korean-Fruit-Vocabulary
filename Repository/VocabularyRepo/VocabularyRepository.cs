using BusinessObject.Models;
using DataAccess;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.VocabularyRepo
{
    public class VocabularyRepository : IVocabularyRepository
    {
        public async Task<IEnumerable<Vocabulary>> GetAll() => await VocabularyDAO.GetInstance.GetAll();
        public async Task<Vocabulary> GetById(int id) => await VocabularyDAO.GetInstance.GetById(id);
        public async Task<int> GenerateNewId() => await VocabularyDAO.GetInstance.IncreaseId();
        public async Task AddVoca(Vocabulary voca) => await VocabularyDAO.GetInstance.SaveData(voca);
        public async Task UpdateVoca(Vocabulary voca) => await VocabularyDAO.GetInstance.SaveData(voca);
        public async Task DeleteVoca(Vocabulary voca) => await VocabularyDAO.GetInstance.SaveData(voca);
        public async Task<string> GetFruitImg(int id) => await VocabularyDAO.GetInstance.GetFruitImg(id);

        public async Task<string> AddFruitImg(int id, string name, IFormFile imageFile)
       => await VocabularyDAO.GetInstance.AddFruitImg(id, name, imageFile);
        public async Task<string> AddVoiceVie(int id, string name, IFormFile voiceVieFile)
       => await VocabularyDAO.GetInstance.AddVoiceVie(id, name, voiceVieFile);
        public async Task<string> AddVoiceEng(int id, string name, IFormFile voiceEngFile)
       => await VocabularyDAO.GetInstance.AddVoiceEng(id, name, voiceEngFile);
        public async Task<string> AddVoiceKor(int id, string name, IFormFile voiceKorFile)
       => await VocabularyDAO.GetInstance.AddVoiceKor(id, name, voiceKorFile);

        public async Task<(List<string> labels, List<string> totals)> GetTopIncorrectVocabularies()
        => await VocabularyDAO.GetInstance.GetTopIncorrectVocabularies();
    }
}
