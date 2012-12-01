using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LerosClient.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// 

    [DotLiquid.LiquidType("Strikes", "Laps", "Duration", "Distance")]
    public class Activity 
    {
         private int strikes;
         private int laps;
         private int duration;
         private int distance;
         public int Strikes
         {
             get
             {
                 return strikes;
             }
             set
             {
                 this.strikes = value;
             }
         }
         public int Laps
         {
             get
             {
                 return this.laps;
             }
             set { this.laps = value; }
         }
         public int Duration
         {
             get
             {
                 return this.duration;
             }
             set
             {
                 this.duration = value;
             }
         }
         public int Distance
         {
             get
             {
                 return this.distance;
             }
             set
             {
                 this.distance = value;
             }
         }
    }
    /// <summary>
    /// 
    /// </summary>
    [DotLiquid.LiquidType("Time", "Description", "Title")]
    public class Session
    {
        public Activity[] activities;
        
        private String time;
        public String Time
        {
            get
            {
                return this.Time;
            }
            set
            {
                this.time = value;
            }
        }
        private String description;
        public String Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }
        private String title;
        public String Title
        {
            get
            {
                return this.title;
            }
            set
            {
                this.title = value;
            }
        }
    }
}
