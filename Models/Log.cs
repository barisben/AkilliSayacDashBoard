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
        [DataType(DataType.Date)]
        public DateTime LogTime { get; set; }
    }
}
