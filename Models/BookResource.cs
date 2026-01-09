using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class BookResource
    {
        public int BookId { get; set; }
        public string ResourceType { get; set; }
        public string BookTitle { get; set; }
        public string Author { get; set; }
        public int NumberOfPages { get; set; }
        public string PublishedDate { get; set; }
    }
}
