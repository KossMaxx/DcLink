using LegacySql.Domain.Shared;
using MediatR;

namespace LegacySql.Commands.LoggingSettings.SetHttpBodyLogging
{
    class SetHttpBodyLoggingCommandHandler : RequestHandler<SetHttpBodyLoggingCommand>
    {
        private readonly LoggingSettingsManager _loggingSettings;
        
        public SetHttpBodyLoggingCommandHandler(LoggingSettingsManager loggingSettings)
        {
            _loggingSettings = loggingSettings;
        }

        protected override void Handle(SetHttpBodyLoggingCommand command)
        {
            _loggingSettings.HttpBodyLogging = command.HttpBodyLogging;
        }
    }
}