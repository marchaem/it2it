using Android.Content;
using Android.Preferences;
using Nethereum.Core.Signing.Crypto;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LockIxis
{
    public class User
    {
        private string _username;
        private string _password;
        private AsymmetricCipherKeyPair _ACKeypair = null;
        static X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
        private HashSet<string> _locks = new HashSet<string>();
        private HashSet<string> _transactions = new HashSet<string>();
        private HashSet<string> _authorizedLocks = new HashSet<string>();
        private string _privateKey;
        private string _publicKey;

        public string Password
        {
            get { return _password; }
        }

        public int NumberofLocks
        {
            get { return _locks.Count; }
        }

        public int NumberofTransactions
        {
            get { return _transactions.Count; }
        }

        public HashSet<string> Locks
        {
            get { return _locks; }
            set { _locks = value; }
        }

        public HashSet<string> AuthorizedLocks
        {
            get { return _authorizedLocks; }
            set { _authorizedLocks = value; }
        }

        public HashSet<string> Transactions
        {
            get { return _transactions; }
        }

        public string PublicKey
        {
            get
            {
                if (_publicKey == null)
                {
                    return "0x" + EthECKey.GetPublicAddress(_privateKey);
                }
                return _publicKey;
            }
            set
            {
                _publicKey = value;
            }
        }
        public void AddLock(string address)
        {
            _locks.Add(address);
            LockIxisApp.GetPrefsEditor().PutStringSet(("userLocks"), _locks);
            LockIxisApp.GetPrefsEditor().Apply();
        }

        public void AddTransaction(string address)
        {
            _transactions.Add(address);
        }

        public User()
        { }

        //public override string ToString()
        //{
        //    return PublicKey;
        //}

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public string PrivateKey
        {
            get
            {
                return _privateKey;
            }
            set
            {
                _privateKey = value;
            }
        }

        public string Name
        {
            get { return _username; }
            set { _username = value; }
        }

        public void InitialiseCryptoParameters(string qrCode_privateKey)
        {
            //"0x" + EthECKey.GetPublicAddress(qrCode_privateKey)
            //var ethECKey = new EthECKey(qrCode_privateKey);
            _privateKey = qrCode_privateKey;
        }

        public bool IsSet
        {
            get { return Name.Length > 0 && _ACKeypair != null; }
        }
    }
}
