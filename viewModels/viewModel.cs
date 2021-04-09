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
        public event PropertyChangedEventHandler PropertyChanged;
        private FlightSimulator model;
        
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
        public string VM_Time
        {
            get
            {
                return this.model.Time;
            }
        }
        /// <summary>
        /// current video play speed - current pace we send data to flight gear / number of element being added to the graphs per second.
        /// </summary>
        public string VM_Playback_speed
        {
            get
            {
                return this.model.Playback_speed;
            }
        }
        public void bottom_control_clicked(string buttonName)
        {
            model.bottom_control_clicked(buttonName);
        }
        /// <summary>
        /// user moved the time slider - update model with new time.
        /// </summary>
        /// <param name="value"></param>
        
        public void updateTime(double value)
        {
            this.model.updateTime(value);
        }
        
    }
}