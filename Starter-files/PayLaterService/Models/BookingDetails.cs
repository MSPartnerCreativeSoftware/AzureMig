using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PayLaterService.Models
{
	public class BookingDetails
	{

        public long id { set; get; }
        public string frstName { set; get; }
        public string lastName { set; get; }
        public DateTime bookingDate { set; get; }
        public String status { get; set; }
    }
}
