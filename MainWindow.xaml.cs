using System;
using System.Windows;
using System.Windows.Controls;
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
    }
}