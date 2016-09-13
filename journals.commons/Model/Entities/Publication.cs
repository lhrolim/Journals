using System;
using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {

    [Class(Table = "PUBLICATION")]
    public class Publication : ABaseEntity
    {

        public const string ByJournalId = "from Publication where JournalId =?";
        public const string ByJournalIdsAndDate = "from Publication where JournalId in (:p0) and RegisterDate > :p1";

        [Property]
        public virtual DateTime? RegisterDate {
            get; set;
        }


        public override void OnSave() {
            if (RegisterDate == null) {
                RegisterDate = DateTime.Now;
            }
        }

        public override IEnumerable<string> Validate()
        {
            var errors = new List<string>();
            if (Title == null) {
                errors.Add("Title is required");
            }

            return errors;
        }


        [Property]
        public virtual string Title { get; set; }

        [Property]
        public virtual string Description {get; set;}

        [Property]
        public virtual int Volume { get; set; }

        [Property]
        public virtual int Issue {get; set;}

        [Property(Column = "journal_id")]
        public virtual int JournalId { get; set; }




    }
}
