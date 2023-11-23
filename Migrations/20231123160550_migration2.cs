using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persol_Hms.Migrations
{
    public partial class migration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicals_Labs_LabID",
                table: "Medicals");

            migrationBuilder.DropIndex(
                name: "IX_Medicals_LabID",
                table: "Medicals");

            migrationBuilder.DropColumn(
                name: "LabID",
                table: "Medicals");

            migrationBuilder.CreateIndex(
                name: "IX_Symptoms_PatientNo",
                table: "Symptoms",
                column: "PatientNo");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_Patients_PatientNo",
                table: "Symptoms",
                column: "PatientNo",
                principalTable: "Patients",
                principalColumn: "PatientNo",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_Patients_PatientNo",
                table: "Symptoms");

            migrationBuilder.DropIndex(
                name: "IX_Symptoms_PatientNo",
                table: "Symptoms");

            migrationBuilder.AddColumn<int>(
                name: "LabID",
                table: "Medicals",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Medicals_LabID",
                table: "Medicals",
                column: "LabID");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicals_Labs_LabID",
                table: "Medicals",
                column: "LabID",
                principalTable: "Labs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
