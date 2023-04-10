using System.ComponentModel.DataAnnotations;

namespace AkilliSayac.Models
{
    public class Device
    {
        [Key]
        public int DeviceId { get; set; }
        [Required]
        public string DeviceName { get; set; } = string.Empty;
    }
}
