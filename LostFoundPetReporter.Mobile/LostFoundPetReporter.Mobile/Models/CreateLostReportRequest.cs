using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.Mobile.Models
{
    public class CreateLostReportRequest
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public string Coordinates { get; set; }
        public DateTime dateTime { get; set; }

        public AnimalDescription PetDescription { get; set; } = new();

    }
}
