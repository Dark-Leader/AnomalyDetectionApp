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
        private List<String> variables = new List<String>();
        private string VM_time;
        private string VM_playback_speed;
        private FlightSimulator model;
        private List<KeyValuePair<float, float>> VM_selectedFeature = new List<KeyValuePair<float, float>>();
        private List<KeyValuePair<float, float>> VM_correlatedFeature;

        private string open_file;// works for both csv file and .exe file.
        private string stackButton_string;
        
        public viewModel(FlightSimulator sim)
        {
            this.model = sim;
            //temp buttons
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

            // temp graphs data
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
        }
        
        public void update_CSVFileName(string name)
        {
            this.model.setCSVFile(name);
        }

        public void update_OpenFlightGear(string name)
        {
            this.model.setFGPath(name);
        }

        public void NotifyPropertyChanged (string propName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public string Open_file
        {
            get
            {
                return open_file;
            }
            set
            {
                if (value != open_file)
                {
                    open_file = value;                 
                }
            }
        }

        public List<KeyValuePair<float, float>> SelectedFeature { 
            get
            {
                return VM_selectedFeature;
            } set
            {
                if (value != VM_selectedFeature) {
                    VM_selectedFeature = value;
                }
            }
        }

        public List<KeyValuePair<float, float>> CorrelatedFeature
        {
            get
            {
                return VM_correlatedFeature;
            }
            set
            {
                if (value != VM_correlatedFeature)
                {
                    VM_correlatedFeature = value;
                }
            }
        }

        public string StackButton_string
        {
            get
            {
                return stackButton_string;
            }
            set
            {
                if (value != stackButton_string)
                {
                    stackButton_string = value;
                }
            }
        }

     public List<String> Variables
        {
            get
            {
                return variables;
            }
            set
            {
                if (value != variables)
                {
                    variables = value;
                }
            }
        }
        public string Time
        {
            get
            {
                return VM_time;
            }
            set
            {
                if (value != VM_time)
                {
                    VM_time = value;
                }
            }
        }

        public string Playback_speed
        {
            get
            {
                return VM_playback_speed;
            }
            set
            {
                if (value != VM_playback_speed)
                {
                    VM_playback_speed = value;
                }
            }
        }




    }
}