using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {

    [Class(Table = "JOURNAL", Lazy = false)]
    public class Journal : ABaseEntity
    {

        public const string ByIdsAndDate = "from Journal where Id in (:p0) and RegisterDate > :p1";

        [Property]
        public virtual string Name {
            get; set;
        }

        [Property]
        public virtual string Description {
            get; set;
        }


        [Property]
        public virtual DateTime? RegisterDate {
            get; set;
        }

        public override void OnSave() {
            if (RegisterDate == null) {
                RegisterDate = DateTime.Now;
            }
        }


        [Set(0, Inverse = true, Lazy = CollectionLazy.True)]
        [Key(1, Column = "journal_id")]
        [OneToMany(2, ClassType = typeof(Journal))]
        public virtual ISet<Journal> Subscriptions {
            get; set;
        }

        [Set(0, Inverse = true, Lazy = CollectionLazy.True)]
        [Key(1, Column = "journal_id")]
        [OneToMany(2, ClassType = typeof(Publication))]
        public virtual ISet<Publication> Publications {
            get; set;
        }


    }
}
