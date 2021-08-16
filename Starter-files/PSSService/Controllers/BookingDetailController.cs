using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PSSService.Models; 
namespace PSSService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingDetailController : ControllerBase
    {
        [HttpGet("{id}")]


        public async Task<BookingDetail> Get(long id)
        {
            ServiceReference1.SampleServiceClient sampleService = new ServiceReference1.SampleServiceClient();
            var data = await sampleService.GetBookingDetailsAsync(id);
            var resBody = data.Body.GetBookingDetailsResult;

            return (new BookingDetail
            {
                Id = resBody.id,
                firstName = resBody.firstName,
                lastName = resBody.lastName,
                bookingDate = resBody.bookingDate,
                status = resBody.status,
            });

        }
    }
}
