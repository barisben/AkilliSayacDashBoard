using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AkilliSayac.Models
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        [Required]
        [ForeignKey("LogTypes")]
        public int LogTypeId { get; set; }
        [Required]
        [ForeignKey("Devices")]
        public int DeviceId { get; set; } = 0;
        [Required]
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string LogMessage { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Date)]
        public DateTime LogTime { get; set; }
    }
}
