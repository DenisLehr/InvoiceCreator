namespace Shared.ApiEndpointRoutes
{
    public static class ApiRoutes
    {
        // Firma Routen
        public const string FirmaBase = "firma";
        public const string FirmaById = $"{FirmaBase}/{{id}}";

        // Kunden Routen
        public const string KundenBase = "kunde";
        public const string KundenPaged = $"{KundenBase}/paged";
        public const string KundenById = $"{KundenBase}/{{id}}";
        public const string KundenSearchByName = $"{KundenBase}/search/{{name}}";
        public const string KundenEmail = $"{KundenBase}/sendEmail";

        // Leistungen Routen
        public const string LeistungBase = "leistung";
        public const string LeistungPaged = $"{LeistungBase}/paged";
        public const string LeistungById = $"{LeistungBase}/{{id}}";

        // Rechnungen Routen
        public const string RechnungBase = "rechnung";
        public const string RechnungPaged = $"{RechnungBase}/paged";
        public const string RechnungById = $"{RechnungBase}/{{id}}";
        public const string BuchhhaltungEmail = $"{RechnungBase}/sendEmail";

        // User Routen
        public const string UserBase = "user";
        public const string UserPaged = $"{UserBase}/paged";
        public const string UserById = $"{UserBase}/{{id}}";

        // Termin Routen
        public const string TerminBase = "termin";
        public const string TerminById = $"{TerminBase}/{{id}}";
    }

}
