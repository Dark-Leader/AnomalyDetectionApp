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


namespace EX2
{
    public partial class viewModel
    {
        //private List<ChartData> points = new List<ChartData>();
        private List<String> variables = new List<String>();
        private string time;
        private double playback_speed;


        private string open_file;// works for both csv file and .exe file.
        private string stackButton_string;
        public viewModel()
        {
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
            this.playback_speed = 1.0;

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

        public double Playback_speed
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