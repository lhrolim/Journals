using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using journals.commons.Model.Entities;

namespace journals.commons.Model.Dto {
    public class SubscriptionDataDto {

        public ICollection<Journal> NewJournals {
            get; set;
        }

        public ICollection<Publication> NewPublications {
            get; set;
        }
    }
}
