using System.Collections.Generic;
using System;
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
using Microsoft.Win32;
using System.ComponentModel;


namespace EX2
{
    public partial class viewModel : INotifyPropertyChanged
    {
        //private List<ChartData> points = new List<ChartData>();
        public event PropertyChangedEventHandler PropertyChanged;
        //private List<String> variables;
        //private string VM_time;
        //private string VM_playback_speed;
        private FlightSimulator model;
        //private List<KeyValuePair<float, float>> VM_selectedFeature;
        //private List<KeyValuePair<float, float>> VM_correlatedFeature;

        // works for both csv file and .exe file.
        
        public viewModel(FlightSimulator sim)
        {
            this.model = sim;
            this.model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                this.notifyPropertyChanged("VM_" + e.PropertyName);
            };
        }
        /// <summary>
        /// send CSV file name to model.
        /// </summary>
        /// <param name="name"></param>
        public void update_CSVFileName(string name)
        {
            this.model.setCSVFile(name);
        }
        /// <summary>
        /// send FlightGear exe path to model.
        /// </summary>
        /// <param name="name"></param>
        public void update_OpenFlightGear(string name)
        {
            this.model.setFGPath(name);
        }
        /// <summary>
        /// gotten update from model, send the update to view.
        /// </summary>
        /// <param name="propName"></param>
        public void notifyPropertyChanged (string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        /// <summary>
        /// vector of points of the feature selected by the user.
        /// </summary>
        public List<KeyValuePair<float, float>> VM_SelectedFeature { 
            get
            {
                return this.model.SelectedFeature;
            } 
        }
        /// <summary>
        /// vector of points of the correlated feature to the selected feature.
        /// </summary>
        public List<KeyValuePair<float, float>> VM_CorrelatedFeature
        {
            get
            {
                return this.model.CorrelatedFeature;
            }
        }
        /// <summary>
        /// vector of strings holding all column names in csv/xml file.
        /// </summary>
        public List<String> Variables
        {
            get
            {
                return this.model.Variables;
            }
        }
        /// <summary>
        /// current time passed in 'XX:XX:XX' format.
        /// </summary>
        public string Time
        {
            get
            {
                return this.model.Time;
            }
        }
        /// <summary>
        /// current video play speed - current pace we send data to flight gear / number of element being added to the graphs per second.
        /// </summary>
        public string Playback_speed
        {
            get
            {
                return this.model.Playback_speed;
            }
        }

    }
}