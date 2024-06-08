using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Feedback_VocaRepo
{
    public interface IFeedback_VocaRepository
    {
        Task<IEnumerable<Feedback_Voca>> GetAll();
        Task<Feedback_Voca> GetById(int id);
    }
}
