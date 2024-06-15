using BusinessObject.Models;
using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Feedback_VocaDAO
    {
        // Get firebaseSetup
        FirebaseSetup firebaseSetup = new FirebaseSetup();
        // Singleton
        private static Feedback_VocaDAO instance;
        private static readonly object instanceLock = new object();
        public static LocalDAO localDAO = new LocalDAO();
        string dtb;
        public static Feedback_VocaDAO GetInstance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Feedback_VocaDAO();
                    }
                    return instance;
                }
            }
        }

        public Feedback_VocaDAO()
        {
            dtb = firebaseSetup.databaseURL + "/Feedback_Voca/";
        }

        public async Task<List<Feedback_Voca>> GetAll()
        {
            string databaseURL = dtb + ".json";
            List<Feedback_Voca> feedback_voca = await localDAO.GetAllString<Feedback_Voca>(databaseURL);
            return feedback_voca;
        }

        public async Task<Feedback_Voca> GetById(string id)
        {
            string databaseURL = dtb + id + ".json";
            return await localDAO.GetById<Feedback_Voca>(databaseURL);
        }

        public async Task SaveData(Feedback_Voca fb_voca)
        {
            string databaseURL = dtb + fb_voca.Id + ".json";
            await localDAO.SaveData(databaseURL, fb_voca);
        }
    }
}
