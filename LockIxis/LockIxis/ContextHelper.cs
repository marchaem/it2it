using Android.Content;
using Android.Preferences;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockIxis
{
    public static class ContextHelper
    {
        //var context = Android.App.Application.Context;
        //ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        //var aaa = prefs.GetString("userId", "");
        public static string GetSharedPreference(this Context c, string key)
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(c);
            var res = prefs.GetString(key, "NOT FOUND");
            return res;
        }
    }
}
