using System;
using System.Windows.Forms;
using journals.commons.Model.Migration;
using journals.commons.SimpleInjector;
using Journals.Client.Util;
using log4net.Config;

namespace Journals.Client {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            XmlConfigurator.Configure();
            new MigratorExecutor().Migrate(runner => runner.MigrateUp());
            var container = ClientSimpleInjectorScanner.InitDIController();


            Application.EnableVisualStyles();
//            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(SimpleInjectorGenericFactory.Instance.GetObject<LoginForm>());


  

        }
    }
}
