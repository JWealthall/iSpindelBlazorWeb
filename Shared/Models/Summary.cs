using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace iSpindelBlazorWeb.Shared.Models
{
    [MessagePackObject]
    public class SummaryDataModel
    {
        [MessagePack.Key(0)]
        public Guid? DeviceId { get; set; }
        [MessagePack.Key(1)]
        public Guid? BatchId { get; set; }
        [MessagePack.Key(2)]
        public List<DeviceSummaryModel> Devices { get; set; }
        [MessagePack.Key(3)]
        public List<BatchSummaryModel> Batches { get; set; }
        [MessagePack.Key(4)]
        public DateTime LastUpdated { get; set; }

        [MessagePack.Key(5)]
        public Batch Batch { get; set; }

        [IgnoreMember]
        public string DeviceName
        {
            get
            {
                if (!DeviceId.HasValue) return "Multiple";
                if (Devices == null || Devices.Count == 0) return "Unknown ";
                return Devices[0].Name;
            }
        }

        [IgnoreMember]
        public string BatchName
        {
            get
            {
                if (!BatchId.HasValue) return "Multiple";
                if (Batches == null || Batches.Count == 0) return "Unknown ";
                return Batches[0].Description;
            }
        }
    }

    [MessagePackObject]
    public class BatchSummaryModel
    {
        public BatchSummaryModel() { }
        public BatchSummaryModel(Batch b, LogSummaryModel ls, LogSummaryModel le, decimal? lx, decimal? ln, decimal? lh, decimal? ll, decimal? la)
        {
            BatchId = b.BatchId;
            Description = b.Description;
            StartDate = b.StartDate;
            EndDate = b.EndDate;

            if (ls != null)
            {
                FirstLogDate = ls.Date;
                StartGravity = ls.Gravity;
                StartTemperature = ls.Temperature;
                TempUnits = ls.TempUnits;
            };

            if (le != null)
            {
                LastLogDate = le.Date;
                EndGravity = le.Gravity;
                EndTemperature = le.Temperature;
            };

            if (lx != null) MaxGravity = lx;
            if (ln != null) MinGravity = ln;
            if (lx != null) MaxTemperature = lh;
            if (ln != null) MinTemperature = ll;
            if (la != null) AvgTemperature = la;
        }
        [MessagePack.Key(0)]
        public Guid BatchId { get; set; }
        [MessagePack.Key(1)]
        public string Description { get; set; }
        [MessagePack.Key(2)]
        public string TempUnits { get; set; }

        [MessagePack.Key(3)]
        [Display(Name = "Start")]
        public DateTime StartDate { get; set; }

        [MessagePack.Key(4)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? StartGravity { get; set; }
        
        [MessagePack.Key(5)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? StartTemperature { get; set; }
        
        [MessagePack.Key(6)]
        [Display(Name = "End")]
        public DateTime? EndDate { get; set; }
        
        [MessagePack.Key(7)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? EndGravity { get; set; }
        
        [MessagePack.Key(8)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? EndTemperature { get; set; }
        
        [MessagePack.Key(9)]
        public DateTime? FirstLogDate { get; set; }
        
        [MessagePack.Key(10)]
        public DateTime? LastLogDate { get; set; }
        [MessagePack.Key(11)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? MaxGravity { get; set; }
        
        [MessagePack.Key(12)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? MinGravity { get; set; }
        
        [MessagePack.Key(13)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? MaxTemperature { get; set; }
        
        [MessagePack.Key(14)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? MinTemperature { get; set; }
        
        [MessagePack.Key(15)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? AvgTemperature { get; set; }

        [IgnoreMember]
        [DisplayFormat(DataFormatString = "{0:0.00}%")]
        public decimal? Abv
        {
            get
            {
                if (!StartGravity.HasValue || !EndGravity.HasValue) return null;
                return (StartGravity - EndGravity) * (decimal?)131.25;
            }
        }

        [IgnoreMember]
        [DisplayFormat(DataFormatString = "{0:0.00}%")]
        [Display(Name = "Abv H/L")]
        public decimal? AbvMaxMin
        {
            get
            {
                if (!MinGravity.HasValue || !MaxGravity.HasValue) return null;
                return (MaxGravity - MinGravity) * (decimal?)131.25;
            }
        }

        [IgnoreMember]
        [DisplayFormat(DataFormatString = "{0:0.000} days")]
        public decimal? Duration
        {
            get
            {
                if (!FirstLogDate.HasValue || !LastLogDate.HasValue) return null;
                var t = LastLogDate - FirstLogDate;
                return (decimal)t.Value.Ticks / (decimal)TimeSpan.TicksPerDay;
            }
        }
        [MessagePack.Key(16)]
        public bool IsDetail { get; set; } = false;
    }

    [MessagePackObject]
    public class DeviceSummaryModel
    {
        public DeviceSummaryModel() { }
        public DeviceSummaryModel(Device d, Log l)
        {
            DeviceId = d.DeviceId;
            Name = d.Name;
            Token = d.Token;
            SpindelId = d.SpindelId;
            Description = d.Description;

            if (l == null) return;
            Date = l.Date;
            Angle = l.Angle;
            Temperature = l.Temperature;
            TempUnits = l.TempUnits;
            Battery = l.Battery;
            Gravity = l.Gravity;
            Interval = l.Interval;
            RSSI = l.RSSI;
        }

        [MessagePack.Key(0)]
        public Guid DeviceId { get; set; }
        [MessagePack.Key(1)]
        public string Name { get; set; }    // This is what's used to match up the iSpindel to this DB
        [MessagePack.Key(2)]
        public string Token { get; set; }    // This could be used for authorisation
        [MessagePack.Key(3)]
        [Display(Name = "Spindel Id")]
        public int? SpindelId { get; set; }
        [MessagePack.Key(4)]
        public string Description { get; set; }

        [MessagePack.Key(5)]
        [Display(Name = "Last Log")]
        public DateTime? Date { get; set; }
        [MessagePack.Key(6)]
        public decimal? Angle { get; set; }
        [MessagePack.Key(7)]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        [Display(Name = "Temp")]
        public decimal? Temperature { get; set; }
        [MessagePack.Key(8)]
        public string TempUnits { get; set; }
        [MessagePack.Key(9)]
        public decimal? Battery { get; set; }
        [MessagePack.Key(10)]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? Gravity { get; set; }
        [MessagePack.Key(11)]
        public int? Interval { get; set; }
        [MessagePack.Key(12)]
        public int? RSSI { get; set; }

        [MessagePack.Key(13)]
        public bool IsDetail { get; set; } = false;
    }

    [MessagePackObject]
    public class LogSummaryModel
    {
        public LogSummaryModel() { }
        public LogSummaryModel( Guid? batchId, DateTime? date, decimal? gravity, decimal? temperature, string tempUnits = null)
        {
            BatchId = batchId;
            Date = date;
            Gravity = gravity;
            Temperature = temperature;
            TempUnits = tempUnits;
        }

        [MessagePack.Key(0)]
        public Guid? BatchId { get; set; }
        [MessagePack.Key(1)]
        public DateTime? Date { get; set; }
        [MessagePack.Key(2)]
        public decimal? Gravity { get; set; }
        [MessagePack.Key(3)]
        public decimal? Temperature { get; set; }
        [MessagePack.Key(4)]
        public string TempUnits { get; set; }
    }
}
