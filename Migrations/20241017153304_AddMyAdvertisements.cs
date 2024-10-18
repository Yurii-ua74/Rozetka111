using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rozetka.Migrations
{
    /// <inheritdoc />
    public partial class AddMyAdvertisements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.CreateTable(
                name: "MyAdvertisements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyAdvertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MyAdvertisements_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MyAdvertisements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
         

            migrationBuilder.CreateIndex(
                name: "IX_MyAdvertisements_ProductId",
                table: "MyAdvertisements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_MyAdvertisements_UserId",
                table: "MyAdvertisements",
                column: "UserId");
           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropTable(
                name: "MyAdvertisements");

            
        }
    }
}
