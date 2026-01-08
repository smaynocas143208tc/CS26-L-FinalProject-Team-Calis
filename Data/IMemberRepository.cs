using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library_Management_System.Data
{
    public interface IMemberRepository
    {
       
        void AddMember(Member member, string hashedPassword);

      
        Member GetMemberByID(int id);
        Member GetMemberByUsername(string username);

    
        DataTable GetAllMembers();
        string GetNextMemberID();

    
        void UpdateMember(Member member, string hashedPassword);


        void RemoveMember(int id);





    }
}

