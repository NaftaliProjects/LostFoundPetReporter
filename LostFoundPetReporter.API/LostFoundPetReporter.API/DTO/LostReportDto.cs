using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{

    public class LostCoordinateDto : IResponseDto<LostCoordinate, LostCoordinateDto>
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public static LostCoordinateDto FromEntity(LostCoordinate entity)
        {
            return new LostCoordinateDto
            {
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
            };
        }
    }


    public class LostReportExtFileDto
     : IResponseDto<LostReportExtFile, LostReportExtFileDto>
    {
        public string PictureBase64 { get; set; } = "";

        public static LostReportExtFileDto FromEntity(
            LostReportExtFile entity)
        {
            return new LostReportExtFileDto
            {

            };
        }
    }


    public class LostReportDto : IResponseDto<LostReport, LostReportDto> , IHasId
    {
        public int? Id { get; set; }
        public DateTime dateTime { get; set; }

        public int UserId { get; set; }

        public UserDto? User { get; set; }

        public LostCoordinateDto? LostCoordinate { get; set; }
        public AnimalDescriptionDto PetDescription { get; set; } = new();

        public List<string>? PictureBase64List { get; set; }
        public List<FoundReportDto> FoundReports { get; set; } = new();




        public static LostReportDto FromEntity(LostReport entity)
        {
            return new LostReportDto
            {
                Id = entity.Id,
                dateTime = entity.dateTime,
                UserId = entity.UserId,

                User = entity.User == null
                    ? null
                    : UserDto.FromEntity(entity.User),

                PetDescription = entity.PetDescription == null
                    ? new AnimalDescriptionDto()
                    : AnimalDescriptionDto.FromEntity(entity.PetDescription),

                LostCoordinate = entity.LostCoordinateNavigation == null
                    ? null
                    : LostCoordinateDto.FromEntity(entity.LostCoordinateNavigation),



                FoundReports = entity.LostFoundMatchNevigation?
                .Where(m => m.FoundReportNevigation != null)
                .Select(m => FoundReportDto.FromEntity(m.FoundReportNevigation))
                .ToList()
                ?? new(),


                PictureBase64List = entity.LostReportExtFilesNevigation?
                .Where(x => File.Exists(x.FilePath))
                .Select(x => Convert.ToBase64String(
                    File.ReadAllBytes(x.FilePath)))
                .ToList()
            };
        }
    }

    public class CreateLostReportDto : IEntityDto<LostReport>, IHasId
    {
        public int? Id { get; set; }
        public DateTime dateTime { get; set; }

        public int UserId { get; set; }

        public CreateAnimalDescriptionDto PetDescription { get; set; } = new();
        public LostCoordinateDto? LostCoordinate { get; set; } = new();

        public List<string>? PictureBase64List { get; set; }

        public LostReport ToEntity()
        {
            return new LostReport
            {
                Id = Id ?? 0,
                dateTime = dateTime,
                UserId = UserId,
                PetDescription = PetDescription.ToEntity(),
                LostCoordinateNavigation = new LostCoordinate
                {
                    Latitude = LostCoordinate.Latitude,
                    Longitude = LostCoordinate.Longitude
                },

                LostReportExtFilesNevigation = new List<LostReportExtFile>()

            };


        }
    }
}
