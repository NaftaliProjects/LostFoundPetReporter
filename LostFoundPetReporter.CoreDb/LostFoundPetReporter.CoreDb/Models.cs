using System;
using System.Collections.Generic;
using System.Text;

namespace LostFoundPetReporter.CoreDb
{
    public class Models
    {

        public abstract class BaseModel
        {
            public int Id { get; set; }
            public Byte[] TimeStamp { get; set; }
        }

        /// <summary>
        /// Holds the profile and contact information for a system user.
        /// </summary>
        public class User : BaseModel
        {
            public String Name { get; set; }
            public String Email { get; set; }
            public String Phone { get; set; }

        }

        /// <summary>
        /// Hold Description of a Pet in a LostReport or a FoundReport 
        /// </summary>
        public class AnimalDescription
        {
            public String Name { get; set; }
            public String Color { get; set; }
            public String Type { get; set; }
            public String SubType { get; set; }
        }

        /// <summary>
        /// SubModel for FoundReport to hold images data related to a FoundReport
        /// </summary>
        public class FoundReportExtFile
        {
            public String FilePath { get; set; }
            public String FileName { get; set; }
            public String Description { get; set; }
            public int FoundReportId { get; set; }
        }

        /// <summary>
        /// A found report from any user will be save in the appropriate LostReport
        /// </summary>
        public class FoundReport
        {
            public AnimalDescription LostPetDesc { get; set; }
            public User FoundReporter { get; }
            public String FoundCoordinates { get; set; }
            public int LostReportId { get; set; }
            public List<FoundReportExtFile> Extfiles { get; set; }

        }


        /// <summary>
        /// Lost report from users that lost their pets and need help in finding it
        /// </summary>
        public class LostReport
        {
            public AnimalDescription LostPetDesc { get; set; }
            public User LostReporter { get; }
            public String LastSeenCoordinates { get; set; }
            public List<LostReportExtFile> Extfiles { get; set; }
            public List<FoundReport> foundReports = new List<FoundReport>();
        }


        /// <summary>
        /// SubModel for FoundReport to hold images data related to a FoundReport
        /// </summary>
        public class LostReportExtFile
        {
            public String FilePath { get; set; }
            public String FileName { get; set; }
            public String Description { get; set; }
            public int LostReportId { get; set; }
        }
    }
}
