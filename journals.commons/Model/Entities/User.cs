using System.ComponentModel.DataAnnotations;
using journals.commons.Security;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {

    [Class(Table = "USER")]
    public class User : ABaseEntity {

        public const string ByUserName = "from User where lower(Login) = lower(?)";


        [Property]
        [Required]
        public virtual string Login {
            get; set;
        }

        [Property]
        [Required]
        public virtual string Password {
            get; set;
        }


        [Required]
        public virtual string ConfirmPassword {
            get; set;
        }


        [Property]
        [Required]
        public virtual string Email {
            get; set;
        }




        /// <summary>
        /// Controls whether or not this user can publish new issues
        /// </summary>
        [Property]
        public virtual bool Publisher {
            get; set;
        }

        public virtual UserIdentity ToIdentity() {
            return new UserIdentity(Login, Password) {
                UserId = Id.Value,
                Email = Email,
                Publisher = Publisher
            };
        }

    }
}
