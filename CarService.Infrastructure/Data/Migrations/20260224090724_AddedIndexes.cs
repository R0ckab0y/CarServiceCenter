using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarService.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_users_PhoneNumber",
                table: "users",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Role",
                table: "users",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_jobCards_CreatedDate",
                table: "jobCards",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_jobCards_Status",
                table: "jobCards",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_jobCards_VehicleId_CreatedDate",
                table: "jobCards",
                columns: new[] { "VehicleId", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_PhoneNumber",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_Role",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_jobCards_CreatedDate",
                table: "jobCards");

            migrationBuilder.DropIndex(
                name: "IX_jobCards_Status",
                table: "jobCards");

            migrationBuilder.DropIndex(
                name: "IX_jobCards_VehicleId_CreatedDate",
                table: "jobCards");
        }
    }
}
