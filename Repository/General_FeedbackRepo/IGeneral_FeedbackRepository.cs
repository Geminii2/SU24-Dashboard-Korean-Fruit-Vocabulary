using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.General_FeedbackRepo
{
    public interface IGeneral_FeedbackRepository
    {
        Task<IEnumerable<General_Feedback>> GetAll();
        Task<General_Feedback> GetById(int id);
    }
}
