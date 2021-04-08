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
            this.model.parseXML();
            //temp buttons
            /*
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            this.variables.Add("speed");
            this.variables.Add("height");
            this.variables.Add("throttle");
            this.variables.Add("width");
            this.variables.Add("HAHAHAHAHHAHA");
            this.VM_time = "00:00:00";
            this.VM_playback_speed = "1.0";
            */

            // temp graphs data
            /*
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(1, 60));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(7, 15));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(8, 23));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(40, 50));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(3, 80));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(11, 15));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(5, 20));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(26, 31));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(9, 70));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(17, 4));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(6, 12));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(15, 19));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(43, 14));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(35, 18));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(24, 41));
            this.VM_selectedFeature.Add(new KeyValuePair<float, float>(28, 500));
            this.VM_correlatedFeature = new List<KeyValuePair<float, float>>(this.VM_selectedFeature);
            */
        }
        
        public void update_CSVFileName(string name)
        {
            this.model.setCSVFile(name);
        }

        public void update_OpenFlightGear(string name)
        {
            this.model.setFGPath(name);
        }

        public void notifyPropertyChanged (string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public List<KeyValuePair<float, float>> VM_SelectedFeature { 
            get
            {
                return this.model.SelectedFeature;
            } 
        }

        public List<KeyValuePair<float, float>> VM_CorrelatedFeature
        {
            get
            {
                return this.model.CorrelatedFeature;
            }
        }

        public List<String> Variables
        {
            get
            {
                return this.model.Variables;
            }
        }
        public string Time
        {
            get
            {
                return this.model.Time;
            }
        }

        public string Playback_speed
        {
            get
            {
                return this.model.Playback_speed;
            }
        }

    }
}