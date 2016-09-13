using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace journals.commons.Security {
    public class UserIdentity : GenericIdentity{
       
        public string Password {
            get; set;
        }
        public string UserName {
            get; set;
        }

        public string Email {
            get; set;
        }
        
        public int UserId {
            get; set;
        }

        public bool Publisher { get; set; }

        /// <summary>
        /// Basic Authentication Identity Constructor
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public UserIdentity(string userName, string password)
            : base(userName, "Basic") {
            Password = password;
            UserName = userName;
        }
    }
}
