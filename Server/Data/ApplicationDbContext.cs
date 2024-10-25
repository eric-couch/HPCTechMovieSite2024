﻿using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using HPCTechMovieSite2024.Server.Models;
using HPCTechMovieSite2024.Shared;

namespace HPCTechMovieSite2024.Server.Data;

public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
{
    public ApplicationDbContext(
        DbContextOptions options,
        IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Rating> Rating => Set<Rating>();
    public DbSet<OMDBMovie> OMDBMovies => Set<OMDBMovie>();
}
