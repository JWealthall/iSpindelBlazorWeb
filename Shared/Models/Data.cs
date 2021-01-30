using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace iSpindelBlazorWeb.Shared.Models
{
    [MessagePackObject]
    public class BaseData
    {
        // This is purely so we can pass messages back from the business/data layers
        [MessagePack.Key(0)]
        [NotMapped] public StatusData StatusData { get; set; } = new StatusData();
    }

    [MessagePackObject]
    public class StatusData
    {
        [MessagePack.Key(0)]
        [NotMapped] public int Code { get; set; } = 0;
        [MessagePack.Key(1)]
        [NotMapped] public string Message { get; set; }

        public bool Failed(int code, string message)
        {
            Code = code;
            Message = message;
            return false;
        }

        public static StatusData Failure(string message) { return new StatusData() { Code = -1, Message = message }; }
        public static StatusData Failure(int code, string message) { return new StatusData() { Code = code, Message = message }; }
    }

    [MessagePackObject]
    public class Batch : BaseData
    {
        [MessagePack.Key(1)]
        [Display(Name = "Batch Id")]
        public Guid BatchId { get; set; } = Guid.NewGuid();

        [MessagePack.Key(2)]
        [MaxLength(50)][Required]
        public string Description { get; set; }

        [MessagePack.Key(3)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.UtcNow.TrimToSecond();

        [MessagePack.Key(4)]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [MessagePack.Key(5)]
        [Display(Name = "Device Id")]
        public Guid DeviceId { get; set; }

        [MessagePack.Key(6)]
        public List<Log> Logs { get; set; }

        public Batch Failure(string message) => Failure(-1, message);
        public Batch Failure(int code, string message)
        {
            StatusData = StatusData.Failure(code, message);
            return this;
        }
    }

    [MessagePackObject]
    public class Device : BaseData
    {
        [MessagePack.Key(1)]
        [Display(Name = "Device Id")]
        public Guid DeviceId { get; set; } = Guid.NewGuid();

        [MessagePack.Key(2)]
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }    // This is what's used to match up the iSpindel to this DB

        [MessagePack.Key(3)]
        [MaxLength(100)]
        public string Token { get; set; }    // This could be used for authorisation

        [MessagePack.Key(4)]
        [Display(Name = "Spindel Id")]
        public int? SpindelId { get; set; }

        [MessagePack.Key(5)]
        [MaxLength(50)][Required]
        public string Description { get; set; }

        [MessagePack.Key(6)]
        public List<Log> Logs { get; set; }
        [MessagePack.Key(7)]
        public List<Batch> Batches { get; set; }

        public Device Failure(string message) => Failure(-1, message);
        public Device Failure(int code, string message)
        {
            StatusData = StatusData.Failure(code, message);
            return this;
        }
    }

    [MessagePackObject]
    public class Log : BaseData
    {
        [MessagePack.Key(1)]
        public Guid LogId { get; set; } = Guid.NewGuid();
        [MessagePack.Key(2)]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        [MessagePack.Key(3)]
        [Column(TypeName = "decimal(8, 5)")]
        [DisplayFormat(DataFormatString = "{0:0.00}")]
        public decimal? Angle { get; set; }

        [MessagePack.Key(4)]
        [Column(TypeName = "decimal(7, 3)")]
        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public decimal? Temperature { get; set; }

        [MessagePack.Key(5)]
        [MaxLength(1)]
        public string TempUnits { get; set; }

        [MessagePack.Key(6)]
        [Column(TypeName = "decimal(8, 6)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? Battery { get; set; }

        [MessagePack.Key(7)]
        [Column(TypeName = "decimal(8, 6)")]
        [DisplayFormat(DataFormatString = "{0:0.000}")]
        public decimal? Gravity { get; set; }

        [MessagePack.Key(8)]
        public int? Interval { get; set; }
        [MessagePack.Key(9)]
        public int? RSSI { get; set; }
        [MessagePack.Key(10)]
        public Guid DeviceId { get; set; }
        [MessagePack.Key(11)]
        public Guid? BatchId { get; set; }

        public Log Failure(string message) => Failure(-1, message);
        public Log Failure(int code, string message)
        {
            StatusData = StatusData.Failure(code, message);
            return this;
        }
    }
}
