using System;
using System.Collections.Generic;
using System.Text;

namespace LockIxis.ViewModels
{
    public class ConnectedLockViewModel : BaseViewModel
    {
        private ConnectedLock _lock;
        private string _status;

        public ConnectedLockViewModel(ConnectedLock l)
        {
            _lock = l;
        }



        public string Status
        {
            get
            {
                return _lock.Status;
            }
            set
            {
                _status = _lock.Status;
                OnPropertyChanged("Text");
            }
        }


        public string Text
        {
            get
            {
                return "prout" + Status;
                //return String.Format("Lock: \n*Bluetooth device name:{0}\n*Public key:{1}\n*Status:{2}\n", _lock.BTDeviceAddress, Status);
            }
        }
    }
}
