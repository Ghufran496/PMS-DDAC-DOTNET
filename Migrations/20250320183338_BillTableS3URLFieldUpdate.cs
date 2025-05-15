using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMS_DDAC.Migrations
{
    /// <inheritdoc />
    public partial class BillTableS3URLFieldUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiptUrl",
                table: "Bills",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptUrl",
                table: "Bills");
        }
    }
}
