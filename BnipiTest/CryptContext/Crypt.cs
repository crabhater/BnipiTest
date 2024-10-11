using BnipiTest.Models;
using BnipiTest.Models.InterfacesLib;
using System.Buffers.Text;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using static BnipiTest.CryptContext.WinApi;

namespace BnipiTest.CryptContext
{
    public class Crypt : ICrypt
    {
        public ICryptArgs GetCryptArgs(string thumbprint, string signedCms, string? cms = null)
        {
            var cert = GetCertByThumbPrint(thumbprint);
            var cryptArgs = new CryptArgs()
            {
                Certificate = cert,
                SignedCMS = Convert.FromBase64String(signedCms)
            };
            return cryptArgs;
        }
        private X509Certificate2 GetCertByThumbPrint(string thumbprint)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Этот код работает только на винде");
            }
            var certificates = WinApi.GetCertByThumbPrint(thumbprint, false, true);
            return certificates.Where(cert => cert.NotAfter < DateTime.Now).FirstOrDefault();
        }

        public byte[] Sign(ICryptArgs args)
        {
            throw new NotImplementedException();
        }

        public bool VerifyAttached(ICryptArgs args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Этот код работает только на винде");
            }
            CRYPT_VERIFY_MESSAGE_PARA verifyParams = new CRYPT_VERIFY_MESSAGE_PARA();
            verifyParams.cbSize = Marshal.SizeOf(verifyParams);
            verifyParams.dwMsgAndCertEncodingType = 1;


            byte[] decoded = new byte[8192];
            int decodedLength = decoded.Length;
            IntPtr signerCert = IntPtr.Zero;

            bool result = WinApi.CryptVerifyMessageSignature(
                ref verifyParams,
                0,
                args.SignedCMS,
                args.SignedCMS.Length,
                decoded,
                ref decodedLength,
                ref signerCert);

            return result;
        }
        public bool VerifyDetached(ICryptArgs args)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException("Этот код работает только на винде");
            }
            GCHandle handle = GCHandle.Alloc(args.RawData, GCHandleType.Pinned);
            List<byte[]> signers = new List<byte[]>(); 
            try
            {

                CRYPT_VERIFY_MESSAGE_PARA verifyParams = new CRYPT_VERIFY_MESSAGE_PARA();
                verifyParams.cbSize = Marshal.SizeOf(verifyParams);
                verifyParams.dwMsgAndCertEncodingType = 65537;

                int i = 0;
                IntPtr signerCert;
                while (WinApi.CryptVerifyDetachedMessageSignature(ref verifyParams, 
                                                                  i, 
                                                                  args.SignedCMS,
                                                                  args.SignedCMS.Length, 
                                                                  1, 
                                                                  new IntPtr[1] { handle.AddrOfPinnedObject() }, 
                                                                  new int[1] { args.RawData.Length }, 
                                                                  out signerCert));
                {
                    signers.Add(WinApi.GetBinaryCert(signerCert));
                    WinApi.CertFreeCertificateContext(signerCert);
                    i++;
                }
            }
            finally
            {
                handle.Free();
            }
            return  Convert.ToBase64String(signers.First()) == Convert.ToBase64String(args.Certificate.RawData);//Вытащили сертификаты из подписи и проверяем есть ли наш
        }


        public void SaveCert(string base64Cert)
        {
            WinApi.SaveCert(base64Cert);
        }
    }
}
