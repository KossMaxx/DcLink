using System;
using System.Threading;
using System.Threading.Tasks;
using LegacySql.Domain.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LegacySql.Commands.Shared
{
    public abstract class ManagedCommandHandler<TRequest> : IRequestHandler<TRequest> where TRequest : IRequest
    {
        private readonly ICommandsHandlerManager _manager;
        protected readonly ILogger _logger;

        protected ManagedCommandHandler(ILogger logger, ICommandsHandlerManager manager)
        {
            _manager = manager;
            _logger = logger;
        }
        public async Task<Unit> Handle(TRequest command, CancellationToken cancellationToken)
        {
            var handlerType = GetType();
            var entityName = handlerType.GetEntityName();
            if (await _manager.IsExecuted(handlerType))
            {
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {handlerType.Name} Ignored. Is executing");

                return await Unit.Task;
            }

            await _manager.Start(handlerType);
            _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {handlerType.Name} Started.");

            try
            {
                await HandleCommand(command, cancellationToken);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                await _manager.Finish(handlerType);
                _logger.LogInformation($"LegacySql | {entityName} | CommandHandler: {handlerType.Name} Finished.");
            }

            return await Unit.Task;
        }

        public abstract Task HandleCommand(TRequest command, CancellationToken cancellationToken);
    }
}
