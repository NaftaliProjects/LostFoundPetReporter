using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models.Map
{
    public class MapReportGroup
    {
        public int LostReportId { get; set; }

        public MapPoint? LostPoint { get; set; }

        public List<MapPoint> FoundPoints { get; set; } = new();

        // Color identifier for this group
        public string Color { get; set; } = string.Empty;
    }
}
