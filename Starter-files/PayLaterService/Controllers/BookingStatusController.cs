using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayLaterService.Models;

namespace PayLaterService.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class BookingStatusController : ControllerBase
	{
		private readonly IHttpClientFactory _clientFactory;

		public BookingStatusController(IHttpClientFactory clientFactory)
		{
			_clientFactory = clientFactory;
		}

		[HttpGet("{id}")]
		public async Task<BookingStatus> Get(int id)
		{
			var PSSServiceApi = Environment.GetEnvironmentVariable("PSS_SERVICE_API");
			if (PSSServiceApi == null)
            {
				PSSServiceApi = "localhost:5002";

			}
			var request = new HttpRequestMessage(HttpMethod.Get, "http://"+PSSServiceApi+"/api/bookingdetail/" + id);
			
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);
			var responseStream = await response.Content.ReadAsStreamAsync();
			var bookingDetails = await JsonSerializer.DeserializeAsync<BookingDetails>(responseStream);
			Console.WriteLine(bookingDetails.ToString());
			var bookingStatus = new BookingStatus
			{
                Id = id,
				clientId=bookingDetails.id,
				status=bookingDetails.status,
			};

			return bookingStatus;
		}
	}
}
