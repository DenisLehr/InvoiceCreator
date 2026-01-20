using Shared.Domain.Models;
using Shared.Domain.ValueObjects;

namespace Data.Interfaces
{
    public interface IFirmaRepository: IBaseRepository<Firma>
    {
        Task <Firma>GetFirmendatenAsync();
        Task<Adresse> GetAdresseVonFirmaAsync();
        Task UpdateAdresseVonFirmaAsync(Adresse adresse);

        Task<Bankverbindung> GetBankverbindungVonFirmaAsync();
        Task UpdateBankverbindungVonFirmaAsync(Bankverbindung bankverbindung);
    }
}
