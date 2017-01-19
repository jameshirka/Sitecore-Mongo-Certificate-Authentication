using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongoCertificateAuth
{
    internal struct Constants
    {
        internal struct Settings
        {
            internal const string VerifySslCertificate = "MongoCertificateAuth.VerifySslCertificate";
            internal const string CertificateName = "MongoCertificateAuth.CertificateName";
            internal const string CertificatePassword = "MongoCertificateAuth.CertificatePassword";
            internal const string CertificateThumbprint = "MongoCertificateAuth.CertificateThumbprint";
            internal const string CheckCertificateRevocation = "MongoCertificateAuth.CheckCertificateRevocation";
        }
    }
}