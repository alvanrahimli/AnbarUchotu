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

        // To buy product, u must create transaction:
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
                            Guid = p.Guid,
                            ProductGuid = p.Guid,
                            SoldCount = wantedProducts[i].Count,
                            TransactionGuid = transaction.Guid
                        }
                    })
                    .FirstOrDefaultAsync(
                        p => p.SoldProduct.Guid == wantedProducts[i].Guid
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

        public async Task<bool> Sign(string tGuid)
        {
            // Update transaction:
            var transaction = await _context.Transactions
                .Include(t => t.Content)
                .FirstOrDefaultAsync(t => t.Guid == tGuid);

            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Signed;
            }

            // Update products (reduce count):
            var content = transaction.Content.ToList();
            for (int i = 0; i < content.Count; i++)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Guid == content[i].Guid);

                if (product != null)
                {
                    product.Count -= content[i].SoldCount;
                }
            }
            var result = await _context.SaveChangesAsync();
            
            return result > 0 ? true : false;
        }

        public async Task<bool> CancelTransaction(string tGuid)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Guid == tGuid);

            if (transaction != null)
            {
                transaction.Status = TransactionStatus.Cancelled;
            }
            var result = await _context.SaveChangesAsync();

            return result > 0 ? true : false;
        }

        public async Task<TransactionReturnDto> GetTransaction(string guid)
        {
            var transaction = await _context.Transactions
                .Select(t => new TransactionReturnDto()
                {
                    Guid = t.Guid,
                    IssueDate = t.IssueDate,
                    ApprovalDate = t.ApprovalDate,
                    CancellationDate = t.CancellationDate,
                    Status = t.Status,
                    IssuerGuid = t.IssuerGuid
                })
                .FirstOrDefaultAsync(t => t.Guid == guid);

            var contentReturn = await _context.SoldProducts
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
    }
}