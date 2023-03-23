using System.ComponentModel.DataAnnotations;

namespace AkilliSayac.Models
{
    public class AnomalyType
    {
        [Key]
        public int AnomalyTypeId { get; set; }
        [Required]
        public string AnomalyTypeName { get; set; } = string.Empty;
    }
}
