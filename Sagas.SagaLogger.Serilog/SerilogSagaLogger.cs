using Sagas.Contracts;
using System;
using Serilog;

namespace Sagas.SagaLogger.Serilog
{
    public class SerilogSagaLogger : ISagaLogger
    {
        private readonly ILogger _serilogLogger;

        public SerilogSagaLogger(ILogger serilogLogger)
        {
            _serilogLogger = serilogLogger;
        }

        private void LogInformation(ILogger logger, Guid sagaId, SagaState state)
        {
            logger.Information("{Action} with id {SagaId} was {State}",
                   "Saga",
                   sagaId,
                   state.ToString());
        }

        public void Log(Guid sagaId, SagaState state, Guid erpId, int sqlId, string data = null)
        {
            var logger = _serilogLogger
                .ForContext("ErpId", erpId)
                .ForContext("SqlId", sqlId)
                .ForContext("Data", data);

            LogInformation(logger, sagaId, state);
        }

        public void Log(Guid sagaId, SagaState state, Guid erpId, string data = null)
        {
            var logger = _serilogLogger
                .ForContext("ErpId", erpId)
                .ForContext("Data", data);

            LogInformation(logger, sagaId, state);
        }

        public void Log(Guid sagaId, SagaState state, int sqlId, string data = null)
        {
            var logger = _serilogLogger
                 .ForContext("SqlId", sqlId)
                 .ForContext("Data", data);

            LogInformation(logger, sagaId, state);
        }
    }
}
