using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace movie_hospital_1.dataModel
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string OrderStatus { get; set; }

        // Navigation property - لازم تكون Virtual
        [ForeignKey("MovieId")]
        public virtual Movie? Movie { get; set; }
    }
}