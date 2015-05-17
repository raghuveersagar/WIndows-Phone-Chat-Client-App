using System;
using App1.Data;
using App1.Common;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.UI.Xaml;


//This namespace contains the View Model classes

namespace App1.DataModel
{

    //Delegates for Contacts changed 
    public delegate void ContactListEventHandler(EventArgs e);
    public delegate void MapListEventHandler(EventArgs e);
    public delegate void MessageEventHandler(EventArgs e);


    //View Model class
    public class ChatViewModel
    {

        public event ContactListEventHandler contactListChanged;

        public event MessageEventHandler messagesChanged;

        private static ChatViewModel instance = null;


        private string username;
        public string Username
        {
            get { return username; }

            set
            {


                username = value;
            }

        }


        private string password;

        public string Password
        {

            get
            {
                return password;

            }
            set
            {


                password = value;


            }

        }

        private String first_name;
        public String FirstName
        {

            get { return first_name; }
            set { first_name = value; }


        }

        private String last_name;
        public String LastName
        {
            get
            {
                return last_name;
            }
            set
            {

                last_name = value;
            }

        }


        //Chat account of present user
        ChatAccount ch;

        //Contact List which can be binded in the view
        private ObservableCollection<User> contact_list = new ObservableCollection<User>();


        public ObservableCollection<User> ContactList
        {

            get { return contact_list; }
            set { contact_list = value; }


        }

        //Map List which can be binded in the view
        private ObservableCollection<LocationInfo> map_list = new ObservableCollection<LocationInfo>();
        public ObservableCollection<LocationInfo> MapList
        {

            get { return map_list; }
            set { map_list = value; }


        }


        //Dispatcher Timers for Conatcts and Mesages 
        DispatcherTimer users_timer = new DispatcherTimer();
        DispatcherTimer messages_timer = new DispatcherTimer();


        //Dictionary of users and their messages
        private Dictionary<string, ObservableCollection<Message>> all_messages = new Dictionary<string, ObservableCollection<Message>>();

        public Dictionary<string, ObservableCollection<Message>> All_Users_Messages
        {

            get { return all_messages; }
            set { all_messages = value; }


        }


        private ChatViewModel()
        {


            users_timer.Tick += getUsers;
            users_timer.Tick += getLocations;
            messages_timer.Tick += getMessages;
            users_timer.Interval = new TimeSpan(0, 1, 0);
            messages_timer.Interval = new TimeSpan(0, 0, 7);



        }



        //start the timers
        public void startBackgoundThread()
        {
            if (!users_timer.IsEnabled)
                users_timer.Start();
            if (!messages_timer.IsEnabled)
                messages_timer.Start();

        }


        //Method for delete account
        public async void deleteAccount()
        {


            await ChatAccount.deleteAccount(username, password);

        }

        //Method for setting up account
        public async Task<string> setUpAccount()
        {

            ch = ChatAccount.getInstance(username, password, first_name, last_name);
            string resp = await ch.createAccount();

            return resp;
        }


        //Method for getting users
        public async Task getUsers()
        {
            ch = ChatAccount.getInstance(username, password, first_name, last_name);
            List<User> users = await ch.getUsers();
            ContactList = new ObservableCollection<User>(users);



        }

        public async void getUsers(object sender, object e)
        {

            try
            {
                List<User> users = await ch.getUsers();

                ContactList = new ObservableCollection<User>(users);
                if (contactListChanged != null)
                    contactListChanged(EventArgs.Empty);

            }

            catch (JsonReaderException ex)
            {
                if (users_timer.IsEnabled)
                    users_timer.Stop();

                if (messages_timer.IsEnabled)
                    messages_timer.Stop();

            }
        }


        //Method for getting Locations
        public async Task getLocations()
        {

            MapList.Clear();
            ObservableCollection<UserLocation> user_locations = await ch.getLocations();
            foreach (var m in user_locations)
            {
                if (m.latitude != null && m.longitude != null)
                {


                    if ((m.latitude <= 90 && m.latitude >= -90) && (m.longitude <= 180 && m.longitude >= -180))
                        MapList.Add(new LocationInfo(m.email, m.first_name, m.last_name, Convert.ToDouble(m.latitude), Convert.ToDouble(m.longitude)));



                }
            }
            //mapListChanged(EventArgs.Empty);

        }


        public async void getLocations(object sender, object e)
        {
            try
            { 
            MapList.Clear();
            ObservableCollection<UserLocation> user_locations = await ch.getLocations();
            foreach (var m in user_locations)
            {
                if (m.latitude != null && m.longitude != null)
                {


                    if ((m.latitude <= 90 && m.latitude >= -90) && (m.longitude <= 180 && m.longitude >= -180))
                        MapList.Add(new LocationInfo(m.email, m.first_name, m.last_name, Convert.ToDouble(m.latitude), Convert.ToDouble(m.longitude)));



                }
            }

            }

            catch(JsonReaderException ex)
            {
                if (users_timer.IsEnabled)
                    users_timer.Stop();

                if (messages_timer.IsEnabled)
                    messages_timer.Stop();

            }
        }


        //Method for getting Messages
        public async void getMessages(object sender, object e)
        {

            try { 
            Dictionary<string, ObservableCollection<Message>> mess = await ch.getMessagesFor();
            foreach (KeyValuePair<string, ObservableCollection<Message>> k in mess)
            {

                if (all_messages.ContainsKey(k.Key))
                {
                    foreach (Message m in k.Value)
                    { 
                        all_messages[k.Key].Add(m);
                        if (messagesChanged != null)
                            messagesChanged(EventArgs.Empty);
                    }

                }
                else
                { 
                    all_messages.Add(k.Key, k.Value);
                    if (messagesChanged != null)
                        messagesChanged(EventArgs.Empty);

                }
            }

            if (messagesChanged != null)
                messagesChanged(EventArgs.Empty);
            messages_timer.Stop();
            messages_timer.Interval = new TimeSpan(0, 0, 7);
            messages_timer.Start();
        }

            catch (JsonReaderException ex)
            {
                if (users_timer.IsEnabled)
                    users_timer.Stop();

                if (messages_timer.IsEnabled)
                    messages_timer.Stop();

            }

        }


        //Method for sending Messages
        public async Task sendMessage(string message, string to)
        {

            await ch.sendMessage(message, to);
            messages_timer.Stop();
            messages_timer.Interval = new TimeSpan(0, 0, 1);
            messages_timer.Start();

        }

        public async Task setLocation()
        {

            await ch.setLocation();

        }



        public static ChatViewModel getInstance()
        {

            if (instance == null)
                return (instance = new ChatViewModel());

            else
                return instance;
        }





    }



}
