using System;
using System.Windows.Forms;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using Journals.Client.Model;

namespace Journals.Client {
    public partial class PublicationForm : Form, ISingletonComponent {

        private readonly IGenericDao _dao;
        private readonly JournalViewModel _jvm;
        private PdfLoaderManager _pdfLoaderManager;

        public PublicationForm(IGenericDao dao, JournalViewModel jvm, PdfLoaderManager pdfLoaderManager) {
            _dao = dao;
            _jvm = jvm;
            _pdfLoaderManager = pdfLoaderManager;
            InitializeComponent();
            InitListView();
        }


        private void InitListView() {
            listView1.FullRowSelect = true;

            listView1.View = View.Details;
            // Add columns
            listView1.Columns.Add("Title", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Description", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Volume", 50, HorizontalAlignment.Left);
            listView1.Columns.Add("Issue", 50, HorizontalAlignment.Left);


        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);
        }


        private async void Form1_Load(object sender, System.EventArgs e) {

            pdfViewer.Hide();
            listView1.Show();

            WindowState = FormWindowState.Maximized;
            MinimumSize = Size;
            MaximumSize = Size;

            listView1.View = View.Details;
            var publications = await _dao.FindByQuery<Publication>(Publication.ByJournalId, null, _jvm.CurrentJournalId);

            listView1.Items.Clear();

            foreach (var item in publications) {

                var listitem = new ListViewItem();
                listitem.Tag = item.Id;
                listitem.Text = item.Title;
                listitem.SubItems.Add(item.Description);
                listitem.SubItems.Add("" + item.Volume);
                listitem.SubItems.Add("" + item.Issue);
                listView1.Items.Add(listitem);
            }

        }

        private void listView1_SelectedIndexChanged(object sender, System.EventArgs e) {

        }

        private void listView1_Click(object sender, EventArgs e) {

        }

        private void button1_Click(object sender, EventArgs e) {
            if (_jvm.ShowingPdf) {
                pdfViewer.Hide();
                listView1.Show();
                _jvm.ShowingPdf = false;
                return;
            }

            Hide();
            _jvm.JournalForm.Show();
        }

        private async void listView1_DoubleClick(object sender, EventArgs e) {
            var publicationId = int.Parse(listView1.SelectedItems[0].Tag.ToString());

            this.listView1.Hide();
            this.pdfViewer.Show();
            _jvm.ShowingPdf = true;
            // Load PDF Document into WinForms Control
            var pdfDocument = await _pdfLoaderManager.LoadPdfData(publicationId);
            pdfViewer.Load(pdfDocument);

        }
    }
}
