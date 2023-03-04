using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Departments;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Shared;
using MessageBus.Departments.Import;

namespace LegacySql.Consumers.Commands.Departments
{
    public class ErpDepartmentSaver
    {
        private readonly IDbConnection _db;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private readonly IDepartmentMapRepository _departmentMapRepository;
        private ErpDepartmentDto _department;
        private ExternalMap _departmentMapping;
        private ExternalMap _bossMapping;

        public ErpDepartmentSaver(IDbConnection db, 
            IEmployeeMapRepository employeeMapRepository, 
            IDepartmentMapRepository departmentMapRepository)
        {
            _db = db;
            _employeeMapRepository = employeeMapRepository;
            _departmentMapRepository = departmentMapRepository;
        }

        public void InitErpObject(ErpDepartmentDto department, ExternalMap departmentMapping)
        {
            _department = department;
            _departmentMapping = departmentMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            _bossMapping = await _employeeMapRepository.GetByErpAsync(_department.BossId);
            if (_bossMapping == null)
            {
                why.Append($"Маппинг сотрудника (boss) id:{_department.BossId} не найден\n");
            }

            var whyString = why.ToString();
            return new MappingInfo
            {
                IsMappingFull = string.IsNullOrEmpty(whyString),
                Why = whyString,
            };
        }

        public async Task Save(Guid messageId)
        {
            if (_departmentMapping == null)
            {
                await Create(messageId);
            }
            else
            {
                await Update();
            }
        }

        private async Task Create(Guid messageId)
        {
            var insertSqlQuery = @"insert into [dbo].[departments]
                                 ([nazv],
                                 [descr],
                                 [boss],
                                 [BossId])
                                 values (@Title,
                                 @Description,
                                 (select [Должность] from [dbo].[Сотрудники] where [КодСотрудника]=@BossId),
                                 @BossId);
                                 select cast(SCOPE_IDENTITY() as int)";
            var newDepartmentId = await _db.QueryFirstOrDefaultAsync<int>(insertSqlQuery, new
            {
                Title = _department.Title,
                Description = _department.Description,
                BossId = _bossMapping.LegacyId
            });

            await _departmentMapRepository.SaveAsync(new ExternalMap(messageId, newDepartmentId, _department.Id));
        }

        private async Task Update()
        {
            var updateSqlQuery = @"update [dbo].[departments]
                                 set 
                                 [nazv]=@Title,
                                 [descr]=@Description,
                                 [boss]=(select [Должность] from [dbo].[Сотрудники] where [КодСотрудника]=@BossId),
                                 [BossId]=@BossId
                                 where [id]=@Id";
            await _db.ExecuteAsync(updateSqlQuery, new
            {
                Id = _departmentMapping.LegacyId,
                Title = _department.Title,
                Description = _department.Description,
                BossId = _bossMapping.LegacyId
            });
        }
    }
}
