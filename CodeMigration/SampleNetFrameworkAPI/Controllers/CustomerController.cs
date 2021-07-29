using SampleNetFrameworkAPI.Models;
using System.Collections.Generic;
using System.Web.Http;

namespace SampleNetFrameworkAPI.Controllers
{
    //.NET5 VERSION
    //[ApiController]
    //public class CustomerController : ControllerBase
    public class CustomerController : ApiController
    {
        private List<Customer> _customers = new List<Customer> { new Customer { Id = 1, Name = "John" }, new Customer { Id = 2, Name = "Doe" } };


        public IHttpActionResult GetCustomers()
        {
            return Json(_customers);
        }


        public IHttpActionResult GetCustomerById(int id)
        {

            return Json(_customers.FindAll(customer => customer.Id == id));
        }

        ////.NET5 VERSION
        //[HttpGet]
        //[Route("api/Customers")]
        //// GET api/values
        //public IActionResult GetCustomers()
        //{
        //    return Ok(_customers.ToArray());
        //}

        //[HttpGet]
        //[Route("api/Customer/{id}")]
        //// GET api/values/5
        //public IActionResult GetCustomerById(int id)
        //{

        //    return Ok(_customers.FindAll(customer => customer.Id == id).ToArray());
        //}
    }
}
