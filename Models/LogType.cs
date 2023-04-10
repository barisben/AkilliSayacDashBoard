using System.ComponentModel.DataAnnotations;

namespace AkilliSayac.Models
{
    public class LogType
    {
        [Key]
        public int LogTypeId { get; set; }
        [Required]
        public string LogTypeName { get; set; } = string.Empty;
        public int ?LogMessageNumber { get; set; }
    }
}
