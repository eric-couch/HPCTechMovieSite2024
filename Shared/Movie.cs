using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HPCTechMovieSite2024.Shared;

public class Movie
{
    public int Id { get; set; }
    public string imdbId { get; set; }

    [ForeignKey("ApplicationUserId")]
    public string ApplicationUserId {  get; set; }
}
