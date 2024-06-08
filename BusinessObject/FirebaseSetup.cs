namespace BusinessObject
{
    public class FirebaseSetup
    {
        public FirebaseSetup()
        {
            apiKey = "AIzaSyBU-fD9vUnATypJlfNS8AMpoNqdAOL8yVg";
            authDomain = "su24-sep490-koreandictionary.firebaseapp.com";
            databaseURL = "https://su24-sep490-koreandictionary-default-rtdb.firebaseio.com";
            projectId = "su24-sep490-koreandictionary";
            storageBucket = "su24-sep490-koreandictionary.appspot.com";
            messagingSenderId = "939199094656";
            appId = "1:939199094656:android:c93019d816484ce4e76b65";
            measurementId = "G-MN3XSQ5ZCF";
        }

        public string apiKey { get; set; }
        public string authDomain { get; set; }
        public string databaseURL { get; set; }
        public string projectId { get; set; }
        public string storageBucket { get; set; }
        public string messagingSenderId { get; set; }
        public string appId { get; set; }
        public string measurementId { get; set; }
    }
}
