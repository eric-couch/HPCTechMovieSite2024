using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPCTechMovieSite2024.Server.Migrations
{
    /// <inheritdoc />
    public partial class userRatingAndReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "userRating",
                table: "Movies",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "userReview",
                table: "Movies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "userRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "userReview",
                table: "Movies");
        }
    }
}
