using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnbarUchotu.Data;
using AnbarUchotu.Models;
using AnbarUchotu.Models.Dtos;
using AnbarUchotu.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AnbarUchotu.Repos.Products
{
    public class ProductsRepo : IProductsRepo
    {
        private readonly AnbarUchotuDbContext _context;

        public ProductsRepo(AnbarUchotuDbContext context)
        {
            this._context = context;
        }

        public async Task<ProductReturnDto> Product(string guid)
        {
            var product = await _context.Products
                .Select(p => new ProductReturnDto()
                {
                    Guid = p.Guid,
                    Barcode = p.Barcode,
                    Count = p.Count,
                    Description = p.Description,
                    Mass = p.Mass,
                    Name = p.Name,
                    Price = p.Price
                })
                .FirstOrDefaultAsync(p => p.Guid == guid);

            return product;
        }

        public async Task<List<ProductReturnDto>> Products(int rn, int c)
        {
            var products = await _context.Products
                .Skip((rn - 1) * c)
                .Take(c)
                .Select(p => new ProductReturnDto()
                {
                    Guid = p.Guid,
                    Barcode = p.Barcode,
                    Count = p.Count,
                    Description = p.Description,
                    Mass = p.Mass,
                    Name = p.Name,
                    Price = p.Price
                })
                .ToListAsync();

            return products;
        }

        public async Task<ProductReturnDto> Register(ProductRegisterDto product)
        {
            var newProduct = new Product()
            {
                Guid = Guid.NewGuid().ToString(),
                Barcode = product.Barcode,
                Name = product.Name,
                Description = product.Description,
                Count = product.Count,
                Mass = product.Mass,
                Price = product.Price
            };

            await _context.Products.AddAsync(newProduct);
            var result = await _context.SaveChangesAsync();
            var returnValue = await Product(newProduct.Guid);
            return returnValue;
        }

        public async Task<ProductReturnDto> Update(ProductUpdateDto product)
        {
            var oldP = await _context
                .Products.FirstOrDefaultAsync(p => p.Guid == product.Guid);

            if (oldP != null)
            {
                oldP.Name = product.Name;
                oldP.Description = product.Description;
                oldP.Barcode = product.Barcode;
                oldP.Count = product.Count;
                oldP.Mass = product.Mass;
                oldP.Price = product.Price;
            }

            await _context.SaveChangesAsync();

            var p = await Product(product.Guid);
            return p;
        }

        public async Task<TransactionReturnDto> Buy(string uGuid, List<ProductBuyDto> wantedProducts)
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
            for (int i = 0; i < wantedProducts.Count; i++)
            {
                var product = await _context.Products
                    .Select(p => new SoldProduct()
                    {
                        Guid = p.Guid,
                        ProductGuid = p.Guid,
                        SoldCount = wantedProducts[i].Count,
                        TransactionGuid = transaction.Guid
                    })
                    .FirstOrDefaultAsync(p => p.Guid == wantedProducts[i].Guid);
                if (product != null)
                {
                    var p = await _context.Products
                        .FirstOrDefaultAsync(p => p.Guid == wantedProducts[i].Guid);
                    amount += p.Price * product.SoldCount;
                    content.Add(product);
                }
            }
            transaction.Content = content;
            transaction.Amount = amount;

            await _context.Transactions.AddAsync(transaction);
            var result = await _context.SaveChangesAsync();

            // Return transaction:
            if (result > 0)
            {
                var transactionReturnDto = await _context.Transactions
                    .Select(t => new TransactionReturnDto()
                    {
                        Guid = t.Guid,
                        IssueDate = t.IssueDate,
                        ApprovalDate = t.ApprovalDate,
                        CancellationDate = t.CancellationDate,
                        Status = t.Status,
                        IssuerGuid = t.IssuerGuid
                    })
                    .FirstOrDefaultAsync(t => t.Guid == transaction.Guid);

                var contentReturn = await _context.SoldProducts
                    .Include(p => p.Product)
                    .Where(p => p.TransactionGuid == transaction.Guid)
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

                transactionReturnDto.Content = contentReturn;
                return transactionReturnDto;
            }
            return null;
        }

        public Task<TransactionReturnDto> Retrieve(string tGuid)
        {
            throw new System.NotImplementedException(); // TODO;
        }
    }
}