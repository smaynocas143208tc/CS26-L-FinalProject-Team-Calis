using BCrypt.Net;
using Library_Management_System.Data;
using Library_Management_System.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;



namespace Library_Management_System.BusinessLogic 
{
    public class MemberManager
    {
        private readonly IMemberRepository _memberRepository;


        public MemberManager(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }


        public void RegisterMember(Member member, string plainPassword)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

  
            _memberRepository.AddMember(member, hashedPassword);
        }

 
        public Member VerifyLogin(string username, string plainPassword)
        {
 
            Member member = _memberRepository.GetMemberByUsername(username);

            if (member != null)
            {
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(plainPassword, member.PasswordHash);

                if (isPasswordValid)
                {
                    return member; 
                }
            }
            return null; 
        }

 
        public DataTable GetRegisteredMembers()
        {
            return _memberRepository.GetAllMembers();
        }

 
        public string GetNextMemberID()
        {
            return _memberRepository.GetNextMemberID();
        }


        public Member VerifyMember(int id)
        {
            return _memberRepository.GetMemberByID(id);
        }


        public void UpdateMember(Member member, string newPlainPassword)
        {
            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(newPlainPassword);
            _memberRepository.UpdateMember(member, newHashedPassword);
        }


        public void RemoveMember(int id)
        {
            _memberRepository.RemoveMember(id);
        }



        public Member GetMemberByID(int id)
        {
            return _memberRepository.GetMemberByID(id);
        }



        





    }
}