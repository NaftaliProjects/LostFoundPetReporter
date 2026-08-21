using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class FoundReport
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Coordinates { get; set; } = string.Empty;
        public DateTime dateTime { get; set; }

        public User? User { get; set; }
        public AnimalDescription PetDescription { get; set; } = new();
    }
}
