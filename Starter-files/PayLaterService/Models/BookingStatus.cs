using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayLaterService.Models
{
	public class BookingStatus
	{
		public int Id { get; set; }
		public long clientId { get; set; }
		public String status { get; set; }
	}
}
