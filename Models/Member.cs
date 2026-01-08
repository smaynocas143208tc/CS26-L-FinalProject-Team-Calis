using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Models
{
    public class Member
    {
        public int MemberID { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public string PasswordHash { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string  Phone { get; set; }
    }
}
