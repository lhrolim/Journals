using System;
using System.Collections.Generic;
using System.Text;
using journals.commons.Model.Entities;

namespace journals.commons.Model.Dao {
    public class ModelValidationException : Exception {

        public ModelValidationException(string message) : base(message){
            
        }

        public static ModelValidationException Instance(IBaseEntity entity, IEnumerable<string> errors) {
            var sb = new StringBuilder();
            sb.AppendFormat("error validating entity {0}. Errors: {1}", entity.GetType().Name, string.Join(",", errors));
            return new ModelValidationException(sb.ToString());
        }
    }
}
