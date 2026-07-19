using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace LostFoundPetReporter.CoreDb.Models
{
   

    public abstract class BaseModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Timestamp]
        required public Byte[] TimeStamp { get; set; }
    }

    /// <summary>
    /// Holds the profile and contact information for a system user.
    /// </summary>
    public class User : BaseModel
    {
        //properties
        [Required]
        required public String Name { get; set; }
        [Required]
        required public String HashedPassword { get; set; }
        public String Email { get; set; } = "";
        public String Phone { get; set; } = "";

    }



    /// <summary>
    /// Holds the description of a pet in a LostReport or a FoundReport.
    /// <para>
    /// Colors examples:
    /// - "Red"
    /// - "White,Black,Grey"
    /// </para>
    /// </summary>
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
        public String Coordinates { get; set; } = "";
        public DateTime dateTime { get; set; }

        //Foreign keys 
        public int UserId { get; set; }


        //Nevigation properties 
        [Required]
        required public User User { get; set; }
        public List<FoundReportExtFile> ExtFiles { get; set; } = new();
        public AnimalDescription PetDescription { get; set; } = new();
        public List<LostFoundMatch> Matches { get; set; } = new();

    }


    /// <summary>
    /// Lost report from users that lost their pets and need help in finding it
    /// </summary>
    public class LostReport : BaseModel
    {
        //properties
        public String Coordinates { get; set; } = "";
        public DateTime dateTime { get; set; }


        //Foreign keys 
        public int UserId { get; set; }

        //Nevigation properties
        [Required]
        required public User User { get; set; }
        public AnimalDescription PetDescription { get; set; } = new();
        public List<LostReportExtFile> ExtFiles { get; set; } = new();
        public List<LostFoundMatch> Matches { get; set; } = new();
    }

    /// <summary>
    /// Costume and extandable Many To Many Table for Lost/Found Report tables 
    /// </summary>
    public class LostFoundMatch : BaseModel
    {
        public int LostReportId { get; set; }
        public int FoundReportId { get; set; }

        required public LostReport LostReport { get; set; }
        required public FoundReport FoundReport { get; set; }
    }



    /// <summary>
    /// SubModel for FoundReport to hold images data related to a FoundReport
    /// </summary>
    public class LostReportExtFile : BaseModel
    {
        //properties
        [Required]
        required public String FilePath { get; set; }
        [Required]
        required public String FileName { get; set; }

        public String Description { get; set; } = "";

        //Foreign keys 
        [Required]
        required public int LostReportId { get; set; }
    }


    
}
