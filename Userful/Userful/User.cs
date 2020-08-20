using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Userful
{
    public class User
    {
        public string user;
        public string password;

        public User()
        {
            user = "user";
            password = "password";
        }

        public User(string usr, string pw)
        {
            user = usr;
            password = pw;
        }
    }
}