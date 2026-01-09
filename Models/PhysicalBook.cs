using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class PhysicalBook : BookResource
    {
        public string ISBN { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
        public string CopyID { get; set; }

        public List<BookCopy> Copies { get; set; } = new List<BookCopy>();

        public void CheckOut(string copyId)
        {
            var copy = Copies.Find(c => c.CopyId == copyId);
            if (copy != null && copy.Status == "Available")
            {
                copy.SetAvailable(false);
            }
        }

        public void CheckIn(string copyId, string newCondition)
        {
            var copy = Copies.Find(c => c.CopyId == copyId);
            if (copy != null)
            {
                copy.SetAvailable(true);
                copy.UpdateCondition(newCondition);
            }
        }

    }
}
