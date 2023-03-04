using MediatR;

namespace LegacySql.Queries.LoggingSettings.GetLoggingSettings
{
    public class GetLoggingSettingsQuery : IRequest<LoggingSettingsDto> { }
}