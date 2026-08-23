using LostFoundPetReporter.Mobile.Models.Map;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LostFoundPetReporter.Mobile.ViewModels
{
    public class MapViewModel
    {
        public ObservableCollection<MapPoint> Points { get; } = new();

    }
}
