using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

public partial class AddPriceAtPurchase : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "price_at_purchase",
            table: "tickets",
            type: "decimal(6,2)",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "price_at_purchase",
            table: "tickets");
    }
}
