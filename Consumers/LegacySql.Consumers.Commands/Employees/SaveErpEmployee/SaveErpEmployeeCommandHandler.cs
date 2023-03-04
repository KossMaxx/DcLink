using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Employees;
using LegacySql.Domain.Shared;
using MediatR;
using MessageBus.Employees.Import;

namespace LegacySql.Consumers.Commands.Employees.SaveErpEmployee
{
    public class SaveErpEmployeeCommandHandler : IRequestHandler<BaseSaveErpCommand<ErpEmployeeDto>>
    {
        private readonly IDbConnection _db;
        private readonly IEmployeeMapRepository _employeeMapRepository;
        private ExternalMap _employeeMap;

        public SaveErpEmployeeCommandHandler(IDbConnection db, IEmployeeMapRepository employeeMapRepository)
        {
            _db = db;
            _employeeMapRepository = employeeMapRepository;
        }

        public async Task<Unit> Handle(BaseSaveErpCommand<ErpEmployeeDto> command, CancellationToken cancellationToken)
        {
            var employee = command.Value;
            _employeeMap = await _employeeMapRepository.GetByErpAsync(employee.Id);
            if (_employeeMap == null)
            {
                var newEmployeeId = await Create(employee);
                await _employeeMapRepository.SaveAsync(new ExternalMap(command.MessageId, newEmployeeId, employee.Id));
            }
            else
            {
                await Update(employee);
            }

            return new Unit();
        }

        private async Task<int> Create(ErpEmployeeDto employee)
        {
            var insertSqlQuery = @"insert into [dbo].[Сотрудники] 
                                 ([Имя],
                                 [Фамилия],
                                 [уволен],
                                 [ФИОполн],
                                 [Отчество],
                                 [INN],
                                 [Внутренний],
                                 [РабочийТелефон],
                                 [email],
                                 [паспорт],
                                 [выдан],
                                 [серия],
                                 [выданД],
                                 [uuu])
                                 values (@FirstName,
                                 @LastName,
                                 @Fired,
                                 @FullName,
                                 @MiddleName,
                                 @IndividualTaxNumber,
                                 @InternalPhone,
                                 @WorkPhone,
                                 @Email,
                                 @PassportSerialNumber,
                                 @PassportIssuer,
                                 @PassportSeries,
                                 @PassportIssuedAt,
                                 @NickName);
                                 select cast(SCOPE_IDENTITY() as int)";

            return (await _db.QueryAsync<int>(insertSqlQuery, employee)).FirstOrDefault();
        }

        private async Task Update(ErpEmployeeDto employee)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var selectNickNameSqlQuery = @"select [uuu] from [dbo].[Сотрудники]
                                         where [КодСотрудника]=@Id";
                var employeeNickName = (await _db.QueryAsync<string>(selectNickNameSqlQuery, new
                {
                    Id = _employeeMap.LegacyId
                }, transaction)).FirstOrDefault();

                var updateSqlQuery = @"update [dbo].[Сотрудники] 
                                 set [Имя]=@FirstName,
                                 [Фамилия]=@LastName,
                                 [уволен]=@Fired,
                                 [ФИОполн]=@FullName,
                                 [Отчество]=@MiddleName,
                                 [INN]=@IndividualTaxNumber,
                                 [Внутренний]=@InternalPhone,
                                 [РабочийТелефон]=@WorkPhone,
                                 [email]=@Email,
                                 [паспорт]=@PassportSerialNumber,
                                 [выдан]=@PassportIssuer,
                                 [серия]=@PassportSeries,
                                 [выданД]=@PassportIssuedAt,
                                 [uuu]=@NickName
                                 where [КодСотрудника]=@Id";
                await _db.ExecuteAsync(updateSqlQuery, new
                {
                    Id = _employeeMap.LegacyId,
                    employee.FirstName,
                    employee.LastName,
                    employee.Fired,
                    employee.FullName,
                    employee.MiddleName,
                    employee.IndividualTaxNumber,
                    employee.InternalPhone,
                    employee.WorkPhone,
                    employee.Email,
                    employee.PassportSerialNumber,
                    employee.PassportIssuer,
                    employee.PassportSeries,
                    employee.PassportIssuedAt,
                    employee.NickName
                }, transaction);


                if (!string.IsNullOrEmpty(employeeNickName))
                {
                    var updateProductsSqlQuery = @"update [dbo].[Товары]
                                         set [ProductManager]=@NewNickName
                                         where [ProductManager]=@OldNickName";
                    await _db.ExecuteAsync(updateProductsSqlQuery, new
                    {
                        NewNickName = employee.NickName,
                        OldNickName = employeeNickName
                    }, transaction);
                }

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
            finally
            {
                _db.Close();
            }
        }
    }
}
