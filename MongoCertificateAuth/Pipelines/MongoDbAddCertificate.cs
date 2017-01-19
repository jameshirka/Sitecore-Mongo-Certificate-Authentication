using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using MongoDB.Driver;
using Sitecore.Analytics.Pipelines.UpdateMongoDriverSettings;
using Sitecore.Configuration;

namespace MongoCertificateAuth.Pipelines
{
    public class MongoDbAddCertificate : UpdateMongoDriverSettingsProcessor
    {
        public override void UpdateSettings(UpdateMongoDriverSettingsArgs args)
        {
            var certificate = Getx509Certificate();

            if (certificate != null)
            {
                var verifySslCertificate =
                    Convert.ToBoolean(Settings.GetSetting(Constants.Settings.VerifySslCertificate, "false"));
                var checkCertificateRevocation =
                    Convert.ToBoolean(Settings.GetSetting(Constants.Settings.CheckCertificateRevocation, "false"));

                args.MongoSettings.VerifySslCertificate = verifySslCertificate;

                args.MongoSettings.UseSsl = true;
                args.MongoSettings.SslSettings = new SslSettings
                                                     {
                                                         ClientCertificates = new[] { certificate },
                                                         CheckCertificateRevocation = checkCertificateRevocation
                                                     };
            }
            else
            {
                Sitecore.Diagnostics.Log.Warn("could not load certificate from file.", this);
            }
        }

        private static X509Certificate2 Getx509Certificate()
        {
            X509Certificate2 certificate = null;

            var certificateThumbprint = Settings.GetSetting(Constants.Settings.CertificateThumbprint, "");

            if (!string.IsNullOrEmpty(certificateThumbprint))
            {
                certificate = Getx509CertificateFromCertificateStore(certificateThumbprint);
            }
            else
            {
                var certificateName = Settings.GetSetting(Constants.Settings.CertificateName, "");
                var certificatePassword = Settings.GetSetting(Constants.Settings.CertificatePassword, "");

                if (!string.IsNullOrEmpty(certificateName))
                {
                    certificate = Getx509CertificateFromDisc(certificateName, certificatePassword);
                }

            }

            return certificate;
        }

        public static X509Certificate2 Getx509CertificateFromDisc(string certificateName, string certificatePassword)
        {
            X509Certificate2 certificate = null;

            try
            {
                var binaryData = File.ReadAllBytes(certificateName);
                certificate = new X509Certificate2(binaryData, certificatePassword);
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(
                    $"could not load certificate from file. [{nameof(certificateName)}: {certificateName}]",
                    ex);
            }

            return certificate;
        }

        public static X509Certificate2 Getx509CertificateFromCertificateStore(string certificateThumbprint)
        {
            X509Certificate2 certificate = null;

            try
            {
                var certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                certStore.Open(OpenFlags.ReadOnly);

                certificate = certStore.Certificates
                    .Cast<X509Certificate2>()
                    .FirstOrDefault(cert => cert.Thumbprint != null &&
                    string.Equals(cert.Thumbprint.Trim(), certificateThumbprint.Trim(), StringComparison.InvariantCultureIgnoreCase));
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error(
                    $"could not load certificate from certificate store. [{nameof(certificateThumbprint)}: {certificateThumbprint}]",
                    ex);
            }

            return certificate;
        }
    }

}