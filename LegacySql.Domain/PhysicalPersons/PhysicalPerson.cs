using System;
using LegacySql.Domain.Shared;

namespace LegacySql.Domain.PhysicalPersons
{
    public class PhysicalPerson : Mapped
    {
        public IdMap Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string JobPosition { get; }
        public string WorkPhone { get; }
        public string PassportSerialNumber { get; }
        public string PassportIssuer { get; }
        public string PassportSeries { get; }
        public DateTime? PassportIssuedAt { get; }
        public string Email { get; }
        public bool Fired { get; }
        public string NickName { get; }
        public string FullName { get; }
        public string MiddleName { get; }
        public string IndividualTaxNumber { get; }
        public string InternalPhone { get; }

        public PhysicalPerson(bool hasMap, IdMap id, string firstName, string lastName, string jobPosition, string workPhone, string passportSerialNumber, string passportIssuer, string passportSeries, DateTime? passportIssuedAt, string email, bool fired, string nickName, string fullName, string middleName, string individualTaxNumber, string internalPhone) : base(hasMap)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            JobPosition = jobPosition;
            WorkPhone = workPhone;
            PassportSerialNumber = passportSerialNumber;
            PassportIssuer = passportIssuer;
            PassportSeries = passportSeries;
            PassportIssuedAt = passportIssuedAt;
            Email = email;
            Fired = fired;
            NickName = nickName;
            FullName = fullName;
            MiddleName = middleName;
            IndividualTaxNumber = individualTaxNumber;
            InternalPhone = internalPhone;
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
