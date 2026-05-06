using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomerSettlement.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ShortName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LegalAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ActualAddress = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Inn = table.Column<string>(type: "TEXT", maxLength: 12, nullable: false),
                    Kpp = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    Ogrn = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Bank = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Rs = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ks = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Bik = table.Column<string>(type: "TEXT", maxLength: 9, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Position = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DirectorFullName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Category = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nomenclatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nomenclatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractSubject = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OwnerCompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sum = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    PaymentSum = table.Column<decimal>(type: "TEXT", nullable: false),
                    PaymentStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActStatus = table.Column<bool>(type: "INTEGER", nullable: false),
                    ContractId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountHeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountHeaders_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHeaders_Companies_OwnerCompanyId",
                        column: x => x.OwnerCompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountHeaders_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NomenclatureId = table.Column<int>(type: "INTEGER", nullable: true),
                    Quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    VatRate = table.Column<decimal>(type: "TEXT", precision: 5, scale: 2, nullable: false),
                    AmountWithVat = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Period = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AccountHeaderId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountLines_AccountHeaders_AccountHeaderId",
                        column: x => x.AccountHeaderId,
                        principalTable: "AccountHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountLines_Nomenclatures_NomenclatureId",
                        column: x => x.NomenclatureId,
                        principalTable: "Nomenclatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Number = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Sum = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AccountHeaderId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_AccountHeaders_AccountHeaderId",
                        column: x => x.AccountHeaderId,
                        principalTable: "AccountHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "ActualAddress", "Bank", "Bik", "Category", "DirectorFullName", "Inn", "Kpp", "Ks", "LegalAddress", "Name", "Ogrn", "Phone", "Position", "Rs", "ShortName" },
                values: new object[] { -1, "", "", "", null, "", "", "", "", "", "Моя компания", "", "", "", "", "" });

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeaders_CompanyId",
                table: "AccountHeaders",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeaders_ContractId",
                table: "AccountHeaders",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountHeaders_OwnerCompanyId",
                table: "AccountHeaders",
                column: "OwnerCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLines_AccountHeaderId",
                table: "AccountLines",
                column: "AccountHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountLines_NomenclatureId",
                table: "AccountLines",
                column: "NomenclatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_CompanyId",
                table: "Contracts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_AccountHeaderId",
                table: "Payments",
                column: "AccountHeaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLines");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Nomenclatures");

            migrationBuilder.DropTable(
                name: "AccountHeaders");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
