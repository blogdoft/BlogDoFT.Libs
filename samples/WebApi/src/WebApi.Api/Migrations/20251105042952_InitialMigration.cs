using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    navigation_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    client_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    client_name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    installments = table.Column<int>(type: "INTEGER", nullable: false),
                    payment_method = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sale_item",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    product_id = table.Column<Guid>(type: "TEXT", nullable: false),
                    product_description = table.Column<string>(type: "TEXT", maxLength: 300, nullable: true),
                    quantity = table.Column<decimal>(type: "TEXT", precision: 18, scale: 3, nullable: false),
                    unit_value = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    discount = table.Column<decimal>(type: "TEXT", precision: 18, scale: 4, nullable: false),
                    sale_id = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sale_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_sale_item__sale_id",
                        column: x => x.sale_id,
                        principalTable: "sale",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_sale_navigation_id",
                table: "sale",
                column: "navigation_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sale_item__product_id",
                table: "sale_item",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_sale_item__sale_id",
                table: "sale_item",
                column: "sale_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sale_item");

            migrationBuilder.DropTable(
                name: "sale");
        }
    }
}
