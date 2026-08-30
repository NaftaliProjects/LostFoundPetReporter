using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class LostReport
    {
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        public int UserId { get; set; }

        public User? User { get; set; }

        public LostCoordinate? LostCoordinate { get; set; }

        public AnimalDescription? PetDescription { get; set; }

        public List<string> PictureBase64List { get; set; } = new();

        public List<FoundReport> FoundReports { get; set; } = new();



    }
}
