using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContractMonthlyClaimSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Claims_Lecturers_LecturerId1",
                table: "Claims");

            migrationBuilder.DropIndex(
                name: "IX_Claims_LecturerId1",
                table: "Claims");

            migrationBuilder.DropColumn(
                name: "LecturerId1",
                table: "Claims");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LecturerId1",
                table: "Claims",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_LecturerId1",
                table: "Claims",
                column: "LecturerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Claims_Lecturers_LecturerId1",
                table: "Claims",
                column: "LecturerId1",
                principalTable: "Lecturers",
                principalColumn: "LecturerId");
        }
    }
}
