using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{
    public class LostReportExtFileDto
    : IResponseDto<LostReportExtFile, LostReportExtFileDto>
    {
        public string FilePath { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        // Foreign key
        public int LostReportId { get; set; }


        public static LostReportExtFileDto FromEntity(LostReportExtFile entity)
        {
            return new LostReportExtFileDto
            {
                FilePath = entity.FilePath,
                FileName = entity.FileName,
                Description = entity.Description,
                LostReportId = entity.LostReportId
            };
        }
    }


    public class LostReportDto : IResponseDto<LostReport, LostReportDto> , IHasId
    {
        public int? Id { get; set; }
        public string Coordinates { get; set; } = string.Empty;
        public DateTime dateTime { get; set; }

        public int UserId { get; set; }

        public UserDto? User { get; set; }

        public AnimalDescriptionDto PetDescription { get; set; } = new();

        public List<LostReportExtFileDto> LostReportExtFiles { get; set; } = new();
        public List<FoundReportDto> FoundReports { get; set; } = new();




        public static LostReportDto FromEntity(LostReport entity)
        {
            return new LostReportDto
            {
                Id = entity.Id,
                Coordinates = entity.Coordinates,
                dateTime = entity.dateTime,
                UserId = entity.UserId,

                User = entity.User == null
                    ? null
                    : UserDto.FromEntity(entity.User),

                PetDescription = entity.PetDescription == null
                    ? new AnimalDescriptionDto()
                    : AnimalDescriptionDto.FromEntity(entity.PetDescription),


                LostReportExtFiles = entity.LostReportExtFilesNevigation?
                    .Select(LostReportExtFileDto.FromEntity)
                    .ToList()
                    ?? new(),

                FoundReports = entity.LostFoundMatchNevigation?
                .Where(m => m.FoundReportNevigation != null)
                .Select(m => FoundReportDto.FromEntity(m.FoundReportNevigation))
                .ToList()
                ?? new()
            };
        }
    }

    public class CreateLostReportDto : IEntityDto<LostReport>, IHasId
    {
        public int? Id { get; set; }
        public string Coordinates { get; set; } = string.Empty;
        public DateTime dateTime { get; set; }

        public int UserId { get; set; }

        public AnimalDescriptionDto PetDescription { get; set; } = new();

        public LostReport ToEntity()
        {
            return new LostReport
            {
                Id = Id ?? 0,
                Coordinates = Coordinates,
                dateTime = dateTime,
                UserId = UserId,

                PetDescription = PetDescription.ToEntity()
            };
        }
    }
}
