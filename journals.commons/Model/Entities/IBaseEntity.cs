using System.Collections.Generic;

namespace journals.commons.Model.Entities {
    public interface IBaseEntity {
        int? Id {
            get; set;
        }

        IEnumerable<string> Validate();

        void OnSave();
    }
}