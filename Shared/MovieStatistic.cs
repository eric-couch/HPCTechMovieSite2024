using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared;

public class MovieStatistic : OMDBMovie
{
    public double AverageRating { get; set; }
    public int NumberOfRatings { get; set; }
}
