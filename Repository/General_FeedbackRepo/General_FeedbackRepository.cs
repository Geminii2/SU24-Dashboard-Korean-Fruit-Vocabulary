using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.General_FeedbackRepo
{
    public class General_FeedbackRepository : IGeneral_FeedbackRepository
    {
        public async Task<IEnumerable<General_Feedback>> GetAll()=> await General_FeedbackDAO.GetInstance.GetAll();
        public async Task<General_Feedback> GetById(string id) => await General_FeedbackDAO.GetInstance.GetById(id);
        public async Task UpdateVoca(General_Feedback gen_fb) => await General_FeedbackDAO.GetInstance.SaveData(gen_fb);
    }
}
