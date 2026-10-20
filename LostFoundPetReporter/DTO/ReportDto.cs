using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.API.Services.BackgroundServices;

namespace LostFoundPetReporter.API.DTO
{
   

        public class CoordinateDto : IResponseDto<Coordinate, CoordinateDto>
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }

            public static CoordinateDto FromEntity(Coordinate entity)
            {
                return new CoordinateDto
                {
                    Latitude = entity.Latitude,
                    Longitude = entity.Longitude,
                };
            }
        }


        public class ReportDto : IResponseDto<Report, ReportDto>, IHasId
        {

            private IFileStorageService _fileStorageService;
            public int? Id { get; set; }
            public DateTime DateReported { get; set; }
            public int UserId { get; set; }

           public string ReportType { get; set; } = "";

        
            public UserDto? User { get; set; }
            public CoordinateDto? Coordinate { get; set; }
            public AnimalDescriptionDto PetDescription { get; set; } = new();

            public List<MatchedReportDto> MatchedReports { get; set; } = new();
            public List<string>? PictureBase64List { get; set; }


            public static ReportDto FromEntity(Report entity)
            {
                return new ReportDto
                {
                    Id = entity.Id,
                    DateReported = entity.DateReported,
                    ReportType = entity.ReportType,

                    MatchedReports = entity.ReportType == "l"
                        ? entity.LostMatches
                            .Select(match => MatchedReportDto.FromEntity(
                                match.FoundReportNevigation, match))
                            .ToList()
                        : entity.ReportType == "f"
                            ? entity.FoundMatches
                                .Select(match => MatchedReportDto.FromEntity(
                                    match.LostReportNevigation, match))
                                .ToList()
                            : new List<MatchedReportDto>(),

                    UserId = entity.UserId,

                    User = entity.UserNevigation == null
                        ? null
                        : UserDto.FromEntity(entity.UserNevigation),

                    PetDescription = entity.PetDescription == null
                        ? new AnimalDescriptionDto()
                        : AnimalDescriptionDto.FromEntity(entity.PetDescription),

                    Coordinate = entity.ReportCoordinateNavigation == null
                        ? null
                        : CoordinateDto.FromEntity(entity.ReportCoordinateNavigation),

                    PictureBase64List = entity.ReportExtFilesNevigation?
                    .Where(x => File.Exists(x.FilePath))
                    .Select(x => Convert.ToBase64String(
                        File.ReadAllBytes(x.FilePath)))
                    .ToList()


                };
            }

        }


        public class CreateReportDto : IEntityDto<Report>, IHasId
        {
            public int? Id { get; set; }
            

            public DateTime dateTime { get; set; }

            public int UserId { get; set; }

            public CreateAnimalDescriptionDto PetDescription { get; set; } = new();
            public CoordinateDto? Coordinate { get; set; } = new();

            public List<string>? PictureBase64List { get; set; }

            public virtual  Report ToEntity()
            {
                return new Report
                {
                    Id = Id ?? 0,
                    ReportType = "",
                    DateReported = dateTime,
                    UserId = UserId,
                    PetDescription = PetDescription.ToEntity(),
                    ReportCoordinateNavigation = new Coordinate
                    {
                        Latitude = Coordinate.Latitude,
                        Longitude = Coordinate.Longitude
                    },

                    ReportExtFilesNevigation = new List<ExtFile>(),

                };

            }
        }


        public class CreateFoundReportDto : CreateReportDto
         {


            public override Report ToEntity()
            {
                Report report = base.ToEntity();
                report.ReportType = "f";
                return report;
            }

         }

        public class CreateLostReportDto : CreateReportDto
         {
            public override Report ToEntity()
            {
                Report report = base.ToEntity();


                report.ReportType = "l";
                return report;
            }

         }




    public class MatchedReportDto
    {
        public int Id { get; set; }
        public string ReportType { get; set; } = "";
        public DateTime DateReported { get; set; }

        public CoordinateDto? Coordinate { get; set; }

        public AnimalDescriptionDto PetDescription { get; set; } = new();

        public List<string> PictureBase64List { get; set; } = new();

        public double Score { get; set; }

        public string MatchReason { get; set; } = "";



        public static MatchedReportDto FromEntity(
            Report report,
            LostFoundMatch match)
        {
            return new MatchedReportDto
            {
                Id = report.Id,
                ReportType = report.ReportType,
                DateReported = report.DateReported,

                Coordinate = report.ReportCoordinateNavigation == null
                    ? null
                    : CoordinateDto.FromEntity(
                        report.ReportCoordinateNavigation),

                PetDescription = report.PetDescription == null
                    ? new AnimalDescriptionDto()
                    : AnimalDescriptionDto.FromEntity(
                        report.PetDescription),

                PictureBase64List = report.ReportExtFilesNevigation?
                    .Where(file => File.Exists(file.FilePath))
                    .Select(file => Convert.ToBase64String(
                        File.ReadAllBytes(file.FilePath)))
                    .ToList() ?? new List<string>(),

                Score = match.Score,
                MatchReason = match.MatchReason
            };
        }
    }


}

