using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace LegacySql.Commands.Employees
{
    public class MapEmployeeCommand : IRequest
    {
        public MapEmployeeCommand(int innerId, Guid externalId)
        {
            InnerId = innerId;
            ExternalId = externalId;
        }

        public int InnerId { get; }
        public Guid ExternalId { get; }
    }
}
