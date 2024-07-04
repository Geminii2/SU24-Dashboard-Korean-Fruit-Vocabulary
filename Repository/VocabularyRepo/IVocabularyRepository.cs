using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.VocabularyRepo
{
    public interface IVocabularyRepository
    {
        Task<IEnumerable<Vocabulary>> GetAll();
        Task<Vocabulary> GetById(int id);
        Task<int> GenerateNewId();
        Task AddVoca(Vocabulary voca);
        Task UpdateVoca(Vocabulary voca);
        Task DeleteVoca(Vocabulary voca);
        Task<string> GetFruitImg(int id);
        Task<string> AddFruitImg(int id, string name, IFormFile imgFile);
        Task<string> AddVoiceVie(int id, string name, IFormFile voiceVieFile);
        Task<string> AddVoiceEng(int id, string name, IFormFile voiceEngFile);
        Task<string> AddVoiceKor(int id, string name, IFormFile voiceKorFile);

        Task<(List<string> labels, List<string> totals)> GetTopIncorrectVocabularies();
    }
}
