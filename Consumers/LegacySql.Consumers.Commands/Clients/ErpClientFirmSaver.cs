using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using LegacySql.Domain.Clients;
using LegacySql.Domain.Firms;
using LegacySql.Domain.Shared;
using MassTransit;
using MessageBus.Clients.Import;
using MessageBus.Firms.Export;
using MessageBus.Firms.Export.Change;

namespace LegacySql.Consumers.Commands.Clients
{
    public class ErpClientFirmSaver
    {
        private readonly IDbConnection _db;
        private readonly IClientMapRepository _clientMapRepository;
        private readonly IFirmMapRepository _firmMapRepository;
        private ExternalMap _clientMapping;
        private ExternalMap _firmMapping;
        private ErpFirmDto _firm;
        private readonly IBus _bus;
        private readonly ILegacyFirmRepository _legacyFirmRepository;

        public ErpClientFirmSaver(
            IDbConnection db, 
            IClientMapRepository clientMapRepository, 
            IFirmMapRepository firmMapRepository, 
            IBus bus, 
            ILegacyFirmRepository legacyFirmRepository)
        {
            _db = db;
            _clientMapRepository = clientMapRepository;
            _firmMapRepository = firmMapRepository;
            _bus = bus;
            _legacyFirmRepository = legacyFirmRepository;
        }

        public void InitErpObject(ErpFirmDto firm, ExternalMap firmMapping)
        {
            _firm = firm;
            _firmMapping = firmMapping;
        }

        public async Task<MappingInfo> GetMappingInfo()
        {
            var why = new StringBuilder();

            if (_firm.ClientId.HasValue)
            {
                _clientMapping = await _clientMapRepository.GetByErpAsync(_firm.ClientId.Value);
                if (_clientMapping == null)
                {
                    why.Append($"Маппинг клиента id:{_firm.ClientId} не найден\n");
                }
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
            if (_firmMapping != null)
            {
                await Update(_firmMapping.LegacyId);
            }
            else
            {
                await Create(messageId);
            }
        }

        private async Task Create(Guid messageId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var insertSqlQuery = @"execute dbo.E21_pkg_modify_Firms NULL, @ClientId, @IsNotResident, @Title, @TaxCode, @LegalAddress, @Phone, @Account, @BankName, @BankCode, @Address, @PayerCode, @CertificateNumber";
                var newFirmId = await _db.QueryFirstOrDefaultAsync<int>(insertSqlQuery, new
                {
                    ClientId = _clientMapping?.LegacyId,
                    IsNotResident = _firm.IsNotResident,
                    Title = _firm.Title,
                    TaxCode = _firm.TaxCode,
                    LegalAddress = _firm.LegalAddress,
                    Phone = _firm.Phone,
                    Account = _firm.Account,
                    BankName = _firm.BankName,
                    BankCode = _firm.BankCode,
                    Address = string.IsNullOrEmpty(_firm.Address) ? "" : _firm.Address,
                    PayerCode = _firm?.PayerCode,
                    CertificateNumber = _firm.CertificateNumber
                }, transaction);
                transaction.Commit();

                await _firmMapRepository.SaveAsync(new ExternalMap(messageId, newFirmId, externalMapId: _firm.Id));
                await PublishNewProduct(newFirmId);
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

        private async Task Update(int firmId)
        {
            _db.Open();
            using var transaction = _db.BeginTransaction();
            try
            {
                var updateSqlQuery = @"execute dbo.E21_pkg_modify_Firms @Id, @ClientId, @IsNotResident, @Title, @TaxCode, @LegalAddress, @Phone, @Account, @BankName, @BankCode, @Address, @PayerCode, @CertificateNumber";
                await _db.ExecuteAsync(updateSqlQuery, new
                {
                    Id = firmId,
                    ClientId = _clientMapping?.LegacyId,
                    IsNotResident = _firm.IsNotResident,
                    Title = _firm.Title,
                    TaxCode = _firm.TaxCode,
                    LegalAddress = _firm.LegalAddress,
                    Phone = _firm.Phone,
                    Account = _firm.Account,
                    BankName = _firm.BankName,
                    BankCode = _firm.BankCode,
                    Address = string.IsNullOrEmpty(_firm.Address) ? "" : _firm.Address,
                    PayerCode = _firm?.PayerCode,
                    CertificateNumber = _firm.CertificateNumber
                }, transaction);

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

        private async Task PublishNewProduct(int newFirmId)
        {
            var firm = await _legacyFirmRepository.GetFirmAsync(newFirmId, CancellationToken.None);
            if (firm == null)
            {
                throw new KeyNotFoundException("Фирма не найдена");
            }

            var firmDto = MapToDto(firm);
            await _bus.Publish(new ChangeLegacyFirmMessage
            {
                SagaId = Guid.NewGuid(),
                MessageId = Guid.NewGuid(),
                Value = firmDto,
                ErpId = _firm.Id
            });
        }

        private FirmDto MapToDto(Firm firm)
        {
            return new FirmDto
            {
                Code = firm.Id.InnerId,
                Title = firm.Title,
                TaxCode = firm.TaxCode,
                NotVat = firm.NotVat ?? false,
                ClientId = firm.ClientId?.ExternalId,
                MasterClientId = firm.MasterClientId?.ExternalId,
                Phone = firm.Phone,
                Address = firm.Address,
                LegalAddress = firm.LegalAddress,
                PayerCode = firm.PayerCode,
                CertificateNumber = firm.CertificateNumber,
                Account = firm.Account,
                BankCode = firm.BankCode,
                BankName = firm.BankName,
                IsNotResident = firm.IsNotResident
            };
        }
    }
}
