using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Data;
using LegacySql.Data.Models;
using LegacySql.Domain.Classes;
using LegacySql.Domain.ProductSubtypes;
using LegacySql.Domain.ProductTypes;
using LegacySql.Domain.Shared;
using LegacySql.Legacy.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LegacySql.Legacy.Data.Repositories
{
    public class ClassRepository : ILegacyClassRepository
    {
        private readonly LegacyDbConnection _db;
        private readonly AppDbContext _mapDb;

        public ClassRepository(LegacyDbConnection db, AppDbContext mapDb)
        {
            _db = db;
            _mapDb = mapDb;
        }

        private async Task<IEnumerable<ProductClassEF>> GetAllAsync()
        {
            const string selectSqlRequest = @"select ROW_NUMBER() OVER (Order by Title) AS Id, Title FROM (
                                    select distinct nazv AS Title from klas
                                    where not nazv is null and nazv != ''
                                    union
                                    select distinct [klas] AS Title from [dbo].[Товары]
                                    where not [klas] is null and [klas] != ''
                                    union
                                    select distinct [klas] AS Title from [dbo].[PriceAlgoritmDetails]
                                    where not [klas] is null and [klas] != '' and [klas] != 'все') PC";
            var classList = await _db.Connection.QueryAsync<ProductClassEF>(selectSqlRequest);
            return classList;
        }

        private async Task<IEnumerable<int>> GetProductTypesAsync(string title)
        {
            const string selectProcedure = "E21_pkg_get_category_by_class";
            var selectParameters = new { class_name = title };
            return (await _db.Connection.QueryAsync<int>(selectProcedure, selectParameters,
                commandType: CommandType.StoredProcedure)).ToList();
        }

        public async IAsyncEnumerable<ProductClass> GetAllAsync(Func<ProductClassEF, bool> filter, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var query = await GetAllAsync();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var productClassEf = query
#if DEBUG
                .Take(1000)
#endif
                .ToList();

            foreach (var p in productClassEf)
            {
                yield return await MapToDomain(p, cancellationToken);
            }
        }

        public IAsyncEnumerable<ProductClass> GetChangedClassAsync(CancellationToken cancellationToken)
        {
            return GetAllAsync(null, cancellationToken);
        }

        public async Task<ProductClass> GetAsync(string title, CancellationToken cancellationToken)
        {
            var query = await GetAllAsync();
            var productClassEf = query.FirstOrDefault(p => p.Title == title);

            if (productClassEf == null)
            {
                return null;
            }

            return await MapToDomain(productClassEf, cancellationToken);
        }

        private async Task<ProductClass> MapToDomain(ProductClassEF productClassEf, CancellationToken cancellationToken)
        {
            var classMap = await _mapDb.ClassMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyTitle == productClassEf.Title, cancellationToken: cancellationToken);
            var productTypeMaps = new List<IdMap>();
            var productTypes = await GetProductTypesAsync(productClassEf.Title);
            foreach (var productType in productTypes)
            {
                var productTypeMap = await _mapDb.ProductTypeMaps.AsNoTracking().FirstOrDefaultAsync(m => m.LegacyId == productType, cancellationToken: cancellationToken);
                productTypeMaps.Add(new IdMap(productType, productTypeMap?.ErpGuid));
            }

            return new ProductClass(
                 id: new IdMap(productClassEf.Id, classMap?.ErpGuid),
                 title: productClassEf.Title,
                 productTypes: productTypeMaps,
                 hasMap: classMap != null
             );
        }
    }
}