using CMCS.Models;

namespace CMCS.Services
{
    public interface IDataService
    {
        List<Claim> GetClaims();
        Claim GetClaim(int id);
        void SaveClaim(Claim claim);
        void DeleteClaim(int id);
        List<User> GetUsers();
        void SaveDocument(Document document);
        List<Document> GetDocumentsByClaimId(int claimId);
    }
}