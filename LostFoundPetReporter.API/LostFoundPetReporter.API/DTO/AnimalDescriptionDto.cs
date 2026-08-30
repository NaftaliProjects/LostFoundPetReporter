using LostFoundPetReporter.API.DTO.Interfaces;

namespace LostFoundPetReporter.API.DTO
{
    public class CreateAnimalDescriptionDto
        : IEntityDto<AnimalDescription>
    {
        public string Name { get; set; } = "";

        // "Dog", "Cat", etc.
        public string Type { get; set; } = "";

        // "Labrador", "German Shepherd", "Mixed", etc.
        public string Breed { get; set; } = "";

        // "white,black", "brown", etc.
        public string Colors { get; set; } = "";

        // "Male", "Female", "Unknown"
        public string Sex { get; set; } = "";

        // Approximate age in years
        public double? Age { get; set; }

        // "Small", "Medium", "Large"
        public string Size { get; set; } = "";

        // Optional approximate weight
        public double? WeightKg { get; set; }

        // "Short", "Medium", "Long"
        public string CoatLength { get; set; } = "";

        // "Straight", "Curly", "Wavy", etc.
        public string CoatType { get; set; } = "";

        // "Solid", "Spotted", "Striped", "Tabby", etc.
        public string Pattern { get; set; } = "";

        // Human-readable unique features
        // Example: "white patch on chest, scar above left eye"
        public string DistinctiveMarkings { get; set; } = "";

        // "Brown", "Blue", "Green", etc.
        public string EyeColor { get; set; } = "";

        // Example: "Left ear folded"
        public string EarDescription { get; set; } = "";

        // Example: "Long tail with white tip"
        public string TailDescription { get; set; } = "";

        // Collar/accessories
        public bool? CollarPresent { get; set; }

        public string CollarColor { get; set; } = "";

        public string CollarType { get; set; } = "";

        public bool? HarnessPresent { get; set; }

        public string HarnessColor { get; set; } = "";





        public AnimalDescription ToEntity()
        {
            return new AnimalDescription
            {
                Name = Name,
                Type = Type,
                Breed = Breed,
                Colors = Colors,

                Sex = Sex,
                Age = Age,
                Size = Size,
                WeightKg = WeightKg,

                CoatLength = CoatLength,
                CoatType = CoatType,
                Pattern = Pattern,

                DistinctiveMarkings = DistinctiveMarkings,
                EyeColor = EyeColor,
                EarDescription = EarDescription,
                TailDescription = TailDescription,

                CollarPresent = CollarPresent,
                CollarColor = CollarColor,
                CollarType = CollarType,

                HarnessPresent = HarnessPresent,
                HarnessColor = HarnessColor
            };
        }
    }




    public class AnimalDescriptionDto : IResponseDto<AnimalDescription, AnimalDescriptionDto> 
    {
        public string Name { get; set; } = "";

        // "Dog", "Cat", etc.
        public string Type { get; set; } = "";

        // "Labrador", "German Shepherd", "Mixed", etc.
        public string Breed { get; set; } = "";

        // "white,black", "brown", etc.
        public string Colors { get; set; } = "";

        public static AnimalDescriptionDto FromEntity(AnimalDescription entity)
        {
           
            return new AnimalDescriptionDto 
            {
                Name = entity.Name,
                Type = entity.Type,
                Breed = entity.Breed,
                Colors = entity.Colors,

            };
        }
    }
}