using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SastreriaApi.GenericRepository;
using SastreriaApi.Data;
using SastreriaApi.Models;
using System.Net;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;

namespace SastreriaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        
        private IGenericRepository<Order> _repositoryorder;
        private IGenericRepository<Client> _repositoryclient;

        public OrdersController(IGenericRepository<Order> orderrepostiroy, IGenericRepository<Client> clientrepository)
        {
            _repositoryorder = orderrepostiroy;
            _repositoryclient = clientrepository;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            try
            {
                return await _repositoryorder.GetAll().ToListAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        // GET: api/Orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public async Task<ActionResult<OrderDetailsViewModel>> GetOrder(Guid id)
        {

        
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var order = await _repositoryorder.GetAll().Include(order => order.Client).Include(order => order.Payments).FirstOrDefaultAsync(m => m.Id == id);
                if (order == null)
                {
                    return NotFound();
                }
                OrderDetailsViewModel otv = new OrderDetailsViewModel
                {
                    Id = order.Id,
                    ClientId = order.Client.Id,
                    Name = order.Client.Name,
                    Address = order.Client.Address,
                    Tel = order.Client.Tel,
                    DeliveryDate = order.DeliveryDate,
                    Cost = order.Cost,
                    Details = order.Details,
                    Complete = order.Complete,
                    Amount = order.Payments.Sum(P => P.Amount),
                    Payments = order.Payments,
                };

                return Ok(otv);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            //var order = await _db.Orders
            //    .FirstOrDefaultAsync(m => m.Id == id);
            
            
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<ActionResult<Order>> PutOrder(Guid id, Order order)
        {
            var updateorder = await _repositoryorder.GetAll().Include(order => order.Client).Include(order => order.Payments).FirstOrDefaultAsync(m => m.Id == id);
            if(updateorder != null)
            {
                updateorder.DeliveryDate = order.DeliveryDate;  
                updateorder.Complete = order.Complete;
                updateorder.Cost = order.Cost;
                updateorder.Details = order.Details;
                _repositoryorder.Update(updateorder);
                _repositoryorder.Save();
                return updateorder;
            }
            return null;
            //_repositoryorder.Update(order);
            //    _repositoryorder.Save();
      
        }

        // POST: api/Orders
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<OrderCreateViewModel> PostOrder([FromBody]OrderCreateViewModel orderCreateViewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    var client = new Client
                    {

                        Id = orderCreateViewModel.ClientId ?? Guid.Empty,
                        Name = orderCreateViewModel.Name,
                        Address = orderCreateViewModel.Address,
                        Tel = orderCreateViewModel.Tel
                    };

                    var payment = new Payment
                    {
                        Id = Guid.NewGuid(),
                        Amount = orderCreateViewModel.Amount,
                        Date = DateTime.Now

                    };

                    var order = new Order
                    {
                        Id = Guid.NewGuid(),
                        Client = client,
                        DeliveryDate = orderCreateViewModel.DeliveryDate,
                        Cost = orderCreateViewModel.Cost,
                        Details = orderCreateViewModel.Details,
                        Complete = orderCreateViewModel.Complete
                    };

                    order.Payments.Add(payment);
                    //_repositoryclient.
                    _repositoryorder.Insert(order);


                    //if (orderCreateViewModel.ClientId != null)
                    //{
                    //    _repositoryclient.Update(client);
                    //    //_repositoryorder.Entry(client).State = EntityState.Modified;
                    //}
                    //else
                    //{
                    //    _repositoryclient.Insert(client);
                    //}

                    //await _repositoryorder.SaveChangesAsync();
                    _repositoryorder.Save();
                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);

                    


                }
                return new JsonResult(orderCreateViewModel);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }



            
            
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(Guid id)
        {

            //var order = await _repositoryorder.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            var order = await _repositoryorder.GetAll().Include(order => order.Client).Include(order => order.Payments).FirstOrDefaultAsync(m => m.Id == id);

            if (order != null)
            {
                _repositoryorder.Delete(order);
                _repositoryorder.Save();
            }


            return order;
        }

        private bool OrderExists(Guid id)
        {
            return _repositoryorder.GetAll().Any(e => e.Id == id);
        }
    }

 
}
