using LostFoundPetReporter.Mobile.Models;
using LostFoundPetReporter.Mobile.Services.Api;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LostFoundPetReporter.Mobile.ViewModels
{
    public class HomeViewModel
    {
        private readonly ILostReportApiService _lostReportApiService;


        public ObservableCollection<LostReport> LostReports { get; }
            = new();

        public HomeViewModel(
            ILostReportApiService lostReportApiService)
        {
            _lostReportApiService = lostReportApiService;
        }

        public async Task LoadLostReportsAsync()
        {
            var reports = await _lostReportApiService.GetLostReportsAsync();

            LostReports.Clear();

            foreach (var report in reports)
            {
                LostReports.Add(report);
            }
        }
    }
}
