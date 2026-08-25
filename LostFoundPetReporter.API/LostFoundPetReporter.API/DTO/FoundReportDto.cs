using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{



    public class FoundCoordinateDto : IResponseDto<FoundCoordinate, FoundCoordinateDto>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static FoundCoordinateDto FromEntity(FoundCoordinate entity)
        {
            return new FoundCoordinateDto
            {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
            };
        }
    }

    

    public class FoundReportExtFileDto
        : IResponseDto<FoundReportExtFile, FoundReportExtFileDto> , IHasId
    {
        public int? Id { get; set; }
        //properties
        public String FilePath { get; set; }

        public String FileName { get; set; }
        public String Description { get; set; } = "";

        //Foreign keys 
        public int FoundReportId { get; set; }


        public static FoundReportExtFileDto FromEntity(FoundReportExtFile entity)
        {
            return new FoundReportExtFileDto
            {
                FilePath = entity.FilePath,
                FileName = entity.FileName,
                Description = entity.Description,
                FoundReportId = entity.FoundReportId
            };
        }
    }

    public class FoundReportDto : IResponseDto<FoundReport, FoundReportDto> , IHasId
    {
        public int? Id { get; set; }
        public DateTime dateTime { get; set; }
        public int UserId { get; set; }

        
        public UserDto? User { get; set; }
        public FoundCoordinateDto? FoundCoordinate { get; set; }
        public AnimalDescriptionDto PetDescription { get; set; } = new();
        public List<FoundReportExtFileDto> FoundReportExtFiles { get; set; } = new();


        public static FoundReportDto FromEntity(FoundReport entity)
        {
            return new FoundReportDto
            {
                Id = entity.Id,
                dateTime = entity.dateTime,
                UserId = entity.UserId,

                User = entity.UserNevigation == null
                    ? null
                    : UserDto.FromEntity(entity.UserNevigation),

                PetDescription = entity.PetDescription == null
                    ? new AnimalDescriptionDto()
                    : AnimalDescriptionDto.FromEntity(entity.PetDescription),

                FoundCoordinate = entity.FoundCoordinateNavigation == null 
                    ? null
                    : FoundCoordinateDto.FromEntity(entity.FoundCoordinateNavigation),

                FoundReportExtFiles = entity.FoundReportExtFilesNevigation?
                    .Select(FoundReportExtFileDto.FromEntity)
                    .ToList()
                    ?? new()
            };
        }

    }


    public class CreateFoundReportDto : IEntityDto<FoundReport> , IHasId
    {
        public int? Id { get; set; }
        public DateTime dateTime { get; set; }

        // Foreign keys required to link relationships
        public int UserId { get; set; }

        // Embedded data required on creation
        public AnimalDescriptionDto PetDescription { get; set; } = new();
        public FoundCoordinateDto? FoundCoordinate { get; set; } = new();


        public FoundReport ToEntity()
        {
            return new FoundReport
            {
                Id = Id ?? 0,
                dateTime = dateTime,
                UserId = UserId,
                PetDescription = PetDescription.ToEntity(),
                FoundCoordinateNavigation = new FoundCoordinate
                {
                    Latitude = FoundCoordinate.Latitude,
                    Longitude = FoundCoordinate.Longitude
                }
            };


        }
    }
}