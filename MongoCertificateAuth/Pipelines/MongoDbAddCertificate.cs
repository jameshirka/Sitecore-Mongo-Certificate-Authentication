using Sitecore.Analytics.Pipelines.UpdateMongoDriverSettings;

namespace MongoCertificateAuth.Pipelines
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    using MongoDB.Driver;

    public class MongoDbAddCertificate : UpdateMongoDriverSettingsProcessor
    {
        public override void UpdateSettings(UpdateMongoDriverSettingsArgs args)
        {
            var certificateName = Sitecore.Configuration.Settings.GetSetting(Constants.Settings.CertificateName, "");
            var certificatePassword = Sitecore.Configuration.Settings.GetSetting(Constants.Settings.CertificatePassword, "");
            var verifySslCertificate = Convert.ToBoolean(Sitecore.Configuration.Settings.GetSetting(Constants.Settings.VerifySslCertificate, "false"));

            if (!string.IsNullOrEmpty(certificateName) && !string.IsNullOrEmpty(certificatePassword))
            {
                args.MongoSettings.VerifySslCertificate = verifySslCertificate;

                var cert = new X509Certificate2(certificateName, certificatePassword);
                args.MongoSettings.SslSettings = new SslSettings
                {
                    ClientCertificates = new[] { cert }
                };

                args.MongoSettings.UseSsl = true;
            }
        }
    }

}