using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyClinic.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintToDoctorServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorServices_DoctorId",
                table: "DoctorServices");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorServices_DoctorId_MedicalServiceId",
                table: "DoctorServices",
                columns: new[] { "DoctorId", "MedicalServiceId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DoctorServices_DoctorId_MedicalServiceId",
                table: "DoctorServices");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorServices_DoctorId",
                table: "DoctorServices",
                column: "DoctorId");
        }
    }
}
