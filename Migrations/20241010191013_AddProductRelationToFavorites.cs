using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rozetka.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRelationToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdProduct",
                table: "Favorites",
                newName: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_ProductId",
                table: "Favorites",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Products_ProductId",
                table: "Favorites");

            migrationBuilder.DropIndex(
                name: "IX_Favorites_ProductId",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Favorites",
                newName: "IdProduct");
        }
    }
}
