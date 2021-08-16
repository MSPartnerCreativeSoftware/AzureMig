using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PSSService.Models
{
    public class BookingDetail
    {

        public long Id { set; get; }
        public string firstName { set; get; }
        public string lastName { set; get; }
        public DateTime bookingDate { set; get; }
        public String status { get; set; }
    }
}
