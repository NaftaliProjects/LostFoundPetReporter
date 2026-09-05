using LostFoundPetReporter.Mobile.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Services.Api
{

    public interface ILostReportApiService
    {
        Task<IEnumerable<LostReport>?> GetLostReportsAsync();

        Task<LostReport?> GetLostReportAsync(int id);
        Task<IEnumerable<LostReport>?> GetLostReportByUserIdAsync(int id);

        Task<LostReport?> CreateLostReportAsync(CreateLostReportRequest request);

        Task UpdateLostReportAsync(int id, CreateLostReportRequest request);

        Task DeleteLostReportAsync(int id);

        Task<CreateAnimalDescription?> ImageToAnimalDescriptionAsync(List<string> pictureBase64List);
        Task RemoveLostFoundMatchAsync(int lostReportId, int foundReportId);


    }
}
