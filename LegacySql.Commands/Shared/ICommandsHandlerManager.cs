using System;
using System.Threading.Tasks;

namespace LegacySql.Commands.Shared
{
    public interface ICommandsHandlerManager
    {
        Task<bool> IsExecuted(Type handlerType);
        Task Start(Type handlerType);
        Task Finish(Type handlerType);
    }
}
