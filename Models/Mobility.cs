using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Models
{
    public class Mobility
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Promotion { get; set; }

        [Required]
        public string StudentName { get; set; }
        [Column(TypeName = "date")]
        public DateTime StartingDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime EndingDate { get; set; }

        public DateTime SubmissionDate { get; set; }
        public Boolean State { get; set; }
    }
}