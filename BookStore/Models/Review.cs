using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace BookStore.Models
{
	public class Review
	{
		[Key]
		public int ReviewId { get; set; }
		[Required(ErrorMessage = "Acest camp este obligatoriu")]
		public int Rating { get; set; } //1-5 stele
		[Required]
		[MaxLength(150)]
		public string Comment { get; set; }
		public DateTime Date { get; set; }
		[Required]
		public int BookId { get; set; }

		public virtual Book Book { get; set; }

		public string UserId { get; set; } 


		public virtual ApplicationUser User { get; set; }

	}
}