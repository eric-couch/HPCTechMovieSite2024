﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared;

public class UserDto
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public List<Movie> FavoriteMovies { get; set; } = new();
    public List<OMDBMovie> OMDBMovies { get; set; } = new();
}
