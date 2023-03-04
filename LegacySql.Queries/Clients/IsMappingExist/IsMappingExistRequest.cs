using System;
using MediatR;

namespace LegacySql.Queries.Clients.IsMappingExist
{
    public class IsMappingExistRequest : IRequest<bool>
    {
        public IsMappingExistRequest(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; }
    }
}
