using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class FoundReport
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Today;
        public FoundCoordinate? FoundCoordinate { get; set; }
        public User? User { get; set; }
        public AnimalDescription PetDescription { get; set; } = new();
        public List<string> PictureBase64List { get; set; } = new();
    }
}
