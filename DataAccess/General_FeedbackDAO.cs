using BusinessObject.Models;
using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class General_FeedbackDAO
    {
        // Get firebaseSetup
        FirebaseSetup firebaseSetup = new FirebaseSetup();
        // Singleton
        private static General_FeedbackDAO instance;
        private static readonly object instanceLock = new object();
        public static LocalDAO localDAO = new LocalDAO();
        string dtb;
        public static General_FeedbackDAO GetInstance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new General_FeedbackDAO();
                    }
                    return instance;
                }
            }
        }

        public General_FeedbackDAO()
        {
            dtb = firebaseSetup.databaseURL + "/General Feedback/";
        }

        public async Task<List<General_Feedback>> GetAll()
        {
            string databaseURL = dtb + ".json";
            List<General_Feedback> gen_feedback = await localDAO.GetAll<General_Feedback>(databaseURL);
            return gen_feedback;
        }
        public async Task<General_Feedback> GetById(int id)
        {
            string databaseURL = dtb + id + ".json";
            return await localDAO.GetById<General_Feedback>(databaseURL);
        }
    }
}
