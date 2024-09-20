using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rozetka.Migrations
{
    /// <inheritdoc />
    public partial class AddSubChildCategoryToProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubChildCategoryId",
                table: "Products",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SubChildCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChildCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubChildCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubChildCategories_Childcategories_ChildCategoryId",
                        column: x => x.ChildCategoryId,
                        principalTable: "Childcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_SubChildCategoryId",
                table: "Products",
                column: "SubChildCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubChildCategories_ChildCategoryId",
                table: "SubChildCategories",
                column: "ChildCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SubChildCategories_SubChildCategoryId",
                table: "Products",
                column: "SubChildCategoryId",
                principalTable: "SubChildCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_SubChildCategories_SubChildCategoryId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "SubChildCategories");

            migrationBuilder.DropIndex(
                name: "IX_Products_SubChildCategoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SubChildCategoryId",
                table: "Products");
        }
    }
}
