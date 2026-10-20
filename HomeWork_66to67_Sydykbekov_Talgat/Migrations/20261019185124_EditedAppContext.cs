using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeWork_66_Sydykbekov_Talgat.Migrations
{
    /// <inheritdoc />
    public partial class EditedAppContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resume_AspNetUsers_ApplicantId",
                table: "Resume");

            migrationBuilder.DropForeignKey(
                name: "FK_Resume_Categories_CategoryId",
                table: "Resume");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resume",
                table: "Resume");

            migrationBuilder.RenameTable(
                name: "Resume",
                newName: "Resumes");

            migrationBuilder.RenameIndex(
                name: "IX_Resume_CategoryId",
                table: "Resumes",
                newName: "IX_Resumes_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Resume_ApplicantId",
                table: "Resumes",
                newName: "IX_Resumes_ApplicantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resumes",
                table: "Resumes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_AspNetUsers_ApplicantId",
                table: "Resumes",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_Categories_CategoryId",
                table: "Resumes",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_AspNetUsers_ApplicantId",
                table: "Resumes");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_Categories_CategoryId",
                table: "Resumes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Resumes",
                table: "Resumes");

            migrationBuilder.RenameTable(
                name: "Resumes",
                newName: "Resume");

            migrationBuilder.RenameIndex(
                name: "IX_Resumes_CategoryId",
                table: "Resume",
                newName: "IX_Resume_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Resumes_ApplicantId",
                table: "Resume",
                newName: "IX_Resume_ApplicantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Resume",
                table: "Resume",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resume_AspNetUsers_ApplicantId",
                table: "Resume",
                column: "ApplicantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Resume_Categories_CategoryId",
                table: "Resume",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
