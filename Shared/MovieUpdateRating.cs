using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPCTechMovieSite2024.Shared;

public class MovieUpdateRating
{
    public string ImdbId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public double Rating { get; set; }
}
