    using LostFoundPetReporter.API.DTO.Interfaces;

    namespace LostFoundPetReporter.API.DTO
    {
        public class BaseResponseDto : IDto
        {
            public int Id { get; set; }
        }


        public class BaseCreateOrUpdateDto : IDto
        {
            public int? Id { get; set; }
        }
    }
