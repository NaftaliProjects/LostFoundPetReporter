using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;


namespace LostFoundPetReporter.CoreDb.Models
{
   

    public abstract class BaseModel
    {
        required public int Id { get; set; }
    }

    /// <summary>
    /// Holds the info for Push notification system
    /// </summary>
    public class UserDevice : BaseModel
    {
        //properties
        public int UserId { get; set; }
        public string Token { get; set; } = "";
        public string Platform { get; set; } = "";
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Holds the profile and contact information for a system user.
    /// </summary>
    public class User : BaseModel
    {
        //properties
        required public String Name { get; set; }

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


    public class ExtFile : BaseModel
    {
        //properties
        required public String FilePath { get; set; }

        required public String FileName { get; set; }
        public String Description { get; set; } = "";

        //Foreign keys 
        required public int ReportId { get; set; }
    }


    public class Coordinate : BaseModel
    {
        required public double Latitude { get; set; }
        required public double Longitude { get; set; }

        required public int ReportId { get; set; }
        required public Report ReportNavigation { get; set; }
    }

    


    public class Report : BaseModel
    {
        //properties
        public DateTime DateReported { get; set; }

        //(L : Lost, F : Found)
        required public string ReportType { get; set; }

        //Foreign keys 
        public int UserId { get; set; }


        //Nevigation properties 
        required public User UserNevigation { get; set; }
        public Coordinate? ReportCoordinateNavigation { get; set; }
        public List<ExtFile>? ReportExtFilesNevigation { get; set; } = new();
        public AnimalDescription PetDescription { get; set; } = new();
        public List<LostFoundMatch> LostFoundMatchNevigation { get; set; } = new();

    }


    

    


    



    /// <summary>
    /// Costume and extandable Many To Many Table for Lost/Found Report tables 
    /// </summary>
    public class LostFoundMatch : BaseModel
    {
        required public int LostReportId { get; set; }

        required public int FoundReportId { get; set; }

        /// <summary>
        /// Overall matching confidence from 0 to 1.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Optional human-readable explanation.
        /// </summary>
        public string MatchReason { get; set; } = "";

        required public Report LostReportNevigation { get; set; }

        required public Report FoundReportNevigation { get; set; }

    }

    
}
