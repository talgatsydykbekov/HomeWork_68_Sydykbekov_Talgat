using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeWork_66_Sydykbekov_Talgat.Migrations
{
    /// <inheritdoc />
    public partial class AddREsumeContacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Resumes",
                type: "character varying(254)",
                maxLength: 254,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Resumes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkedInUrl",
                table: "Resumes",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Resumes",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telegram",
                table: "Resumes",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "LinkedInUrl",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "Telegram",
                table: "Resumes");
        }
    }
}
