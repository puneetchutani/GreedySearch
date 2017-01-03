using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreedyMatch.Models
{
    public class GreedyMatchModel
    {
        [Required]
        public long Fragments { get; set; }

        [Required]
        public string StrText { get; set; }
    }
}