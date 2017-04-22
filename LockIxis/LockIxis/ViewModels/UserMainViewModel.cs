using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace LockIxis.ViewModels
{
    public class UserMainViewModel : BaseViewModel
    {
        private User _user;
        public UserMainViewModel(User u)
        {
            _user = u;
        }

        public string Username
        {
            get { return String.Format("{0}\r\n@aaaa.com",_user.Name); }
        }

        public string PublicKey
        {
            get { return _user.PublicKey; }
        }

        public string NumberofTransactions
        {
            get
            {
                OnPropertyChanged("NumberofTransactions");
                return String.Format("{0}", _user.NumberofTransactions);
            }
        }

        public string NumberofLocks
        {
            get
            {
                OnPropertyChanged("NumberofLocks");
                return String.Format("{0}", _user.NumberofLocks); ;
            }
        }
    }
}
