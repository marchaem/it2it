using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace LockIxis
{
    public class Lock
    {
        public enum LockStatus
        {
            Open,
            AboutToOpen,
            AboutToClose,
            Closed
        }

        private ECPublicKeyParameters _publicKeyParams;

        public Lock()
        {

        }
    }
}
