using System.Collections.Generic;
using System.Threading.Tasks;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Repos.Transactions;
using AnbarUchotu.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AnbarUchotu.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepo _repo;

        public TransactionsController(ITransactionsRepo repo)
        {
            this._repo = repo;
        }

        [HttpGet("{guid:guid}")]
        public async Task<IActionResult> GetTransaction(string guid)
        {
            var transaction = await _repo.GetTransaction(guid);

            if (transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }

        [HttpGet("all")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetTransactions(int rn, int c)
        {
            var transaction = await _repo.GetTransactions(rn, c);

            if (transaction != null)
            {
                return Ok(transaction);
            }
            return NotFound();
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]List<ProductBuyDto> products)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string guid = Helper.GetCurrentUserGuid(HttpContext);
            var result = await _repo.Create(guid, products);

            if (result.transaction == null && result.unableProducts == null)
            {
                return StatusCode(500, "Operation failed. Please try again.");
            }

            return Ok(new
            {
                transaction = result.transaction,
                unavaliableProducts = result.unableProducts
            });
        }

        [HttpPost("sign/{guid:guid}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> Sign(string guid)
        {
            var transaction = await _repo.Sign(guid);

            if (transaction != null)
            {
                return Ok(transaction);
            }
            return StatusCode(500, "Operation failed, try again.");
        }

        [HttpPost("sign/all")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> SignAll()
        {
            var transactions = await _repo.SignAll();

            if (transactions != null)
            {
                return Ok(transactions);
            }
            return StatusCode(500, "Operation failed, try again.");
        }

        [HttpPost("cancel/{guid:guid}")]
        public async Task<IActionResult> Cancel(string guid)
        {
            var transaction = await _repo.CancelTransaction(guid);

            if (transaction != null)
            {
                return Ok(transaction);
            }
            return StatusCode(500, "Operation failed, try again.");
        }

        [HttpGet("{status}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            List<TransactionReturnDto> transactions = new List<TransactionReturnDto>();

            switch (status)
            {
                case "signed":
                    transactions = await _repo.GetTransactionsByStatus(TransactionStatus.Signed);
                    break;
                case "pending":
                    transactions = await _repo.GetTransactionsByStatus(TransactionStatus.Pending);
                    break;
                case "cancelled":
                    transactions = await _repo.GetTransactionsByStatus(TransactionStatus.Cancelled);
                    break;
                default:
                    return BadRequest("Status must be 'signed', 'pending' or 'cancelled'.");
            }

            if (transactions.Count > 0)
            {
                return Ok(transactions);
            }
            return NotFound();
        }

        [HttpGet("user")]
        [Authorize(Roles = "Buyer")]
        public async Task<IActionResult> GetForUser()
        {
            var user = Helper.GetCurrentUserGuid(HttpContext);
            var transactions = await _repo.GetForUser(user);
            if (transactions != null)
            {
                return Ok(transactions);
            }
            return NotFound();
        }
    }
}