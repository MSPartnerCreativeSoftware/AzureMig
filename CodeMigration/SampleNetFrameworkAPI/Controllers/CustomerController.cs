using SampleNetFrameworkAPI.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace SampleNetFrameworkAPI.Controllers
{
    public class CustomerController : ApiController
    {
        private List<Customer> _customers = new List<Customer> { new Customer { Id = 1, Name = "John" }, new Customer { Id = 2, Name = "Doe" } };

        // GET api/values
        public IHttpActionResult GetCustomers()
        {
            return Json(_customers);
        }

        // GET api/values/5
        public IHttpActionResult GetCustomerById(int id)
        {

            return Json(_customers.FindAll(customer => customer.Id == id));
        }

        //// POST api/values
        //public void Post([FromBody] Customer value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody] string value)
        //{
            
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}
    }
}
