
using Microsoft.AspNetCore.Mvc;
using CustomADODataService.Models;
using CustomADODataService.Repository;

namespace CustomADODataService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderRepository _repository;

        public OrdersController(OrderRepository repository)
        {
            _repository = repository;
        }

        // POST: api/orders
        [HttpPost]
        public IActionResult CreateOrder(Order order)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int orderId = _repository.CreateOrder(order);

                return StatusCode(201, new
                {
                    Message = "Order created successfully",
                    OrderId = orderId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
