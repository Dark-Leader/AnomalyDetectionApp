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

            vm = new viewModel(new FlightSimulator());
            // create the buttons in the left side of the window.
            create_buttons();
            this.DataContext = vm;
            // load the graphs.
            LoadLineChartData();


        }
        /// <summary>
        /// default function for all variables. if a user selects a variable we update the viewModel about it.
        /// </summary>
        /// <param name="sender"> button clicked </param>
        /// <param name="e"></param>
        private void stackButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Console.WriteLine(b.Content);

        }

        /// <summary>
        /// add buttons to the left stack of variables.
        /// </summary>
        private void create_buttons()
        {

            foreach (string var in this.vm.Variables)
                
            {
                // create button and add it to the stack frame.
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
        /// <param name="sender"> button selected </param>
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

        /// <summary>
        /// user selected a new time to skip to. need to send update to viewModel of the selected time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SliderValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (IsLoaded)
            {
                // this.vm.updateTime(SliderValue);
                Console.WriteLine(SliderValue);
            }

        }

        /// <summary>
        /// load in all the graphs in the view.
        /// </summary>
        private void LoadLineChartData()
        {
            selectedFeature.DataContext = this.vm.VM_SelectedFeature;
            correlatedFeature.DataContext = this.vm.VM_CorrelatedFeature;
        }

    }
}