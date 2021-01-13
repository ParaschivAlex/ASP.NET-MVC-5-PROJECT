using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BookStore.Models
{
	public class Basket
	{	[Key]
		public int BasketId { get; set; }
		public string UserId { get; set; }

		public virtual ApplicationUser User { get; set; }
		public virtual ICollection<Is_In> Is_Ins { get; set; }
	}
}