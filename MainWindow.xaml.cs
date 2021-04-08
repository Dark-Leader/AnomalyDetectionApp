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
        private viewModel vm;
        
        public MainWindow()
        {
            InitializeComponent();
            // Example for injecting 15 buttons into the stackPanel
            vm = new viewModel(new FlightSimulator());
            create_buttons();
            this.DataContext = vm;
            LoadLineChartData();


        }
        // default function to handle all stackPanel button clicks.
        private void stackButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Console.WriteLine(b.Content);

        }

        private void create_buttons()
        {

            
            foreach (string var in this.vm.Variables)
                
            {
                Button button = new Button();
                button.Content = var;
                string name = var.Replace("-", "_");
                button.Name = name;
                button.Click += new RoutedEventHandler(stackButton_Click);
                sp.Children.Add(button);
            }
            
        }

        /// <summary>
        /// opens file explorer and allows the user to choose a file and then sends the viewmodel said file name.
        /// </summary>
        /// <param name="sender">  </param>
        /// <param name="e"></param>
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
                Button clicked = (Button)sender;
                if (clicked.Name == "btnOpenCSV")
                {
                    this.vm.update_CSVFileName(fileNames);
                } else
                {
                    this.vm.update_OpenFlightGear(fileNames);
                }
            }
        }

        // TODO: send the model and the flightgear simulator the current value of the slider.
        //Slider.
        private void SliderValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                Console.WriteLine(SliderValue);
            }

        }

        private void LoadLineChartData()
        {
            selectedFeature.DataContext = this.vm.VM_SelectedFeature;
            correlatedFeature.DataContext = this.vm.VM_CorrelatedFeature;
        }

    }
}