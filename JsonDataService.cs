using System.Text.Json;
using CMCS.Models;

namespace CMCS.Services
{
    public class JsonDataService : IDataService
    {
        private readonly string _dataPath;
        private readonly IWebHostEnvironment _environment;

        public JsonDataService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _dataPath = Path.Combine(_environment.ContentRootPath, "Data", "claims.json");
            InitializeData();
        }

        private void InitializeData()
        {
            var directory = Path.GetDirectoryName(_dataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_dataPath))
            {
                var initialData = new ClaimData
                {
                    Claims = new List<Claim>(),
                    Users = new List<User>
                    {
                        new User { UserId = 1, FirstName = "John", LastName = "Smith", Email = "john.smith@university.ac.za", Role = "Lecturer" },
                        new User { UserId = 2, FirstName = "Sarah", LastName = "Johnson", Email = "sarah.j@university.ac.za", Role = "Coordinator" },
                        new User { UserId = 3, FirstName = "Michael", LastName = "Brown", Email = "michael.b@university.ac.za", Role = "Manager" }
                    }
                };
                SaveData(initialData);
            }
        }

        public List<Claim> GetClaims()
        {
            var data = LoadData();
            return data.Claims;
        }

        public Claim GetClaim(int id)
        {
            var claims = GetClaims();
            return claims.FirstOrDefault(c => c.ClaimId == id);
        }

        public void SaveClaim(Claim claim)
        {
            var data = LoadData();
            var existingClaim = data.Claims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);

            if (existingClaim != null)
            {
                data.Claims.Remove(existingClaim);
            }

            data.Claims.Add(claim);
            SaveData(data);
        }

        public void DeleteClaim(int id)
        {
            var data = LoadData();
            var claim = data.Claims.FirstOrDefault(c => c.ClaimId == id);
            if (claim != null)
            {
                data.Claims.Remove(claim);
                SaveData(data);
            }
        }

        public List<User> GetUsers()
        {
            var data = LoadData();
            return data.Users;
        }

        public void SaveDocument(Document document)
        {
            // Documents are stored within the claim in this implementation
            var claim = GetClaim(document.ClaimId);
            if (claim != null)
            {
                if (claim.Documents == null)
                    claim.Documents = new List<Document>();

                claim.Documents.Add(document);
                SaveClaim(claim);
            }
        }

        public List<Document> GetDocumentsByClaimId(int claimId)
        {
            var claim = GetClaim(claimId);
            return claim?.Documents ?? new List<Document>();
        }

        private ClaimData LoadData()
        {
            try
            {
                var json = File.ReadAllText(_dataPath);
                return JsonSerializer.Deserialize<ClaimData>(json) ?? new ClaimData();
            }
            catch
            {
                return new ClaimData();
            }
        }

        private void SaveData(ClaimData data)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(data, options);
            File.WriteAllText(_dataPath, json);
        }
    }

    public class ClaimData
    {
        public List<Claim> Claims { get; set; } = new List<Claim>();
        public List<User> Users { get; set; } = new List<User>();
    }
}