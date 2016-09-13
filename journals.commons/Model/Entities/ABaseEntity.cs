using System.Collections.Generic;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {
    public abstract class ABaseEntity : IBaseEntity {

        [Id(0, Name = "Id")]
        [Generator(1, Class = "native")]
        public virtual int? Id {
            get; set;
        }

        public virtual IEnumerable<string> Validate() {
            return null;
        }

        public virtual void OnSave(){
            //NOOP
        }
    }
}
