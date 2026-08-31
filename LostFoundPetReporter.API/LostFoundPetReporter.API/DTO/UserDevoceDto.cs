using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{
    public class CreateUserDeviceDto : IEntityDto<UserDevice>, IHasId
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public string Platform { get; set; } = "";
        public DateTime LastUpdated { get; set; }


        public UserDevice ToEntity()
        {
            return new UserDevice
            {
                Id = Id ?? 0,
                UserId = UserId,
                Token = Token,
                Platform = Platform,
                LastUpdated = LastUpdated,

            };
        }
    }
}
