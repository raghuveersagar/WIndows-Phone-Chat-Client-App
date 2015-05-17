using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;


//This contains all the classes acting as Model
namespace App1.Data
{

    public class Message
    {

        [JsonProperty("msg_type")]
        public string msg_type { get; set; }


        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("ts")]
        public DateTime ts { get; set; }

        [JsonProperty("first_name")]
        public string first_name { get; set; }


        [JsonProperty("last_name")]
        public string last_name { get; set; }


        [JsonProperty("email")]
        public string email { get; set; }


        public override string ToString()
        {
            return message;
        }




    }

    public class User
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        public override string ToString()
        {
            return FirstName + " " + LastName;
        }

    }


    public class UserLocation
    {
        [JsonProperty("email")]
        public string email { get; set; }
        [JsonProperty("first_name")]
        public string first_name { get; set; }
        [JsonProperty("last_name")]
        public string last_name { get; set; }
        [JsonProperty("latitude")]
        public Double? latitude { get; set; }

        [JsonProperty("longitude")]
        public Double? longitude { get; set; }
        [JsonProperty("accuracy")]
        public int? accuracy { get; set; }
        [JsonProperty("lastUpdated")]
        public DateTime? lastUpdated { get; set; }


    }


    public class LocationInfo
    {

        public LocationInfo(string e, string f, string l, Double lat, Double lon)
        {


            geo_point = new Geopoint(new BasicGeoposition() { Latitude = lat, Longitude = lon });

            email = e;
            first_name = f;
            last_name = l;
            user_info = new User();
            user_info.Email = email;
            user_info.FirstName = first_name;
            user_info.LastName = last_name;

        }
        public Geopoint geo_point { get; set; }
        public string email { get; set; }

        public User user_info { get; set; }

        public Windows.Foundation.Point Anchor { get { return new Windows.Foundation.Point(0.5, 1); } }

        public string first_name { get; set; }
        public string last_name { get; set; }

    }
    public class ChatAccount
    {

        string email;
        string password;
        List<string> messages = new List<string>();
        string first_name;
        string last_name;
        private static ChatAccount account_instance = null;



        private ChatAccount(string e, string p, string f, string l)
        {
            email = e;
            password = p;
            first_name = f;
            last_name = l;
        }

        public static ChatAccount getInstance(string e, string p, string f, string l)
        {

            account_instance = new ChatAccount(e, p, f, l);

            return account_instance;


        }


        public static ChatAccount getInstance()
        {


            return account_instance;


        }


        //Creating account
        public async Task<string> createAccount()
        {
            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=createAccount&email=" + email + "&password=" + password;


            req_uri = req_uri + "&first_name=" + first_name;

            req_uri = req_uri + "&last_name=" + last_name;

            System.Diagnostics.Debug.WriteLine("Create Account");
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();
            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string resp = readStream.ReadToEnd().ToString();

            response.Dispose();
            readStream.Dispose();

            return resp;
        }



        //Deleting account
        public async static Task deleteAccount(string email, string password)
        {
            System.Diagnostics.Debug.WriteLine("Deleting account");
            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=deleteAccount&email=" + email + "&password=" + password;
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();
            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);


            response.Dispose();
            readStream.Dispose();
        }

        //Getting messages
        public async Task<ObservableCollection<Message>> getMessages()
        {
            System.Diagnostics.Debug.WriteLine("Get Messages");
            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=getMessages&email=" + email + "&password=" + password;
            System.Diagnostics.Debug.WriteLine(req_uri);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();
            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string _json = readStream.ReadToEnd();

            ObservableCollection<Message> all_messages = JsonConvert.DeserializeObject<ObservableCollection<Message>>(_json);


            response.Dispose();
            readStream.Dispose();

            return all_messages;

        }

        private DateTime lastTimeStamp = new DateTime();
        public async Task<Dictionary<string, ObservableCollection<Message>>> getMessagesFor()
        {

            ObservableCollection<Message> messages = await getMessages();
            var query = messages.Where(m => m.ts > lastTimeStamp).OrderBy(m => m.ts);
            Dictionary<string, ObservableCollection<Message>> messages_users = new Dictionary<string, ObservableCollection<Message>>();
            DateTime timestamp_last = lastTimeStamp;
            foreach (Message _m in query)
            {

                if (_m.ts > lastTimeStamp)
                {

                    if (!messages_users.ContainsKey(_m.email))
                    {
                        //Console.WriteLine("Adding to Dictionary " + _m.email);
                        messages_users.Add(_m.email, new ObservableCollection<Message>());
                        messages_users[_m.email].Add(_m);


                    }
                    else
                    {
                        //Console.WriteLine("Appending to List of " + _m.email);
                        messages_users[_m.email].Add(_m);

                    }


                    if (_m.ts > timestamp_last)
                        timestamp_last = _m.ts;

                }


            }


            lastTimeStamp = timestamp_last;
            return messages_users;


        }

        //Getting users
        public async Task<List<User>> getUsers()
        {
            System.Diagnostics.Debug.WriteLine("Get users");
            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=getUsers&email=" + email + "&password=" + password;
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string _json = readStream.ReadToEnd();
            if (_json.Equals("Invalid user"))
            {

                throw new InvalidUserException();
            }
            List<User> all_users = JsonConvert.DeserializeObject<List<User>>(_json);
            //Console.WriteLine(all_users.Count);


            response.Dispose();
            readStream.Dispose();
            return all_users;

        }




        ////Sending messages
        public async Task sendMessage(string message, string to)
        {

            System.Diagnostics.Debug.WriteLine("Send message");
            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=sendMessage&email=" + email + "&password=" + password + "&to=" + to + "&message=" + message;
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());




        }

        //Setting Location
        public async Task setLocation()
        {

            System.Diagnostics.Debug.WriteLine("Set Location");
            Geolocator loc = new Geolocator();
            Geoposition my_pos = await loc.GetGeopositionAsync();
            Geocoordinate co_Ord = my_pos.Coordinate;
            var pos = co_Ord.Point.Position;
            //System.Diagnostics.Debug.WriteLine(pos.Latitude + "," + pos.Longitude);


            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=setLocation&email=" + email + "&password=" + password + "&lat=" + pos.Latitude + "&long=" + pos.Longitude + "&acc=0";
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());


        }
        //Getting locations
        public async Task<ObservableCollection<UserLocation>> getLocations()
        {

            string req_uri = "http://www.cs.rit.edu/~jsb/2135/ProgSkills/Labs/Messenger/api.php?command=getLocations&email=" + email + "&password=" + password;
            System.Diagnostics.Debug.WriteLine(req_uri);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req_uri);
            request.Headers["If-Modified-Since"] = DateTime.Now.ToString();

            HttpWebResponse response = (HttpWebResponse)(await request.GetResponseAsync());
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            string _json = readStream.ReadToEnd();

            ObservableCollection<UserLocation> all_locations = JsonConvert.DeserializeObject<ObservableCollection<UserLocation>>(_json);
            //System.Diagnostics.Debug.WriteLine("Number of locations " + all_locations.Count);
            return all_locations;

        }

    }


    //Exception class for invalid user
    public class InvalidUserException : Exception
    {
        public InvalidUserException()
        {


        }




    }


}

