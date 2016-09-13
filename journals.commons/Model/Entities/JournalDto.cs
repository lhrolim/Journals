using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {


    [Class(0,Table = "Journal")]
    
    public class JournalDto : IBaseEntity {

        public const string ByUser = "from JournalDto order by SubscriptionDate desc";


        [Id(0, Name = "Id")]
        [Generator(1, Class = "assigned")]
        public virtual int? Id {
            get; set;
        }

        public virtual IEnumerable<string> Validate() {
            yield break;
        }

        public virtual void OnSave() {
        }

        [Property ]
        public virtual string Name {
            get; set;
        }
        [Property]
        public virtual string Description {
            get; set;
        }

        [Property(0,Formula = "(select s.subscriptiondate from subscription s where s.journal_id = id and s.user_id = :UserFilter.id)")]
        public virtual DateTime? SubscriptionDate {
            get; set;
        }


        public virtual bool IsSubscribed => SubscriptionDate != null;

        [Property(Formula = "(select count(p.id) from publication p where p.journal_id = id)")]
        public virtual int PublicationCount {
            get; set;
        }



    }
}