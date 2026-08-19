using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{
    public class AnimalDescriptionDto
        : IResponseDto<AnimalDescription, AnimalDescriptionDto>,
          IEntityDto<AnimalDescription>
    {
        public string Name { get; set; } = "";
        public string Colors { get; set; } = "";
        public string Type { get; set; } = "";
        public string Breed { get; set; } = "";


        public static AnimalDescriptionDto FromEntity(AnimalDescription entity)
        {
            return new AnimalDescriptionDto
            {
                Name = entity.Name,
                Colors = entity.Colors,
                Type = entity.Type,
                Breed = entity.Breed
            };
        }


        public AnimalDescription ToEntity()
        {
            return new AnimalDescription
            {
                Name = Name,
                Colors = Colors,
                Type = Type,
                Breed = Breed
            };
        }
    }
}
