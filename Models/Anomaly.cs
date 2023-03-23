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
        [DataType(DataType.Date)]
        public DateTime AnomalyTime { get; set; }
    }
}
