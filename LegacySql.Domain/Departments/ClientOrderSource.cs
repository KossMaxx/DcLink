namespace LegacySql.Domain.Departments
{
    public class ClientOrderSource
    {
        private const string OptDclinkOrgUa = "opt.dclink.org.ua";
        
        public ClientOrderSource(string title)
        {
            Title = title;
        }

        public string Title { get; }

        public ClientOrderSourceType Type
        {
            get
            {
                if (Title == OptDclinkOrgUa)
                {
                    return ClientOrderSourceType.B2B;
                }

                return Title.Substring(0, 2) == "МП" ? ClientOrderSourceType.Marketplace : ClientOrderSourceType.Other;
            }
        }

        public static ClientOrderSource CreateOptDclinkSource()
        {
            return new ClientOrderSource(OptDclinkOrgUa);
        }
    }
}