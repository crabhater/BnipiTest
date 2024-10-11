using System.Security.Cryptography.X509Certificates;

namespace BnipiTest.Models.InterfacesLib
{
    public interface ICrypt
    {
        bool VerifyAttached(ICryptArgs args);
        bool VerifyDetached(ICryptArgs args);
        byte[] Sign(ICryptArgs args);
        ICryptArgs GetCryptArgs(string thumbprint, string signedCms, string? cms = null);
        void SaveCert(string base64Cert);
    }
}
