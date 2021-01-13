using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
	public class Is_In
	{
		[Key]
		public int Is_InId { get; set; }
		public int BookId { get; set; }
		public int BasketId { get; set; }
		public int Quantity { get; set; }

		public virtual Book Book { get; set; }
		public virtual Basket Basket { get; set; }
	}
}