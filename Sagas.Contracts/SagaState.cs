using System;
using System.Collections.Generic;
using System.Text;

namespace Sagas.Contracts
{
    public enum SagaState
    {
        Published,
        Recieved,
        Confirmed,
        Obsoleted
    }
}
