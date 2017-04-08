using Xamarin.Forms;
using System;
using LockIxis.Pages;
using Android.Preferences;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using System.Collections.Generic;
using Android.Content;

namespace LockIxis
{
    public partial class LockIxisApp : Application
    {
        private static LockIxisApp _instance = null;

        public LockIxisApp()
        {
            global::Xamarin.Forms.Forms.SetTitleBarVisibility(Xamarin.Forms.AndroidTitleBarVisibility.Never);
            InitializeComponent();
            var context = Android.App.Application.Context;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var userName = context.GetSharedPreference("userId");
            if(!userName.Equals("NOT FOUND"))
            {
                var user = new User(userName);
                //user.PublicKey = context.GetSharedPreference("publickey");
                user.PrivateKey = context.GetSharedPreference("privatekey");
                MainPage = new NavigationPage ();
                MainPage.Navigation.PushAsync(new UserMainPage(user));
            }
            else
            {
                var regPage = new RegisterPage();
                MainPage = new NavigationPage(regPage);
            }
        }

        public static LockIxisApp GetInstance()
        {
            if(_instance == null)
            {
                _instance = new LockIxisApp();
            }
            return _instance;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        public static MasterDetailPage FetchMainUI(User user)
        {
            var context = Android.App.Application.Context;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var aaa = prefs.GetString("userId", "");
            var aaaaa = user.PublicKey;
            if (aaa.Equals(""))
            {
                SetCurrentUser(user);
                _currentUser = user;
            }
            return new UserMainPage(user);
        }


        private static User _currentUser;



        public static User CurrentUser()
        {
            if(_currentUser != null)
            {
                return _currentUser;
            }
            return null;
        }

        public static void SetCurrentUser(User u)
        {
            _currentUser = u;
            var context = Android.App.Application.Context;
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            var aaa = prefs.GetString("userId", "");

            ISharedPreferencesEditor editor = prefs.Edit();
            editor.PutString("userId", _currentUser.Name);
            editor.PutString("privatekey", _currentUser.PrivateKey);
            editor.PutString("publickey", _currentUser.PublicKey);
            editor.Apply();
            aaa = prefs.GetString("userId", "");
            int i = 0;
            i++;
        }
    }
}
