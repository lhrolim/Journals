using journals.commons.SimpleInjector;

namespace Journals.Client.Model {

    public class JournalViewModel : ISingletonComponent {

        public int CurrentJournalId {
            get; set;
        }

        public string UserName {
            get; set;
        }

        public bool ShowingPdf {
            get; set;
        }

        public JournalForm JournalForm {
            get; set;
        }

        public ClientConfiguration Config { get; set; }


    }
}
