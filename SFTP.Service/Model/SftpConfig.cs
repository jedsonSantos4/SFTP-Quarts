namespace SFTP.Service.Model
{
    public class SftpConfig
    {        

        public SftpConfig(string host, int port, string name, string pass)
        {
            this.Host = host;
            this.Port = port;
            this.UserName = name;
            this.Password = pass;
        }

        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
    }
}
