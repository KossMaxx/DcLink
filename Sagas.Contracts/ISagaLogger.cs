using System;
using System.Collections.Generic;
using System.Text;

namespace Sagas.Contracts
{
    public interface ISagaLogger
    {
        void Log(Guid sagaId, SagaState state, Guid erpId, int sqlId, string data = null);
        void Log(Guid sagaId, SagaState state, Guid erpId, string data = null);
        void Log(Guid sagaId, SagaState state, int sqlId, string data = null);
    }
}
