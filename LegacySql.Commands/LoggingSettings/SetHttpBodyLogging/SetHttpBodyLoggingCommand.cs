using MediatR;

namespace LegacySql.Commands.LoggingSettings.SetHttpBodyLogging
{
    public class SetHttpBodyLoggingCommand : IRequest
    {
        public bool HttpBodyLogging { get; set; }
    }
}