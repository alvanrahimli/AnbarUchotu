using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnbarUchotu.Migrations
{
    public partial class i1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankCards",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    CardNumber = table.Column<string>(nullable: false),
                    Balance = table.Column<decimal>(nullable: false),
                    TotalIncome = table.Column<decimal>(nullable: false),
                    TotalExpense = table.Column<decimal>(nullable: false),
                    TotalCancelled = table.Column<decimal>(nullable: false),
                    IssuedTransactionsCount = table.Column<int>(nullable: false),
                    AcceptedTransactionsCount = table.Column<int>(nullable: false),
                    OwnerGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankCards", x => x.Guid);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    IssuerGuid = table.Column<string>(nullable: true),
                    AcceptorGuid = table.Column<string>(nullable: true),
                    Amount = table.Column<decimal>(nullable: false),
                    IssueDate = table.Column<DateTime>(nullable: false),
                    ApprovalDate = table.Column<DateTime>(nullable: true),
                    CancellationDate = table.Column<DateTime>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Transactions_BankCards_AcceptorGuid",
                        column: x => x.AcceptorGuid,
                        principalTable: "BankCards",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transactions_BankCards_IssuerGuid",
                        column: x => x.IssuerGuid,
                        principalTable: "BankCards",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    Username = table.Column<string>(maxLength: 50, nullable: false),
                    Email = table.Column<string>(nullable: false),
                    NormalizedUsername = table.Column<string>(maxLength: 50, nullable: false),
                    NormalizedEmail = table.Column<string>(nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: false),
                    CardGuid = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Users_BankCards_CardGuid",
                        column: x => x.CardGuid,
                        principalTable: "BankCards",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    Barcode = table.Column<string>(nullable: false),
                    Mass = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    OwnerGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_Products_Users_OwnerGuid",
                        column: x => x.OwnerGuid,
                        principalTable: "Users",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SoldProducts",
                columns: table => new
                {
                    Guid = table.Column<string>(nullable: false),
                    ProductGuid = table.Column<string>(nullable: true),
                    SoldCount = table.Column<int>(nullable: false),
                    TransactionGuid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldProducts", x => x.Guid);
                    table.ForeignKey(
                        name: "FK_SoldProducts_Products_ProductGuid",
                        column: x => x.ProductGuid,
                        principalTable: "Products",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SoldProducts_Transactions_TransactionGuid",
                        column: x => x.TransactionGuid,
                        principalTable: "Transactions",
                        principalColumn: "Guid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_OwnerGuid",
                table: "Products",
                column: "OwnerGuid");

            migrationBuilder.CreateIndex(
                name: "IX_SoldProducts_ProductGuid",
                table: "SoldProducts",
                column: "ProductGuid");

            migrationBuilder.CreateIndex(
                name: "IX_SoldProducts_TransactionGuid",
                table: "SoldProducts",
                column: "TransactionGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AcceptorGuid",
                table: "Transactions",
                column: "AcceptorGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IssuerGuid",
                table: "Transactions",
                column: "IssuerGuid");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CardGuid",
                table: "Users",
                column: "CardGuid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoldProducts");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BankCards");
        }
    }
}
