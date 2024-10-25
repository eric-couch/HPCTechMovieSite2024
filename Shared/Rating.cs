using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared;

public class Rating
{
    public int Id { get; set; }
    public string Source { get; set; }
    public string Value { get; set; }

    public string OMDBMovieID { get; set; }
}
