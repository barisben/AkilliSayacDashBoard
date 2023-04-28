using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkilliSayac.Models
{
    public class Anomaly
    {
        [Key]
        public int AnomalyId { get; set; }
        [Required]
        [ForeignKey("AnomalyTypes")]
        public int AnomalyTypeId { get; set; }
        [Required]
        [ForeignKey("Devices")]
        public int DeviceId { get; set; }
        [Required]
        public string AnomalyMessage { get; set; } = string.Empty;
        [Required]
        public int AnomalyValue { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime AnomalyTime { get; set; }
    }
}
