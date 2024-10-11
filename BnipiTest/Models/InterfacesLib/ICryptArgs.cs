using System.Security.Cryptography.X509Certificates;

namespace BnipiTest.Models.InterfacesLib
{
    public interface ICryptArgs
    {
        X509Certificate2 Certificate { get; set; }
        public byte[]? SignedCMS { get; set; }
        public byte[]? RawData { get; set; }
    }
}
