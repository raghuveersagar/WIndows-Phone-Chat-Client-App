using App1.Common;
using System;
using App1.Data;
using App1.DataModel;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace App1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

     

    public sealed partial class UserMessages : Page
    {
        private NavigationHelper navigationHelper;
        private string user_email;
        public string User_Email
        {

            get
            {
                return user_email;
            }

            set
            {
                user_email = value;
            }

        }

        private string name;
        
        public string NameOfUser
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

            }

        } 
        private ObservableCollection<Message> defaultViewModel = new ObservableCollection<Message>();


        //Delegate which is called when messages change
        public void updateMessages(EventArgs e)
        {
            
            
            if (ChatViewModel.getInstance().All_Users_Messages.ContainsKey(User_Email))
            {
                

                if (ChatViewModel.getInstance().All_Users_Messages[User_Email] != DefaultViewModel)
                {
                    defaultViewModel = ChatViewModel.getInstance().All_Users_Messages[User_Email];
                    Binding new_bind = new Binding();
                    new_bind.Source = DefaultViewModel;
                    new_bind.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(messagesListView, ListView.ItemsSourceProperty, new_bind);


                }
            }

            if((messagesListView.Items.Count - 1) > 0)
            messagesListView.ScrollIntoView(messagesListView.Items[messagesListView.Items.Count - 1]);
        }

      


            public void scrollToBottom()
        {

            if ((messagesListView.Items.Count - 1) > 0)
                messagesListView.ScrollIntoView(messagesListView.Items[messagesListView.Items.Count - 1]);


        }

            public HorizontalAlignment  AlignTypeVar
        {

            get { return HorizontalAlignment.Right; }

        }
        public UserMessages()
        {
            this.InitializeComponent();
            
           ChatViewModel.getInstance().messagesChanged += updateMessages;
           this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableCollection<Message> DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //user_email = "pp2157@cs.rit.edu";
            User u = (User)e.NavigationParameter;
            User_Email = u.Email;
            NameOfUser = u.FirstName +" "+ u.LastName;
            

            if(ChatViewModel.getInstance().All_Users_Messages.ContainsKey(User_Email))
            { 

            if(ChatViewModel.getInstance().All_Users_Messages[User_Email] != DefaultViewModel )
                {
                    defaultViewModel = ChatViewModel.getInstance().All_Users_Messages[User_Email];
                    Binding new_bind = new Binding();
                    new_bind.Source = DefaultViewModel;
                    new_bind.Mode = BindingMode.TwoWay;
                    BindingOperations.SetBinding(messagesListView, ListView.ItemsSourceProperty, new_bind);


                }


            
            }
            if ((messagesListView.Items.Count - 1) > 0)
                messagesListView.ScrollIntoView(messagesListView.Items[messagesListView.Items.Count - 1]);

        }




        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {


            
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {

            
            this.navigationHelper.OnNavigatedFrom(e);
        }



        private async void sendMessageClicked(object sender, RoutedEventArgs e)
        {

            string message = sendMessageTxtBx.Text;
            await ChatViewModel.getInstance().sendMessage(message,user_email);
            sendMessageTxtBx.Text = "";


        }

        #endregion
    }


    public class MsgTypeToAlign :IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            if (value.ToString().Equals("from"))
                return new SolidColorBrush(Windows.UI.Colors.LightGray);
            else
                return new SolidColorBrush(Windows.UI.Colors.LightBlue);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            

            throw new NotImplementedException();
        }
    }


    public class MsgTypeToMargin : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            
            if (value.ToString().Equals("from"))
                return new Thickness(70, 20, 20, 20);
            else
                return new Thickness(10, 20, 20, 20);

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            

            throw new NotImplementedException();
        }
    }

}
