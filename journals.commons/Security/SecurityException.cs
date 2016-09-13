using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using journals.commons.Model.Dao;

namespace journals.commons.Security {
    public class RegistrationException : Exception {

        public RegistrationException(string message) : base(message) {

        }

        public RegistrationException(ModelValidationException e) : this(e.Message) {
        }
    }
}
