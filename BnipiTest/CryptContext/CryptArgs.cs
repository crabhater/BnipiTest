using BnipiTest.Models.InterfacesLib;
using System.Security.Cryptography.X509Certificates;

namespace BnipiTest.CryptContext
{
    public class CryptArgs : ICryptArgs
    {
        public X509Certificate2 Certificate { get ; set ; }
        public byte[]? SignedCMS { get ; set ; }
        public byte[]? RawData { get ; set ; }
    }
}
