﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HPCTechMovieSite2024.Server.Migrations
{
    /// <inheritdoc />
    public partial class RatingsAndOMDBMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OMDBMovies",
                columns: table => new
                {
                    imdbID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Released = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Runtime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Writer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Awards = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poster = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Metascore = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imdbRating = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    imdbVotes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DVD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoxOffice = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Production = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OMDBMovies", x => x.imdbID);
                });

            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OMDBMovieID = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_OMDBMovies_OMDBMovieID",
                        column: x => x.OMDBMovieID,
                        principalTable: "OMDBMovies",
                        principalColumn: "imdbID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rating_OMDBMovieID",
                table: "Rating",
                column: "OMDBMovieID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rating");

            migrationBuilder.DropTable(
                name: "OMDBMovies");
        }
    }
}
