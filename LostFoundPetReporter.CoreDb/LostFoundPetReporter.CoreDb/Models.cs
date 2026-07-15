using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using static LostFoundPetReporter.CoreDb.Models;

namespace LostFoundPetReporter.CoreDb
{
    public class Models
    {

        public abstract class BaseModel
        {
            public int Id { get; set; }
            [Timestamp]
            public Byte[] TimeStamp { get; set; }
        }

        /// <summary>
        /// Holds the profile and contact information for a system user.
        /// </summary>
        public class User : BaseModel
        {
            //properties
            public String Name { get; set; }
            public String Password { get; set; }
            public String Email { get; set; }
            public String Phone { get; set; }

        }



        /// <summary>
        /// Holds the description of a pet in a LostReport or a FoundReport.
        /// <para>
        /// Colors examples:
        /// - "Red"
        /// - "White,Black,Grey"
        /// </para>
        /// </summary>
        [Owned]
        public class AnimalDescription 
        {
            //properties
            public String Name { get; set; } = "";
            public String Colors { get; set; } = "";
            public String Type { get; set; } = "";
            public String Breed { get; set; } = "";
        }

  

        /// <summary>
        /// SubModel for FoundReport to hold images data related to a FoundReport
        /// </summary>
        public class FoundReportExtFile : BaseModel
        {
            //properties
            [Required]
            required public String FilePath { get; set; }
            [Required]
            required public String FileName { get; set; }
            public String Description { get; set; } = "";

            //Foreign keys 
            [Required]
            required public int FoundReportId { get; set; }
        }

 

        /// <summary>
        /// A found report from any user will be save in the appropriate LostReport
        /// </summary>
        public class FoundReport : BaseModel
        {
            //properties
            public String FoundCoordinates { get; set; } = "";

            //Foreign keys 
            required public int UserId { get; set; }


            //Nevigation properties 
            [Required]
            required public User User { get; set; }
            public List<FoundReportExtFile> ExtFiles { get; set; } = new();
            [Required]
            required public AnimalDescription PetDescription { get; set; }
            public List<LostReport> LostReports  { get; set; } = new ();

        }


        /// <summary>
        /// Lost report from users that lost their pets and need help in finding it
        /// </summary>
        public class LostReport : BaseModel
        {
            //properties
            public String LastSeenCoordinates { get; set; } = "";


            //Foreign keys 
            public int UserId { get; set; }

            //Nevigation properties
            [Required]
            required public User User { get; set; }
            public AnimalDescription PetDescription { get; set; }
            public List<LostReportExtFile> ExtFiles { get; set; } = new();
            public List<FoundReport> FoundReports  { get; set; } = new();
        }

        


        /// <summary>
        /// SubModel for FoundReport to hold images data related to a FoundReport
        /// </summary>
        public class LostReportExtFile : BaseModel
        {
            //properties
            public String FilePath { get; set; }
            public String FileName { get; set; }
            public String Description { get; set; }
            //Foreign keys 
            public int LostReportId { get; set; }
        }


    }
}
