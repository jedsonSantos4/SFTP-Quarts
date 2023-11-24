using Renci.SshNet.Sftp;

namespace SFTP.Service.Interface
{
    public interface ISftpService
    {
        IEnumerable<ISftpFile> ListAllFiles(string remoteDirectory = ".");
        void UploadFile(string localFilePath, string remoteFilePath);
        void DownloadFile(string remoteFilePath, string localFilePath);
        void DeleteFile(string remoteFilePath);
    }
}
