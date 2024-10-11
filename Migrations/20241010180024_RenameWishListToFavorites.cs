using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rozetka.Migrations
{
    /// <inheritdoc />
    public partial class RenameWishListToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Переименовываем таблицу WishList в Favorites
            migrationBuilder.RenameTable(
                name: "WishList",
                newName: "Favorites");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Обратное переименование таблицы на случай отката миграции
            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "WishList");
        }
    }
}
