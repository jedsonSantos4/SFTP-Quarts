using System.Net.Http.Json;
using Polly.CircuitBreaker;
using SFTP.Service.Implementation;
using SFTP.Service.Interface;

namespace worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISftpService _SftpService;
        private readonly AsyncCircuitBreakerPolicy _circuitBreaker;

        public Worker(ILogger<Worker> logger,
            IConfiguration configuration,
            AsyncCircuitBreakerPolicy circuitBreaker, ISftpService sftpService)
        {
            _logger = logger;
            _configuration = configuration;
            _circuitBreaker = circuitBreaker;
            _SftpService = sftpService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var httpClient = new HttpClient();
            var urlApiContagem = _configuration["UrlApiContagem"];

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                   await _circuitBreaker.ExecuteAsync(() =>
                    {
                       return (Task)_SftpService.ListAllFiles();
                    });

                    _logger.LogInformation($"* {DateTime.Now:HH:mm:ss} * " +
                        $"Circuito = {_circuitBreaker.CircuitState} | " );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"# {DateTime.Now:HH:mm:ss} # " +
                        $"Circuito = {_circuitBreaker.CircuitState} | " +
                        $"Falha ao invocar a SFTP: {ex.GetType().FullName} | {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

}
