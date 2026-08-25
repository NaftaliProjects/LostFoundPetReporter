using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class LostReport
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Coordinates { get; set; } = string.Empty;
        public DateTime DateTime { get; set; } = DateTime.Today;

        public User? User { get; set; }
        public LostCoordinate? LostCoordinate { get; set; }   
        public List<FoundReport> FoundReports { get; set; } = new();
        public AnimalDescription PetDescription { get; set; } = new();



    }
}
