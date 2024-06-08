﻿using BusinessObject.Models;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Feedback_VocaRepo
{
    public class Feedback_VocaRepository : IFeedback_VocaRepository
    {
        public async Task<IEnumerable<Feedback_Voca>> GetAll() => await Feedback_VocaDAO.GetInstance.GetAll();
        public async Task<Feedback_Voca> GetById(int id) => await Feedback_VocaDAO.GetInstance.GetById(id);
    }
}
