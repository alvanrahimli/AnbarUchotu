using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnbarUchotu.Data;
using AnbarUchotu.Models;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Repos.Products;
using AnbarUchotu.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AnbarUchotu.Repos.Transactions
{
    public class TransactionsRepo : ITransactionsRepo
    {
        private readonly AnbarUchotuDbContext _context;
        private readonly IProductsRepo _productsRepo;

        public TransactionsRepo(AnbarUchotuDbContext context, IProductsRepo productsRepo)
        {
            this._context = context;
            this._productsRepo = productsRepo;
        }

        public async Task<(TransactionReturnDto transaction, List<ProductReturnDto> unableProducts)> Create(string uGuid, List<ProductBuyDto> wantedProducts)
        {
            // Create transaction:
            var transaction = new Transaction()
            {
                Guid = Guid.NewGuid().ToString(),
                IssueDate = DateTime.Now,
                IssuerGuid = uGuid,
                Status = TransactionStatus.Pending,
                ApprovalDate = null,
                CancellationDate = null
            };

            // Create SoldProducts(Content field of Transaction) & calc total amount:
            decimal amount = 0;
            var content = new List<SoldProduct>();
            // variable to store products which user couln't buy:
            var unableToBuyProducts = new List<ProductReturnDto>();

            for (int i = 0; i < wantedProducts.Count; i++)
            {
                // get product with wantedProducts' Guid field:
                var product = await _context.Products
                    .Select(p => new
                    {
                        StockCount = p.Count,
                        SoldProduct = new SoldProduct()
                        {
                            Guid = Guid.NewGuid().ToString(),
                            ProductGuid = p.Guid,
                            SoldCount = wantedProducts[i].Count,
                            TransactionGuid = transaction.Guid
                        }
                    })
                    .FirstOrDefaultAsync(
                        p => p.SoldProduct.ProductGuid == wantedProducts[i].Guid
                        && p.StockCount >= wantedProducts[i].Count
                    );

                if (product != null)
                {
                    // If product is found add it to transaction content
                    // and add price to amount:
                    var p = await _context.Products
                        .FirstOrDefaultAsync(p => p.Guid == wantedProducts[i].Guid);
                    amount += p.Price * product.SoldProduct.SoldCount;
                    content.Add(product.SoldProduct);
                }
                else
                {
                    // To return products which user couldn't buy add them to 'unableToBuyProducts' list:
                    var unableProduct = await _productsRepo
                        .Product(wantedProducts[i].Guid);
                    unableToBuyProducts.Add(unableProduct);
                }
            }
            transaction.Content = content;
            transaction.Amount = amount;

            await _context.Transactions.AddAsync(transaction);
            var result = await _context.SaveChangesAsync();

            // Return transaction:
            if (result > 0)
            {
                var transactionReturn = await GetTransaction(transaction.Guid);
                return (transactionReturn, unableToBuyProducts);
            }
            return (null, null);
        }

        public async Task<TransactionReturnDto> Sign(string tGuid)
        {
            // Update transaction:
            var transaction = await _context.Transactions
                .Include(t => t.Content)
                .FirstOrDefaultAsync(t => t.Guid == tGuid);

            if (transaction == null)
            {
                return null;
            }

            transaction.Status = TransactionStatus.Signed;
            transaction.ApprovalDate = DateTime.Now;

            // Update products (reduce count):
            var content = transaction.Content.ToList();
            for (int i = 0; i < content.Count; i++)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Guid == content[i].ProductGuid);

                if (product != null)
                {
                    product.Count -= content[i].SoldCount;
                }
            }
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var t = await GetTransaction(tGuid);
                return t;
            }
            return null;
        }

        public async Task<TransactionReturnDto> CancelTransaction(string tGuid)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Guid == tGuid && t.Status == TransactionStatus.Pending);

            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Cancelled;
                transaction.CancellationDate = DateTime.Now;
            }
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                var t = await GetTransaction(tGuid);
                return t;
            }
            return null;
        }

        public async Task<TransactionReturnDto> GetTransaction(string guid)
        {
            var transaction = await _context.Transactions
                .AsNoTracking()
                .Select(t => new TransactionReturnDto()
                {
                    Guid = t.Guid,
                    IssueDate = t.IssueDate,
                    ApprovalDate = t.ApprovalDate,
                    CancellationDate = t.CancellationDate,
                    Status = t.Status,
                    IssuerGuid = t.IssuerGuid,
                    Amount = t.Amount
                })
                .FirstOrDefaultAsync(t => t.Guid == guid);

            var contentReturn = await _context.SoldProducts
                .AsNoTracking()
                .Include(p => p.Product)
                .Where(p => p.TransactionGuid == guid)
                .Select(p => new SoldProductReturnDto()
                {
                    Guid = p.Guid,
                    Name = p.Product.Name,
                    Description = p.Product.Description,
                    Barcode = p.Product.Barcode,
                    Count = p.Product.Count,
                    Mass = p.Product.Mass,
                    Price = p.Product.Price,
                    SoldCount = p.SoldCount
                })
                .ToListAsync();

            transaction.Content = contentReturn;
            return transaction;
        }

        public async Task<List<TransactionReturnDto>> GetTransactions(int rn, int c)
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Select(t => new TransactionReturnDto()
                {
                    Guid = t.Guid,
                    IssuerGuid = t.IssuerGuid,
                    IssueDate = t.IssueDate,
                    ApprovalDate = t.ApprovalDate,
                    CancellationDate = t.CancellationDate,
                    Status = t.Status,
                    Amount = t.Amount
                })
                .OrderByDescending(t => t.IssueDate)
                .Skip((rn - 1) * c)
                .Take(c)
                .ToListAsync();

            return transactions;
        }

        public async Task<List<TransactionReturnDto>> GetTransactionsByStatus(TransactionStatus status)
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Status == status)
                .Select(t => new TransactionReturnDto()
                {
                    Guid = t.Guid,
                    IssueDate = t.IssueDate,
                    ApprovalDate = t.ApprovalDate,
                    CancellationDate = t.CancellationDate,
                    Status = t.Status,
                    IssuerGuid = t.IssuerGuid,
                    Amount = t.Amount
                })
                .OrderByDescending(t => t.IssueDate)
                .ToListAsync();

            return transactions;
        }

        public async Task<List<TransactionReturnDto>> SignAll()
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.Status == TransactionStatus.Pending)
                .ToListAsync();

            if (transactions != null)
            {
                foreach (var t in transactions)
                {
                    var result = await Sign(t.Guid);
                }

                var transactionsToReturn = transactions
                    .Select(t => new TransactionReturnDto()
                    {
                        Guid = t.Guid,
                        Amount = t.Amount,
                        ApprovalDate = t.ApprovalDate,
                        CancellationDate = t.CancellationDate,
                        IssueDate = t.IssueDate,
                        IssuerGuid = t.IssuerGuid,
                        Status = t.Status
                    })
                    .OrderByDescending(t => t.IssueDate)
                    .ToList();
                return transactionsToReturn;
            }
            return null;
        }

        public async Task<List<TransactionReturnDto>> GetForUser(string guid)
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.IssuerGuid == guid)
                .Select(t => new TransactionReturnDto()
                {
                    Guid = t.Guid,
                    Amount = t.Amount,
                    ApprovalDate = t.ApprovalDate,
                    CancellationDate = t.CancellationDate,
                    IssueDate = t.IssueDate,
                    IssuerGuid = t.IssuerGuid,
                    Status = t.Status
                })
                .ToListAsync();

            return transactions;
        }
    }
}