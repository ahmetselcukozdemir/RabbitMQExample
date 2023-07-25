using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQExampleProject.DTO;


namespace RabbitMQExampleProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        public InvoiceController()
        {
            
        }

        [HttpPost]
        public IActionResult PostInvoice([FromBody] InvoiceDTO invoiceaDto)
        {
            return Ok("Fatura başarıyla alındı ve işlendi.");
        }
    }
}
