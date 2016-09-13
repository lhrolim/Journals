using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Mapping.Attributes;

namespace journals.commons.Model.Entities {


    [Component]
    public class JournalDefinition {

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

    }
}
