using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Queries.LoggingSettings.GetLoggingSettings
{
    public class GetLoggingSettingsQueryHandler : IRequestHandler<GetLoggingSettingsQuery, LoggingSettingsDto>
    {
        private readonly LoggingSettingsManager _loggingSettings;
        
        public GetLoggingSettingsQueryHandler(LoggingSettingsManager loggingSettings)
        {
            _loggingSettings = loggingSettings;
        }

        public Task<LoggingSettingsDto> Handle(GetLoggingSettingsQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new LoggingSettingsDto
            {
                HttpBodyLogging = _loggingSettings.HttpBodyLogging,
            });
        }
    }
}
