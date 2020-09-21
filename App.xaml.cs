using log4net;
using Npgsql;
using System;
using System.Threading;
using System.Windows;

namespace Restaurant_Pos
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(App));

        protected override void OnStartup(StartupEventArgs e)
        {
            //log4net.Config.XmlConfigurator.Configure();
            log.Info("        =============  Started Logging  =============        ");
            log.Info("        #############  Application Stated  #############        ");


            try
            {


                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                NpgsqlConnection connection = new NpgsqlConnection(PostgreSQL.ConnectionString);
                connection.Open();
                connection.Close();
                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                log.Error("        =============  Database Not Configured Properly  =============        ");
                if (MessageBox.Show(ex.ToString(),
                        "Database Error", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                {
                    Environment.Exit(0);
                }
                Environment.Exit(0);
            }



        }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "Unhandled Thread Exception");
            // here you can log the exception ...
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show((e.ExceptionObject as Exception).Message, "Unhandled UI Exception");
            // here you can log the exception ...
        }

        protected override void OnExit(ExitEventArgs e)
        {
            log.Info("        #############  Application Closed  #############        ");
            log.Info("        =============  Ending The Logs  =============        ");
            base.OnExit(e);
        }
    }
}