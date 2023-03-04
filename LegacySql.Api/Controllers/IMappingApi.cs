using LegacySql.Api.Models;
using LegacySql.Queries.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LegacySql.Api.Controllers
{
    public interface IMappingApi
    {      
        public Task<ActionResult<IEnumerable<MappingDto>>> GetMappings();
        public Task<ActionResult<IEnumerable<LegacyReferenceDto>>> GetReferences(string search);
        public Task<ActionResult> Map(EntityMapping mapping);
        public Task<ActionResult> Unmap(Guid erpId);
    }
}
