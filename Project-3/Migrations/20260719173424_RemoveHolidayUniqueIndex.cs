using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_3.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHolidayUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Holidays_Date_CountryCode",
                table: "Holidays");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holidays",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Holidays",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.CreateIndex(
                name: "IX_Holidays_Date_CountryCode",
                table: "Holidays",
                columns: new[] { "Date", "CountryCode" },
                unique: true);
        }
    }
}
