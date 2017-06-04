using Android.Bluetooth;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockIxis
{
    public class ConnectedLock 
    {
        public enum LockStatus
        {
            Open,
            AboutToOpen,
            AboutToClose,
            Closed
        }

        private BluetoothDevice _btdevice;
        private string _publicKey = null;
        private string _status = "";

        public ConnectedLock(string publicKey, BluetoothDevice bt)
        {
            _publicKey = publicKey;
            _btdevice = bt;
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _publicKey.Length > 0 && _btdevice != null;
            }
        }

        public string PublicKey
        {
            get
            {
                if (_publicKey != null)
                {
                    return _publicKey;
                }
                return null;
            }
            set
            {
                _publicKey = value;
            }
        }

        public string BTDeviceName
        {
            get
            {
                if (_btdevice != null)
                {
                    return _btdevice.Name;
                }
                return "";
            }
        }

        public string BTDeviceAddress
        {
            get
            {
                if (_btdevice != null)
                {
                    return _btdevice.Address;
                }
                return "";
            }
        }

        public BluetoothDevice BTDevice
        {
            get
            {
                if (_btdevice != null)
                {
                    return _btdevice;
                }
                return null;
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
