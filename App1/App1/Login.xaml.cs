using App1.Common;
using App1.DataModel;
using App1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Devices.Geolocation;
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
    public sealed partial class Login : Page
    {
        private NavigationHelper navigationHelper;
        private ChatViewModel defaultViewModel = null;
        private bool signUpChecker = false;
        private bool deleteUserChecker = false;
        Visibility visibility_signUp = Visibility.Collapsed;

        public Visibility VisibilitySignUp
        {

            get { return visibility_signUp; }
            set
            {
                visibility_signUp = value;
            }


        }

        public bool isDeleteUser
        {

            get { return deleteUserChecker; }
            set { deleteUserChecker = value; }



        }
        public bool IsSignUp
        {

            get { return signUpChecker; }
            set
            {

                signUpChecker = value;
                if (value)
                {

                    firstNameTxtBlk.Visibility = Visibility.Visible;
                    firstNameTxtBx.Visibility = Visibility.Visible;
                    lastNameTxtBlk.Visibility = Visibility.Visible;
                    secondNameTxtBx.Visibility = Visibility.Visible;
                }
                else
                {
                    firstNameTxtBlk.Visibility = Visibility.Collapsed;
                    firstNameTxtBx.Visibility = Visibility.Collapsed;
                    lastNameTxtBlk.Visibility = Visibility.Collapsed;
                    secondNameTxtBx.Visibility = Visibility.Collapsed;


                }
            }

        }

        public Login()
        {
            //Set View Model
            defaultViewModel = ChatViewModel.getInstance();
            this.InitializeComponent();

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
        public ChatViewModel DefaultViewModel
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
        //Handle after done button clicked
        private async void done_Button_clicked(object sender, RoutedEventArgs e)
        {

            //System.Diagnostics.Debug.WriteLine("Username " + DefaultViewModel.Username);
            //System.Diagnostics.Debug.WriteLine("Password " + DefaultViewModel.Password);
            //System.Diagnostics.Debug.WriteLine("FirstName " + DefaultViewModel.FirstName);
            //System.Diagnostics.Debug.WriteLine("LastName " + DefaultViewModel.LastName);


            if (isDeleteUser && IsSignUp)
            {
                loginStatusTextBlk.Text = "Check only one field";
                loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                loginStatusTextBlk.Visibility = Visibility.Visible;
                contactListBtn.Visibility = Visibility.Collapsed;
                mapsBtn.Visibility = Visibility.Collapsed;
                repostLocationBtn.Visibility = Visibility.Collapsed;

                return;


            }

            if (isDeleteUser)
            {


                DefaultViewModel.deleteAccount();
                loginStatusTextBlk.Text = "Account Deleted";
                loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                loginStatusTextBlk.Visibility = Visibility.Visible;
                contactListBtn.Visibility = Visibility.Collapsed;
                mapsBtn.Visibility = Visibility.Collapsed;
                repostLocationBtn.Visibility = Visibility.Collapsed;
                return;


            }


            if (IsSignUp && firstNameTxtBx.Text.Equals("") && secondNameTxtBx.Text.Equals(""))
            {
                loginStatusTextBlk.Text = "No blank fields";
                loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                loginStatusTextBlk.Visibility = Visibility.Visible;
                contactListBtn.Visibility = Visibility.Collapsed;
                mapsBtn.Visibility = Visibility.Collapsed;
                repostLocationBtn.Visibility = Visibility.Collapsed;

                return;



            }



            //System.Diagnostics.Debug.WriteLine(resp);

            if (IsSignUp)
            {

                string resp = await DefaultViewModel.setUpAccount();
                System.Diagnostics.Debug.WriteLine(resp);
                if (resp.Equals("Account already exists"))
                {
                    loginStatusTextBlk.Text = "Account already exists";
                    loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    loginStatusTextBlk.Visibility = Visibility.Visible;
                    contactListBtn.Visibility = Visibility.Collapsed;
                    mapsBtn.Visibility = Visibility.Collapsed;
                    repostLocationBtn.Visibility = Visibility.Collapsed;

                }
                else if (resp.Equals("Account Created"))
                {

                    await DefaultViewModel.getUsers();
                    loginStatusTextBlk.Text = "Account Created";
                    loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                    loginStatusTextBlk.Visibility = Visibility.Visible;
                    contactListBtn.Visibility = Visibility.Visible;
                    mapsBtn.Visibility = Visibility.Visible;
                    repostLocationBtn.Visibility = Visibility.Visible;


                }
            }
            else
            {

                try
                {
                    await DefaultViewModel.getUsers();

                    loginStatusTextBlk.Text = "Login Success";
                    loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Green);
                    loginStatusTextBlk.Visibility = Visibility.Visible;

                    contactListBtn.Visibility = Visibility.Visible;
                    mapsBtn.Visibility = Visibility.Visible;
                    repostLocationBtn.Visibility = Visibility.Visible;
                }
                catch (InvalidUserException ex)
                {

                    loginStatusTextBlk.Text = "Invalid User";
                    loginStatusTextBlk.Foreground = new SolidColorBrush(Windows.UI.Colors.Red);
                    loginStatusTextBlk.Visibility = Visibility.Visible;
                    contactListBtn.Visibility = Visibility.Collapsed;
                    mapsBtn.Visibility = Visibility.Collapsed;
                    repostLocationBtn.Visibility = Visibility.Collapsed;



                }
            }






        }

        #endregion


        private void contactList_Clicked(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ContactList));
            //Frame.Navigate(typeof(ItemPage), itemId)


        }

        private async void repostLocationClicked(object sender, RoutedEventArgs e)
        {
            await DefaultViewModel.setLocation();
        }

        private async void maps_clicked(object sender, RoutedEventArgs e)
        {
            await DefaultViewModel.getLocations();
            Frame.Navigate(typeof(MapView));
            //Frame.Navigate(typeof(ItemPage), itemId)


        }

        private void signUp_Checked(object sender, RoutedEventArgs e)
        {
            IsSignUp = true;
        }


        private void signUp_UnChecked(object sender, RoutedEventArgs e)
        {
            IsSignUp = false;
        }
        private void deleteUser_Checked(object sender, RoutedEventArgs e)
        {
            deleteUserChecker = true;
        }


        private void deleteUser_UnChecked(object sender, RoutedEventArgs e)
        {
            deleteUserChecker = false;
        }


    }
}
