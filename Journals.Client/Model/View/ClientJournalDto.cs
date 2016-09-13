using System;
using System.Collections.Generic;
using journals.commons.Model.Entities;
using NHibernate.Mapping.Attributes;

namespace Journals.Client.Model.View {


    [Class(0,Table = "Journal")]
    
    public class ClientJournalDto : IBaseEntity {

        public const string ByUser = "from ClientJournalDto order by Name asc";


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


        [Property(Formula = "(select count(p.id) from publication p where p.journal_id = id)")]
        public virtual int PublicationCount {
            get; set;
        }



    }
}