using Xamarin.Forms;
using System;
using LockIxis.Pages;
using Android.Preferences;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using System.Collections.Generic;
using Android.Content;
using Android.Bluetooth;
using Java.Util;

namespace LockIxis
{
    public partial class LockIxisApp : Application
    {
        private static LockIxisApp _instance = null;
        private static User _currentUser = null;
        private static ISharedPreferencesEditor _editor = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context).Edit();
        private static BluetoothAdapter _bluetoothAdapter = null;
        public static UUID MY_UUID = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB");
        private static BluetoothSocket _btSocket = null;

        public LockIxisApp()
        {
            global::Xamarin.Forms.Forms.SetTitleBarVisibility(Xamarin.Forms.AndroidTitleBarVisibility.Never);
            InitializeComponent();
            var context = Android.App.Application.Context;
            var userName = context.GetSharedPreference("userId");
            var userPassword = context.GetSharedPreference("userPassword");
            if (!userName.Equals("NOT FOUND") &&  (userPassword.Length > 0 && !userPassword.Equals("NOT FOUND")))
            {
                _currentUser = new User(userName, userPassword);
                //user.PublicKey = context.GetSharedPreference("publickey");
                _currentUser.PrivateKey = context.GetSharedPreference("privatekey");
                if (context.GetSharedPreferenceSet("userLocks") != null)
                {
                    _currentUser.Locks = new HashSet<string>(context.GetSharedPreferenceSet("userLocks"));
                }
                MainPage = new NavigationPage ();
                MainPage.Navigation.PushAsync(new UserMainPage(_currentUser));
            }
            else
            {
                var regPage = new RegisterPage();
                MainPage = new NavigationPage(regPage);
                //MainPage.Navigation.PushAsync(regPage);
            }
        }

        public static ISharedPreferencesEditor GetPrefsEditor()
        {
            return _editor;
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
            var userId = Android.App.Application.Context.GetSharedPreference("userId");
            if (userId.Equals("NOT FOUND"))
            {
                SetCurrentUser(user);
                _currentUser = user;
            }
            return new UserMainPage(user);
        }

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
            _editor.PutString("userId", _currentUser.Name);
            _editor.PutString("privatekey", _currentUser.PrivateKey);
            _editor.PutString("publickey", _currentUser.PublicKey);
            _editor.PutString("userPassword", _currentUser.Password);
            _editor.PutStringSet(("userLocks"), new List<string>() { ""});
            _editor.Apply();
        }

        public static BluetoothAdapter GetBluetoothAdapter()
        {
            if (_bluetoothAdapter == null)
            {
                _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
            }
            return _bluetoothAdapter;
        }

        public static BluetoothSocket GetBTSocket()
        {
            if (_btSocket != null)
            {
                return _btSocket;
            }
            return null;
        }

        public static BluetoothSocket SetAndGetBTSocket(BluetoothSocket bts)
        {
            if(_btSocket == null)
            {
                _btSocket = bts;
            }
            return _btSocket;
        }
    }
}
