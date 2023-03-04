using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LegacySql.Queries.Diagnostics.HealthCheck
{
    public class HealthCheckQueryHandler : IRequestHandler<HealthCheckQuery>
    {
        private readonly string _rabbitLogin;
        private readonly string _rabbitPassword;
        private readonly string _rabbitHost;
        private readonly List<string> _connectionNames= new List<string> {"LegacySql.Api", "LegacySql.Consumers.ConsoleApp"};
        private ILogger<HealthCheckQueryHandler> _logger;

        public HealthCheckQueryHandler(ILogger<HealthCheckQueryHandler> logger)
        {
            _rabbitLogin = Environment.GetEnvironmentVariable("RabbitMq__Username");
            _rabbitPassword = Environment.GetEnvironmentVariable("RabbitMq__Password");
            _rabbitHost = GetRabbitHost();
            _logger = logger;
        }

        private string GetRabbitHost()
        {
            var host = Environment.GetEnvironmentVariable("RabbitMq__HostAdress");
            if (string.IsNullOrEmpty(host))
            {
                return string.Empty;
            }
            var splitHost = host.Split(new[] {":"}, StringSplitOptions.RemoveEmptyEntries);
            return $"http:{splitHost[1]}:15672/api/connections";
        }

        public async Task<Unit> Handle(HealthCheckQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var networkCredential = new NetworkCredential(_rabbitLogin, _rabbitPassword);
                var httpClientHandler = new HttpClientHandler { Credentials = networkCredential };
                using var httpClient = new HttpClient(httpClientHandler);
                var httpResponseMessage = await httpClient.GetAsync(_rabbitHost, cancellationToken);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Ошибка запроса. Код ответа {httpResponseMessage.StatusCode}");
                }

                var httpContent = httpResponseMessage.Content;
                using var streamReader = new StreamReader(await httpContent.ReadAsStreamAsync());
                string returnedJsonString = await streamReader.ReadToEndAsync();
                var connections = JsonConvert.DeserializeObject<List<RabbitConnection>>(returnedJsonString)
                    .Where(e=>_connectionNames.Contains(e.user_provided_name))
                    .Select(e=>e.user_provided_name).ToList();

                if (connections.Count() != 2)
                {
                    var message = new StringBuilder("Недостаточно подключений к Rabbit. Подключенные сервисы:");
                    foreach (var connection in connections)
                    {
                        message.Append($"{connection},");

                    }
                    throw new Exception(message.ToString());
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e.Message);
                throw e;
            }

            return new Unit();
        }
    }

    public class RabbitConnection
    {
        public string auth_mechanism { get; set; }
        public long channel_max { get; set; }
        public long channels { get; set; }
        public long connected_at { get; set; }
        public long frame_max { get; set; }
        public string host { get; set; }
        public string name { get; set; }
        public string node { get; set; }
        public string peer_cert_issuer { get; set; }
        public string peer_cert_subject { get; set; }
        public string peer_cert_validity { get; set; }
        public string peer_host { get; set; }
        public long peer_port { get; set; }
        public long port { get; set; }
        public string protocol { get; set; }
        public long recv_cnt { get; set; }
        public long recv_oct { get; set; }
        public long reductions { get; set; }
        public long send_cnt { get; set; }
        public long send_oct { get; set; }
        public long send_pend { get; set; }
        public string ssl { get; set; }
        public string ssl_cipher { get; set; }
        public string ssl_hash { get; set; }
        public string ssl_key_exchange { get; set; }
        public string ssl_protocol { get; set; }
        public string state { get; set; }
        public long timeout { get; set; }
        public string type { get; set; }
        public string user { get; set; }
        public string user_provided_name { get; set; }
        public string user_who_performed_action { get; set; }
        public string vhost { get; set; }
    }
}
