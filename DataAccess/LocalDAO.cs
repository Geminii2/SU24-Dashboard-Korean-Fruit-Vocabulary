using Firebase.Storage;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace DataAccess
{
    public class LocalDAO
    {
        public async Task<string> GetContent(string databaseURL)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(databaseURL);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
        }
        public bool CheckJson(string content)
        {
            if (content.StartsWith("["))
            {
                return true;
            }
            return false;

        }
        public async Task<List<T>> GetAll<T>(string databaseURL)
        {
            var content = await GetContent(databaseURL);
            if (CheckJson(content) == true)
            {
                var objects = JsonConvert.DeserializeObject<List<T>>(content);
                if (objects == null)
                {
                    return new List<T>();
                }
                var list = new List<T>();
                foreach (var item in objects)
                {
                    if (item != null)
                        list.Add(item);
                }
                return list;
            }
            else
            {
                var objects = JsonConvert.DeserializeObject<Dictionary<int, T>>(content);
                if (objects == null)
                {
                    return new List<T>();
                }
                var list = objects.Values.ToList();
                return list;
            }
        }
        public async Task<List<T>> GetAllString<T>(string databaseURL)
        {
            var content = await GetContent(databaseURL);
            if (CheckJson(content) == true)
            {
                var objects = JsonConvert.DeserializeObject<List<T>>(content);
                if (objects == null)
                {
                    return new List<T>();
                }
                var list = new List<T>();
                foreach (var item in objects)
                {
                    if (item != null)
                        list.Add(item);
                }
                return list;
            }
            else
            {
                var objects = JsonConvert.DeserializeObject<Dictionary<string, T>>(content);
                if (objects == null)
                {
                    return new List<T>();
                }
                var list = objects.Values.ToList();
                return list;
            }
        }
        public async Task<int> IncreaseId<T>(string databaseURL, string idPropertyName)
        {
            var content = await GetContent(databaseURL);
            int newid;
            if (CheckJson(content) == true)
            {
                var objects = JsonConvert.DeserializeObject<List<T>>(content);

                if (objects == null)
                {
                    newid = 0;
                }
                else
                {
                    var list = new List<T>();
                    foreach (var item in objects)
                    {
                        if (item != null)
                            list.Add(item);
                    }

                    newid = list.Max(x => GetIdPropertyValue<T>(x, idPropertyName));
                }

                newid++;
                return newid;
            }
            else
            {
                var objects = JsonConvert.DeserializeObject<Dictionary<int, T>>(content);
                if (objects == null)
                {
                    newid = 0;
                }
                else
                {
                    newid = objects.Max(x => GetIdPropertyValue<T>(x.Value, idPropertyName));
                }
                newid++;
                return newid;
            }
        }
        private int GetIdPropertyValue<T>(T obj, string idPropertyName)
        {
            return Convert.ToInt32(obj.GetType().GetProperty(idPropertyName).GetValue(obj, null));
        }

        public async Task SaveData<T>(string databaseURL, T obj)
        {
            using (var httpClient = new HttpClient())
            {
                var jsonData = JsonConvert.SerializeObject(obj);
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync(databaseURL, content);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<T> GetById<T>(string databaseURL)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(databaseURL);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var ojb = JsonConvert.DeserializeObject<T>(content);

                return ojb;
            }
        }

        public async Task DeleteData<T>(string databaseURL, T obj)
        {
            using (var httpClient = new HttpClient())
            {
                var jsonData = JsonConvert.SerializeObject(obj);
                //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                var response = await httpClient.DeleteAsync(databaseURL);

                response.EnsureSuccessStatusCode();
            }
        }

        //public async Task<string> SaveToStorage(string buketURL, string path1, string path2, IFormFile filelink)
        //{
        //    var storage = new FirebaseStorage(buketURL);
        //    var storageReference = storage.Child(path1).Child(path2).Child(filelink.FileName);
        //    //var storageReference = storage.Child(objectName);
        //    using (var stream = new MemoryStream())
        //    {
        //        await filelink.CopyToAsync(stream);
        //        var imageBytes = stream.ToArray();
        //        await storageReference.PutAsync(new MemoryStream(imageBytes));

        //        // Save the image URL in the Realtime Database
        //        string downloadUrl = await storageReference.GetDownloadUrlAsync();

        //        // Save the image URL in the Realtime Database
        //        return downloadUrl;

        //    }
        //}
        public async Task<string> SaveToStorage(string buketURL, string path, IFormFile filelink)
        {
            var storage = new FirebaseStorage(buketURL);
            var storageReference = storage.Child(path);
            using (var stream = new MemoryStream())
            {
                await filelink.CopyToAsync(stream);
                var imageBytes = stream.ToArray();
                await storageReference.PutAsync(new MemoryStream(imageBytes));
                string downloadUrl = await storageReference.GetDownloadUrlAsync();
                return downloadUrl;

            }
        }
        public async Task DeleteFromStorage(string bucketURL, string path)
        {
            var storage = new FirebaseStorage(bucketURL);
            var reference = storage.Child(path);

            // Delete the file from storage
            await reference.DeleteAsync();
        }

    }
}
