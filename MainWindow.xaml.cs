using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Controls.DataVisualization.Charting;

namespace EX2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel vm;

        public double DeadZone { get; private set; } = 0;
        public double Saturation { get; private set; } = 1;
        public bool InvertX { get; private set; } = false;
        public bool InvertY { get; private set; } = false;
        public double Sensitivity { get; private set; } = 0;
        public double Range { get; private set; } = 1;

        public MainWindow()
        {
            vm = new ViewModel(new FlightSimulator());
            this.DataContext = vm;

            InitializeComponent();
        }

        /// <summary>
        /// open file explorer and search for wanted file.
        /// </summary>
        /// <param name="sender">  </param>
        /// <param name="e"></param>
        /// 
        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Multiselect = true;
            openDialog.Filter = "Log Files|* .log|Textfiles|*.txt|All files|*.*";
            openDialog.DefaultExt = ".log";
            Nullable<bool> dialogOK = openDialog.ShowDialog();
            if (dialogOK == true)
            {
                string fileNames = "";
                foreach (string fileName in openDialog.FileNames)
                {
                    fileNames += ";" + fileName;
                }
                fileNames = fileNames.Substring(1);
                Button b = (Button)sender;
                if (b.Name == "Open_train_csv")
                {
                    vm.set_train_csv(fileNames);
                } else if (b.Name == "Open_test_csv")
                {
                    vm.set_test_csv(fileNames);
                } else
                {
                    vm.set_flight_gear(fileNames);
                }
            }
        }

        private void change_speed(object sender, RoutedEventArgs e)
        {
            
            Button b = (Button)sender;
            switch (b.Name)
            {
                case "Slower":
                    vm.change_speed(-5);
                    break;
                case "Slow":
                    vm.change_speed(-1);
                    break;
                case "Fast":
                    vm.change_speed(1);
                    break;
                case "Faster":
                    vm.change_speed(5);
                    break;
                case "Stop":
                    vm.stop();
                    break;
                case "Start":
                    vm.stop();
                    break;
                case "Restart":
                    vm.restart();
                    break;

            }
        }

        private void SmartMeter_Click(object sender, RoutedEventArgs e)
        {
            return;
        }
    }
}