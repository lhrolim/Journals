using System;
using journals.commons.Model.Entities;
using NHibernate.Mapping.Attributes;

namespace Journals.Client.Model {


    [Class(Table = "CONFIGURATION")]
    public class ClientConfiguration :ABaseEntity{
        [Property]
        public virtual string Url {get; set; }
        [Property]
        public virtual string Token {get; set;}

        [Property]
        public virtual DateTime? SyncDate { get; set; }
    }
}
