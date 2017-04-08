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
        private AsymmetricCipherKeyPair _ACKeypair = null;
        static X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
        private HashSet<string> _ownLocks;
        private HashSet<string> _authorizedLocks;
        private string _privateKey;


        public string PublicKey
        {
            get
            {
                return "0x" + EthECKey.GetPublicAddress(_privateKey);
            }
        }

        public User()
        { }

        public User(string username)
        {
            _username = username;
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
