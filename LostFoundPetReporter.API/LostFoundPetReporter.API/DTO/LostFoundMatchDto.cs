using LostFoundPetReporter.API.DTO.Interfaces;
using LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.API.DTO
{
    public class LostFoundMatchDto
        : IResponseDto<LostFoundMatch, LostFoundMatchDto>, IHasId
    {
        public int? Id { get; set; }

        public int LostReportId { get; set; }

        public int FoundReportId { get; set; }


        public static LostFoundMatchDto FromEntity(
            LostFoundMatch entity)
        {
            return new LostFoundMatchDto
            {
                Id = entity.Id,
                LostReportId = entity.LostReportId,
                FoundReportId = entity.FoundReportId
            };
        }
    }


    public class CreateLostFoundMatchDto
        : IEntityDto<LostFoundMatch>, IHasId
    {
        public int? Id { get; set; }

        public int LostReportId { get; set; }

        public int FoundReportId { get; set; }


        public LostFoundMatch ToEntity()
        {
            return new LostFoundMatch
            {
                Id = Id ?? 0,
                LostReportId = LostReportId,
                FoundReportId = FoundReportId
            };
        }
    }
}