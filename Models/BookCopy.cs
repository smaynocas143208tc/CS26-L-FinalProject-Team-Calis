using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class BookCopy

    {
        public string CopyId { get; set; }     
        public int BookId { get; set; }        
        public string Condition { get; set; }  
        public string Status { get; set; }     



        public void UpdateCondition(string newCondition)
        {
            this.Condition = newCondition;
        }

        public void SetAvailable(bool available)
        {
            this.Status = available ? "Available" : "Borrowed";
        }
    










     }

}










