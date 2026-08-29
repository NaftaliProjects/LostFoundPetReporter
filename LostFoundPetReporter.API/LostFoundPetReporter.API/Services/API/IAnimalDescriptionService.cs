using LostFoundPetReporter.API.DTO;

namespace LostFoundPetReporter.API.Services.API
{
    public interface IAnimalDescriptionService
    {
        Task<AnimalDescriptionDto> ImageToAnimalDescriptionAsync(
            List<string> pictureBase64List, 
            CancellationToken cancellationToken = default
        );


    }
}
