using System;
using System.Windows.Forms;
using journals.commons.Model.dao;
using journals.commons.SimpleInjector;
using Journals.Client.Model;

namespace Journals.Client {
    public partial class LoginForm : Form, ISingletonComponent {

        private readonly IGenericDao _dao;
        private readonly JournalForm _journalsForm;

        private ClientLoginManager _clientLoginManager;

        private readonly JournalViewModel _jvm;

        public LoginForm(IGenericDao dao, JournalForm journalsForm, JournalViewModel jvm, ClientLoginManager clientLoginManager) {
            _dao = dao;
            _journalsForm = journalsForm;
            _jvm = jvm;
            _clientLoginManager = clientLoginManager;
            InitializeComponent();


        }




        private async void button1_Click(object sender, EventArgs e) {

            var serverUrl = serverUrlBox.Text;

            var userName = loginBox.Text;
            var password = passBox.Text;

            Cursor.Current = Cursors.WaitCursor;

            try {
                var result = await _clientLoginManager.Login(userName, password, serverUrl);
                if (!result)
                    return;

                Cursor.Current = Cursors.Default;

                Hide();

                _journalsForm.Show();
            } catch (Exception ex) {
                MessageBox.Show("The user name or password is incorrect.");
            }







        }



        private async void LoginForm_Load(object sender, EventArgs e) {
            var config = await _dao.FindSingleByQuery<ClientConfiguration>("from ClientConfiguration");
            if (config != null) {
                serverUrlBox.Text = config.Url;
                //caching
                _jvm.Config = config;
            }


        }
    }
}
