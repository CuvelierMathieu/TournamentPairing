using System.Net;
using TournamentPairing.Core;

namespace TournamentPairing
{
    public class FtpUploader
    {
        public void Upload(UploadParameter uploadParameter)
        {
            using WebClient client = new();
            client.Credentials = new NetworkCredential(uploadParameter.FtpConnectionParameter.Username, uploadParameter.FtpConnectionParameter.Password);

            string remoteFullPath = uploadParameter.FtpConnectionParameter.Address.EndsWith("/")
                ? $"{uploadParameter.FtpConnectionParameter.Address}{uploadParameter.RemoteFilePath}"
                : $"{uploadParameter.FtpConnectionParameter.Address}/{uploadParameter.RemoteFilePath}";

            if (!remoteFullPath.StartsWith("ftp://"))
                remoteFullPath = $"ftp://{remoteFullPath}";

            client.UploadFile(remoteFullPath, WebRequestMethods.Ftp.UploadFile, uploadParameter.LocalFilePath);
        }
    }

    public struct UploadParameter
    {
        public FtpConnectionParameter FtpConnectionParameter { get; set; }
        public string LocalFilePath { get; set; }
        public string RemoteFilePath { get; set; }
    }
}
