using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.Employees
{
    public class Employee : Mapped
    {
        public IdMap Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool Fired { get; }
        public string NickName { get; }
        public string FullName { get; }
        public string MiddleName { get; }
        public string IndividualTaxNumber { get; }
        public string InternalPhone { get; }
        public string WorkPhone { get; }
        public string Email { get; }
        public string PassportSerialNumber { get; }
        public string PassportIssuer { get; }
        public string PassportSeries { get; }
        public DateTime? PassportIssuedAt { get; }

        public Employee(bool hasMap, IdMap id, string firstName, string lastName, bool fired, string nickName, string fullName, string middleName, string individualTaxNumber, string internalPhone, string workPhone, string email, string passportSerialNumber, string passportIssuer, string passportSeries, DateTime? passportIssuedAt) : base(hasMap)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Fired = fired;
            NickName = nickName;
            FullName = fullName;
            MiddleName = middleName;
            IndividualTaxNumber = individualTaxNumber;
            InternalPhone = internalPhone;
            WorkPhone = workPhone;
            Email = email;
            PassportSerialNumber = passportSerialNumber;
            PassportIssuer = passportIssuer;
            PassportSeries = passportSeries;
            PassportIssuedAt = passportIssuedAt;
        }

        public bool IsNew()
        {
            return !HasMap;
        }

        public bool IsChanged()
        {
            return Id?.ExternalId != null;
        }
    }
}
