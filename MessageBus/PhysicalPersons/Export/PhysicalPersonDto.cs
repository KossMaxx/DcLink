using System;

namespace MessageBus.PhysicalPersons.Export
{
    public class PhysicalPersonDto
    {
        public int Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string JobPosition { get; set; }
        public string WorkPhone { get; set; }
        public string PassportSerialNumber { get; set; }
        public string PassportIssuer { get; set; }
        public string PassportSeries { get; set; }
        public DateTime? PassportIssuedAt { get; set; }
        public string Email { get; set; }
        public bool Fired { get; set; }
        public string SqlLogin { get; set; }
        public string FullName { get; set; }
        public string MiddleName { get; set; }
        public string IndividualTaxNumber { get; set; }
        public string InternalPhone { get; set; }
    }
}
