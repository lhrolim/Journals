using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {

    [Class(Table = "SUBSCRIPTION")]
    public class Subscription : ABaseEntity {

        public const string ByUser = "from Subscription where User.id = ?";


        public override IEnumerable<string> Validate() {
            var errors = new List<string>();
            if (Journal == null) {
                errors.Add("Journal is required");
            }
            if (User== null) {
                errors.Add("User is required");
            }

            return errors;
        }

        public override void OnSave() {
            if (SubscriptionDate == null) {
                SubscriptionDate = DateTime.Now;
            }
        }

        [Property]
        public virtual DateTime? SubscriptionDate {
            get; set;
        }

        [ManyToOne(Column = "journal_id", OuterJoin = OuterJoinStrategy.False, Lazy = Laziness.False, Cascade = "none")]
        public virtual Journal Journal {
            get; set;
        }


        [ManyToOne(Column = "user_id", OuterJoin = OuterJoinStrategy.False, Lazy = Laziness.Proxy, Cascade = "none")]
        public virtual User User {
            get; set;
        }





    }
}
