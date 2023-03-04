using MediatR;
using System;

namespace LegacySql.Queries.SerialNumbers
{
    public class GetSerialNumberQuery : IRequest<SerialNumberDto>
    {
        public string SerialNumber { get; private set; }
        public Guid? ClientId { get; private set; }

        public GetSerialNumberQuery(string serialNumber, Guid? clientId)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                throw new ArgumentException(nameof(serialNumber));
            }
            SerialNumber = serialNumber;
            ClientId = clientId;
        }
    }
}
