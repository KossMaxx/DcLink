using System;
using MediatR;

namespace LegacySql.Commands.Tests.GenerateTestClientOrders
{
    public class GenerateTestClientOrdersCommand : IRequest
    {
        public GenerateTestClientOrdersCommand(int count, Guid sessionId)
        {
            Count = count;
            SessionId = sessionId;
        }

        public int Count { get; }
        public Guid SessionId { get; }
    }
}
