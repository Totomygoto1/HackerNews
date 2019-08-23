using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HackerNews
{
    public class Story
    {
        public string title { get; set; }
        public string url { get; set; }
        public string by { get; set; }
        public int score { get; set; }
        public int descendants { get; set; }
    }
}
