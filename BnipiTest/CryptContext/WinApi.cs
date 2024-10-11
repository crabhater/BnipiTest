using BnipiTest.Models.InterfacesLib;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace BnipiTest.CryptContext
{
    [SecurityCritical]
    internal static class WinApi
    {
        public const int CRYPT_EXPORTABLE = 0x00000001;
        public const int CRYPT_USER_KEYSET = 0x00001000;
        public const int PKCS12_NO_PERSIST_KEY = 0x00008000;//Без закрытого ключа
        public const int PKCS12_IMPORT_SILENT = 0x00000040;//Без запроса пароля
        public const int PKCS12_ALLOW_OVERWRITE_KEY = 0x00004000;
        public const int PKCS12_INCLUDE_EXTENDED_PROPERTIES = 0x0010;

        /// <summary>
        /// Проверит только прикрепленную подпись, не сработает если подпись открепленная
        /// </summary>
        /// <param name="pVerifyPara"></param>
        /// <param name="dwSignerIndex"></param>
        /// <param name="pbSignedBlob"></param>
        /// <param name="cbSignedBlob"></param>
        /// <param name="pbDecoded"></param>
        /// <param name="pcbDecoded"></param>
        /// <param name="ppSignerCert"></param>
        /// <returns></returns>
        [DllImport("Crypt32.dll", SetLastError = true)]
        public static extern bool CryptVerifyMessageSignature(//https://learn.microsoft.com/ru-ru/windows/win32/api/wincrypt/nf-wincrypt-cryptverifymessagesignature
        ref CRYPT_VERIFY_MESSAGE_PARA pVerifyPara,
        int dwSignerIndex,
        byte[] pbSignedBlob,
        int cbSignedBlob,
        byte[] pbDecoded,
        ref int pcbDecoded,
        ref IntPtr ppSignerCert);
        /// <summary>
        /// Проверит только открепленную подпись, не сработает если подпись прикрепленная
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="signerIndex"></param>
        /// <param name="signature"></param>
        /// <param name="signatureSize"></param>
        /// <param name="contentsCount"></param>
        /// <param name="contents"></param>
        /// <param name="contentsSizes"></param>
        /// <param name="certificate"></param>
        /// <returns></returns>
        [DllImport("Crypt32.dll", SetLastError = true)]
        public static extern bool CryptVerifyDetachedMessageSignature(//https://learn.microsoft.com/ru-ru/windows/win32/api/wincrypt/nf-wincrypt-cryptverifydetachedmessagesignature
            ref CRYPT_VERIFY_MESSAGE_PARA parameters, 
            int signerIndex, 
            byte[] signature, 
            int signatureSize, 
            int contentsCount, 
            IntPtr[] contents, 
            int[] contentsSizes, 
            out IntPtr certificate
            );

        [DllImport("Crypt32.dll", SetLastError = true)]
        public static extern bool CryptSignMessage(//https://learn.microsoft.com/ru-ru/windows/win32/api/wincrypt/nf-wincrypt-cryptsignmessage
        ref CRYPT_SIGN_MESSAGE_PARA pSignPara,
        bool fDetachedSignature,
        int cToBeSigned,
        IntPtr[] rgpbToBeSigned,
        int[] rgcbToBeSigned,
        byte[] pbSignedBlob,
        ref int pcbSignedBlob);
        [DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr PFXImportCertStore(
        ref CRYPT_DATA_BLOB pPfx, 
        [MarshalAs(UnmanagedType.LPWStr)] string password, 
        int dwFlags);
        [DllImport("Crypt32.dll")]
        public static extern bool CertFreeCertificateContext(IntPtr certificate);//https://learn.microsoft.com/ru-ru/windows/win32/api/wincrypt/nf-wincrypt-certfreecertificatecontext

        public static byte[] GetBinaryCert(IntPtr certificate)
        {
            CERT_CONTEXT context = (CERT_CONTEXT)Marshal.PtrToStructure(certificate, typeof(CERT_CONTEXT));
            byte[] array = new byte[context.encodedCertificateSize];
            Marshal.Copy(context.encodedCertificate, array, 0, context.encodedCertificateSize);
            return array;
        }

        public static void SaveCert(string base64Cert)
        {
            byte[] certB = Convert.FromBase64String(base64Cert);
            var certificate = GetCertFromFile_Silent(certB);
            using X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(certificate);
            store.Close();
        }
        public static List<X509Certificate2> GetCertByThumbPrint(string thumbprint, bool useLocalSystemStorage = false, bool onlyWithPrivateKey = true)
        {
            X509Store x509Store = new X509Store("MY", (!useLocalSystemStorage) ? StoreLocation.CurrentUser : StoreLocation.LocalMachine);
            x509Store.Open(OpenFlags.ReadOnly);
            try
            {
                return (from X509Certificate2 c in x509Store.Certificates
                        where !onlyWithPrivateKey || c.HasPrivateKey
                        select c).ToList();
            }
            finally
            {
                x509Store.Close();
            }
        }
        private static X509Certificate2 GetCertFromFile_Silent(byte[] data)
        {
            byte[] pfxData = data;//System.IO.File.ReadAllBytes(pfxFile);
            CRYPT_DATA_BLOB blob = new CRYPT_DATA_BLOB();//Создаем блоб для хранения информации о сертификате
            blob.cbData = pfxData.Length;
            blob.pbData = Marshal.AllocHGlobal(pfxData.Length);
            Marshal.Copy(pfxData, 0, blob.pbData, pfxData.Length);
            IntPtr hStore = PFXImportCertStore(ref blob, (string)null, CRYPT_EXPORTABLE
                                                                     | CRYPT_USER_KEYSET
                                                                     | PKCS12_IMPORT_SILENT);//Silent-режим импорта. Смотри документацию и ни в коем случае не убирай.
            Marshal.FreeHGlobal(blob.pbData);
            var certs = new X509Certificate2Collection();
            if (hStore == IntPtr.Zero)
            {
                throw new Exception("Сертификат не распознан!");
            }
            using var store = new X509Store(hStore);//Открываем хранилище по указанному дескриптору
            var certsCol = store.Certificates;
            var cert = certsCol.FirstOrDefault();//Должен быть всего 1 сертификат, либо цепочка сертификации
            //Marshal.FreeHGlobal(hStore);
            Marshal.FreeHGlobal(blob.cbData);//Освобождаем память
            return cert;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CERT_CONTEXT
        {
            public int encoding;

            public IntPtr encodedCertificate;

            public int encodedCertificateSize;

            public IntPtr certificateInformation;

            public IntPtr certificateStore;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_SIGN_MESSAGE_PARA
        {
            public int cbSize;
            public int dwMsgEncodingType;
            public IntPtr pSigningCert;
            public int dwInnerContentType;
            public int cMsgCert;
            public IntPtr rgpMsgCert;
            public int cMsgCrl;
            public IntPtr rgpMsgCrl;
            public int cAuthAttr;
            public IntPtr rgAuthAttr;
            public int cUnauthAttr;
            public IntPtr rgUnauthAttr;
            public int dwFlags;
            public int dwInnerContentTypeFlag;
            public IntPtr hCryptProv;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_VERIFY_MESSAGE_PARA
        {
            public int cbSize;
            public int dwMsgAndCertEncodingType;
            public IntPtr hCryptProv;
            public int pfnGetSignerCertificate;
            public int dwFlags;
            public int dwInnerContentType;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_DATA_BLOB
        {
            public int cbData;
            public IntPtr pbData;
        }
    }
        
}
