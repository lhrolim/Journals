﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using journals.commons.Model.dao;
using journals.commons.Model.Entities;
using journals.commons.SimpleInjector;
using journals.commons.Util.Redis;
using Journals.Client.Model;
using Journals.Client.Model.View;

namespace Journals.Client {
    public partial class JournalForm : Form, ISingletonComponent, IEventListener<RefreshJournalsEvent> {

        private readonly IGenericDao _dao;
        private readonly JournalViewModel _journalViewModel;
        private readonly PublicationForm _publicationForm;
        private readonly PublicationSyncManager _publicationSyncManager;
        private readonly RedisService _redisService;


        public JournalForm(IGenericDao dao, JournalViewModel journalViewModel, PublicationForm publicationForm, PublicationSyncManager publicationSyncManager, RedisService redisService) {
            _dao = dao;
            _journalViewModel = journalViewModel;
            _publicationForm = publicationForm;
            _publicationSyncManager = publicationSyncManager;
            _redisService = redisService;
            InitializeComponent();
            InitListView();
        }


        private void InitListView() {
            listView1.View = View.List;
            listView1.FullRowSelect = true;
        }



        private async void Form1_Load(object sender, EventArgs e) {

            WindowState = FormWindowState.Maximized;
            MinimumSize = Size;
            MaximumSize = Size;

            listView1.View = View.Details;

            await _publicationSyncManager.HandleSubscriptions();
            await _publicationSyncManager.SyncData();


            //load again, after sync took place
            //            await LoadFromDB();

        }

        private async void JournalForm_Shown(object sender, EventArgs e) {
            await LoadFromDB();
        }

        private async Task LoadFromDB(bool invokeRequired = false) {

            var dbJournals = await _dao.FindByQuery<ClientJournalDto>(ClientJournalDto.ByUser);

            if (invokeRequired) {
                this.Invoke(new MethodInvoker(delegate {
                    DoUpdateListView(dbJournals);
                }));
            } else {
                DoUpdateListView(dbJournals);
            }
            

        }

        private void DoUpdateListView(IList<ClientJournalDto> dbJournals)
        {
            Cursor.Current = Cursors.WaitCursor;
            listView1.Items.Clear();


            foreach (var item in dbJournals)
            {
                var listitem = new ListViewItem();
                listitem.Tag = item.Id;
                listitem.Text = item.Name;
                listitem.SubItems.Add(item.Description);
                listitem.SubItems.Add(item.PublicationCount.ToString());
                listView1.Items.Add(listitem);
            }

            listView1.Update(); // In case there is databinding
            listView1.Refresh();

            Cursor.Current = Cursors.Default;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void listView1_DoubleClick(object sender, EventArgs e) {
            _journalViewModel.CurrentJournalId = int.Parse(listView1.SelectedItems[0].Tag.ToString());
            _journalViewModel.JournalForm = this;
            Hide();

            //TODO:Authenticate

            _publicationForm.ShowDialog();
        }

        private async void button1_Click(object sender, EventArgs e) {
            if (!_redisService.ServiceAvailable) {
                await _publicationSyncManager.SyncData();
            }
            await LoadFromDB();
        }

        private void logout_Click(object sender, EventArgs e) {
            Hide();

            _journalViewModel.Config = null;
            SimpleInjectorGenericFactory.Instance.GetObject<LoginForm>().Show();
        }


        public async void HandleEvent(RefreshJournalsEvent eventToDispatch) {
            await LoadFromDB(true);
        }
    }
}
