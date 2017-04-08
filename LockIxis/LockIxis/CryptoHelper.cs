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
    public static class CryptoHelper
    {
        public static AsymmetricCipherKeyPair KeyGenerator()
        {
            //PrivateKeyFactory aa = new PrivateKeyFactory()

            X9ECParameters ec = SecNamedCurves.GetByName("secp256k1");
            ECDomainParameters domainParams = new ECDomainParameters(ec.Curve, ec.G, ec.N, ec.H);
            SecureRandom random = new SecureRandom();

            // Generate EC KeyPair
            ECKeyPairGenerator keyGen = new ECKeyPairGenerator();
            ECKeyGenerationParameters keyParams = new ECKeyGenerationParameters(domainParams, random);
            keyGen.Init(keyParams);
            AsymmetricCipherKeyPair keyPair = keyGen.GenerateKeyPair();

            ECPrivateKeyParameters privateKeyParams = keyPair.Private as ECPrivateKeyParameters;
            ECPublicKeyParameters publicKeyParams = keyPair.Public as ECPublicKeyParameters;

            // Get Private Key
            BigInteger privD = privateKeyParams.D;
            byte[] privBytes = privD.ToByteArray();

            if (privBytes.Length == 33)
            {
                var temp = new byte[32];
                Array.Copy(privBytes, 1, temp, 0, 32);
                privBytes = temp;
            }
            else if (privBytes.Length < 32)
            {
                var temp = Enumerable.Repeat<byte>(0x00, 32).ToArray();
                Array.Copy(privBytes, 0, temp, 32 - privBytes.Length, privBytes.Length);
                privBytes = temp;
            }
            string privKey = BitConverter.ToString(privBytes).Replace("-", "");

            // Get Compressed Public Key
            ECPoint q = publicKeyParams.Q;
            FpPoint fp = new FpPoint(ec.Curve, q.AffineXCoord, q.AffineYCoord);
            byte[] enc = fp.GetEncoded(true);
            string compressedPubKey = BitConverter.ToString(enc).Replace("-", "");

            // Get Uncompressed Public Key
            enc = fp.GetEncoded(false);
            string uncompressedPubKey = BitConverter.ToString(enc).Replace("-", "");

            // Output
            //Console.WriteLine(privKey);
            //Console.WriteLine(compressedPubKey);
            //Console.WriteLine(uncompressedPubKey);

            return keyPair;
        }
    }
}
