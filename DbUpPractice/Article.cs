using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbUpPractice
{
    public class Article : Entity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PostingDate { get; set; } = DateTime.Now;
    }
}
