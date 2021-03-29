using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ex1
{
    public class timeElapsed : INotifyPropertyChanged
    {
        private string time;
        public event PropertyChangedEventHandler PropertyChanged;
        public string Time
        {
            get
            {
                return time;
            }
            set
            {
                if (time != value)
                {
                    time = value;
                    PropertyChangedEventHandler handler = PropertyChanged;
                    if (handler != null)
                    {
                        handler(this, new PropertyChangedEventArgs("timeElapsed"));
                    }
                }
            }
        }

        public timeElapsed()
        {
            this.time = "00:00:00";
        }
    }
}
