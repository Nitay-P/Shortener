using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
		[Required]
		public string? LastName { get; set; }
		[Required]
        [EmailAddress]
		public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$",ErrorMessage ="Password Does not meet requirements.\nPassword needs to be atleast 8 characters. \nInclude atleast 1 uppercase character.\nInclude 1 lowercase character.\nInclude 1 number.")]
        public string? Password { get; set; }
        [NotMapped]
        public string? ConfirmPassword { get; set; }

        public virtual ICollection<UrlInfo>? Links { get; set; }
        
    }
}