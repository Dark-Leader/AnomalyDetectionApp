using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using Microsoft.Win32;


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
            vm = new viewModel(new FlightSimulator("hello"));
            this.DataContext = vm;
            foreach (string var in this.vm.Variables)
            {
                Button button = new Button();
                button.Content = var;
                button.Name = "Button_" + var;
                button.Click += new RoutedEventHandler(stackButton_Click);
                sp.Children.Add(button);
            }
            LoadLineChartData();


        }
        // default function to handle all stackPanel button clicks.
        private void stackButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            Console.WriteLine(b.Name);
            //this.vm.StackButton_string = b.Name;


        }

        /// <summary>
        /// 
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
                Console.WriteLine(fileNames);
                this.vm.Open_file = fileNames;
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

        private void McChart_Load(object sender, RoutedEventArgs e)
        {
            LoadLineChartData();
        }
        private void LoadLineChartData()
        {
            ((LineSeries)McChart.Series[0]).ItemsSource =
                new KeyValuePair<float, float>[]{
        new KeyValuePair<float, float>(1, 200),
        new KeyValuePair<float, float>(1.4f, 300),
        new KeyValuePair<float, float>(2,180),
        new KeyValuePair<float, float>(4.2f, 87.7f),
        new KeyValuePair<float, float>(4.5f,79) };
        }

    }
}