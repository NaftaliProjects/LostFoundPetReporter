using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{
    public class LostReportExtFileDto
    {
        //properties
        public String FilePath { get; set; }

        public String FileName { get; set; }
        public String Description { get; set; } = "";

        //Foreign keys 
        public int LostReportId { get; set; }
    }

    /// <summary>
    /// Returned to client for GET requests.
    /// Includes primitive/flat data or nested Response DTOs.
    /// </summary>
    public class LostReportDto : IDto
    {
        public int Id { get; set; }
        public string Coordinates { get; set; } = string.Empty;
        public DateTime dateTime { get; set; }

        // Foreign keys / References
        public int UserId { get; set; }

        // If you need nested details on GET, reference DTOs (not DB entities)
        public AnimalDescriptionDto PetDescription { get; set; } = new();
        public List<LostReportExtFileDto> LostReportExtFiles { get; set; } = new();
    }

    /// <summary>
    /// Received from client for POST requests.
    /// Contains ONLY the fields required to create a record.
    /// </summary>
    public class CreateLostReportDto : IDto
    {
        // NOTE: No Id property here (database generates it)

        public string Coordinates { get; set; } = string.Empty;
        public DateTime dateTime { get; set; }

        // Foreign keys required to link relationships
        public int UserId { get; set; }

        // Embedded data required on creation
        public AnimalDescriptionDto PetDescription { get; set; } = new();
    }
}
