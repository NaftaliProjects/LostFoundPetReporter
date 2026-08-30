using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace LostFoundPetReporter.CoreDb.Models
{
   

    public abstract class BaseModel
    {
        public int Id { get; set; }
    }

    /// <summary>
    /// Holds the profile and contact information for a system user.
    /// </summary>
    public class User : BaseModel
    {
        //properties
        public String Name { get; set; }

        public String HashedPassword { get; set; }
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
        public string Name { get; set; } = "";

        // "Dog", "Cat", etc.
        public string Type { get; set; } = "";

        // "Labrador", "German Shepherd", "Mixed", etc.
        public string Breed { get; set; } = "";

        // "white,black", "brown", etc.
        public string Colors { get; set; } = "";

        // "Male", "Female", "Unknown"
        public string Sex { get; set; } = "";

        // Approximate age in years
        public double? Age { get; set; }

        // "Small", "Medium", "Large"
        public string Size { get; set; } = "";

        // Optional approximate weight
        public double? WeightKg { get; set; }

        // "Short", "Medium", "Long"
        public string CoatLength { get; set; } = "";

        // "Straight", "Curly", "Wavy", etc.
        public string CoatType { get; set; } = "";

        // "Solid", "Spotted", "Striped", "Tabby", etc.
        public string Pattern { get; set; } = "";

        // Human-readable unique features
        // Example: "white patch on chest, scar above left eye"
        public string DistinctiveMarkings { get; set; } = "";

        // "Brown", "Blue", "Green", etc.
        public string EyeColor { get; set; } = "";

        // Example: "Left ear folded"
        public string EarDescription { get; set; } = "";

        // Example: "Long tail with white tip"
        public string TailDescription { get; set; } = "";

        // Collar/accessories
        public bool? CollarPresent { get; set; }

        public string CollarColor { get; set; } = "";

        public string CollarType { get; set; } = "";

        public bool? HarnessPresent { get; set; }

        public string HarnessColor { get; set; } = "";
    }



    /// <summary>
    /// SubModel for FoundReport to hold images data related to a FoundReport
    /// </summary>
    public class FoundReportExtFile : BaseModel
    {
        //properties
        public String FilePath { get; set; }

        public String FileName { get; set; }
        public String Description { get; set; } = "";

        //Foreign keys 
        public int FoundReportId { get; set; }
    }


    public class LostCoordinate 
    {
        public int LostReportId { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        //Nevigation properties 
        public LostReport LostReportNavigation { get; set; }

    }


    public class FoundCoordinate 
    {
        public int FoundReportId { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        //Nevigation properties 
        public FoundReport FoundReportNavigation { get; set; }
    }


    /// <summary>
    /// A found report from any user will be save in the appropriate LostReport
    /// </summary>
    public class FoundReport : BaseModel
    {
        //properties
        public DateTime dateTime { get; set; }

        //Foreign keys 
        public int UserId { get; set; }


        //Nevigation properties 
        public User? UserNevigation { get; set; }
        public FoundCoordinate? FoundCoordinateNavigation { get; set; }
        public List<FoundReportExtFile>? FoundReportExtFilesNevigation { get; set; } = new();
        public AnimalDescription PetDescription { get; set; } = new();
        public List<LostFoundMatch>? LostFoundMatchNevigation { get; set; } = new();

    }


    /// <summary>
    /// Lost report from users that lost their pets and need help in finding it
    /// </summary>
    public class LostReport : BaseModel
    {
        //properties
        public DateTime dateTime { get; set; }


        //Foreign keys 
        public int UserId { get; set; }

        //Nevigation properties
        public User? User { get; set; }
        public LostCoordinate? LostCoordinateNavigation { get; set; }
        public List<LostReportExtFile>? LostReportExtFilesNevigation { get; set; } = new();
        public List<LostFoundMatch>? LostFoundMatchNevigation { get; set; } = new();

        //extra
        public AnimalDescription PetDescription { get; set; } = new();
    }

    /// <summary>
    /// Costume and extandable Many To Many Table for Lost/Found Report tables 
    /// </summary>
    public class LostFoundMatch : BaseModel
    {
        public int LostReportId { get; set; }

        public int FoundReportId { get; set; }

        /// <summary>
        /// Overall matching confidence from 0 to 1.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Optional human-readable explanation.
        /// </summary>
        public string MatchReason { get; set; } = "";

        public LostReport LostReportNevigation { get; set; }

        public FoundReport FoundReportNevigation { get; set; }
    }



    /// <summary>
    /// SubModel for FoundReport to hold images data related to a FoundReport
    /// </summary>
    public class LostReportExtFile : BaseModel
    {
        //properties
        public String FilePath { get; set; }

        public String FileName { get; set; }

        public String Description { get; set; } = "";

        //Foreign keys 
        public int LostReportId { get; set; }
    }


    
}
