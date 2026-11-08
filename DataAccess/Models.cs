global using KeyType = int;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text;



namespace LostFoundPetReporter.CoreDb.Models
{
    public enum ReportType { Lost, Found }
    public enum DevicePlatform { Android, iOS, Web }
    public enum MatchStatus { Pending, Confirmed, Rejected }


    /// <summary>
    /// Holds the info for Push notification system
    /// </summary>
    public class UserDevice 
    {
        required public KeyType Id { get; set; }
        required public string Token { get; set; }
        public DevicePlatform Platform { get; set; }
        required public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        required public User UserNavigation { get; set; }
    }

    /// <summary>
    /// Holds the profile and contact information for a system user.
    /// </summary>
    public class User 
    {
        required public KeyType Id { get; set; }
        required public String Name { get; set; }
        required public String HashedPassword { get; set; }
        public String? Email { get; set; } = null;
        public String? Phone { get; set; } = null;
        public ICollection<UserDevice> Devices { get; set; } = [];
        public ICollection<Report> Reports { get; set; } = [];

    }


   
    public class ExtFile 
    {
        required public KeyType Id { get; set; }
        required public String FilePath { get; set; }

        required public String FileName { get; set; }
        public String? Description { get; set; } = null;

        required public Report ReportNavigation { get; set; }
    }


    //Complex Type
    public class Coordinate 
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }


    public class Report 
    {
        required public KeyType Id { get; set; }
        //properties
        public DateTime DateReported { get; set; } = DateTime.UtcNow;

        required public ReportType Type { get; set; }


        //Navigation properties 
        public User? UserNavigation { get; set; }
        public Coordinate? Coordinate { get; set; } = null;
        public ICollection<ExtFile>? ExtFilesNavigation { get; set; } = [];
        public IDictionary<string, string>? AnimalDescriptions { get; set; } = null;
        public ICollection<LostFoundMatch> MatchesAsLost { get; set; } = [];
        public ICollection<LostFoundMatch> MatchesAsFound { get; set; } = [];



    }


    

    


    



    /// <summary>
    /// Costume and extandable Many To Many Table for Lost/Found Report tables 
    /// </summary>
    public class LostFoundMatch
    {
        required public KeyType Id { get; set; }
        /// <summary>
        /// Overall matching confidence from 0 to 1.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Optional human-readable explanation.
        /// </summary>
        public string MatchReason { get; set; } = "";

        public Report LostReportNavigation { get; set; } = null!;
        public Report FoundReportNavigation { get; set; } = null!;

    }

    
}
