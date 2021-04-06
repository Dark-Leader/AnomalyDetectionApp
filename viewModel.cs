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
        private string time;
        private string playback_speed;
        private FlightSimulator model;
        private List<Point> points = new List<Point>();

        private string open_file;// works for both csv file and .exe file.
        private string stackButton_string;
        
        public viewModel(FlightSimulator sim)
        {
            this.model = sim;
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
            this.time = "00:00:00";
            this.playback_speed = "1.0";
            this.points.Add(new Point(3,6));
            this.points.Add(new Point(1, 2));
            this.points.Add(new Point(2, 4));
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
                return time;
            }
            set
            {
                if (value != time)
                {
                    time = value;
                }
            }
        }

        public string Playback_speed
        {
            get
            {
                return playback_speed;
            }
            set
            {
                if (value != playback_speed)
                {
                    playback_speed = value;
                }
            }
        }




    }
}