using ConsoleApp1.app.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultithreadingWPF
{


    public partial class MainWindow : Window
    {
        ManagerJob managerJob;
       
        public MainWindow()
        {
            InitializeComponent();
        }
        public void AddMessageToPayload()
        {
            this.Dispatcher.Invoke(() =>
            {
                /// using '=' Instead of '+=' to save memory
                this.txtPayload.Text = ManagerJob.payloadString.ToString();
                this.scrollView.ScrollToEnd();

            });
        }
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string countStr = txtCountThreads.Text;
            try
            {
                int count = Int32.Parse(countStr);
                if(count <= 0 )
                {
                    MessageBox.Show("Enter number > 0");
                }
                else
                {
                    managerJob = new ManagerJob(count);
                    // Add reference to external method to update data
                    ManagerJob.addMessage += AddMessageToPayload;
                    managerJob.Start();
                    btnStart.IsEnabled = false;
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Can't Parsed '{0}'", countStr);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            this.managerJob.Stop();
        }
    }
}
