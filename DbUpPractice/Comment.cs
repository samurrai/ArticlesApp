using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbUpPractice
{
    public class Comment : Entity
    {
        public string AuthorName { get; set; }
        public string Text { get; set; }
        public DateTime CommentDate { get; set; } = DateTime.Now;
        public Article Article { get; set; }
        public Guid ArticleId { get; set; }
    }
}