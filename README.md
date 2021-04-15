**Anomaly Detector - Flight Simulator**

Creators: Lola Sirota, Noam Salomon, Sam Katz, Amit Ben Shimon

**Main Project Features:**

- Custom graphs that displays the different features in real time.
- Custom gauges that displays important features all time (altimeter, airspeed, heading, yaw, roll, pitch).
- Custom joystick that displays current aileron, elevator, rudder, throttle readings in a graphical way (the joystick moves up,down,left, right according to the real time values of said features).
- FlightGear opens automatically once the user provides a path to the executable.
- FlightGear runs automatically and updates once opened and receives data from our Model.
- Custom DLL's that implements Anomaly Detection Algorithms (we display said anomalies in our graphs).


**Project Structure:**
- All view related object are inside the View Folder.
- All viewModel related objects are inside the viewModel Folder.
- All Model related objects are inside the Model Folder.


**Requirements:**
- .NET framework version 4.8
- WPFToolkit for graphs : https://www.nuget.org/packages/WPFToolkit/3.5.50211.1?_src=template
- WPFToolkit for graphs : https://www.nuget.org/packages/WPFToolkit.DataVisualization/3.5.50211.1?_src=template
- images and files inside the resources folder.


**How To Use:**
Run the project, once the GUI loaded, click the "Open Train CSV" button, once selected navigate to your regular (train mode) csv file.
Next, click the "Open Test CSV" button, navigate to your test csv file - file you wish to detect anomalies on.
Next, click the "Open Flight Gear" button and navigate to the exe file of flight gear on your computer.
Next, you can click the "Choose Algorithm" button and navigate to your anomaly detection dll. if this button is not selected then the default is linear regression based algorithm.
Next, click on the play button and the program will begin to display the data in real time, open flight gear and simulate the entire flight.
Once the playback has begun you can increase/decrease playback speed, jump to a specific time and inspect whichever feature you wish.

**For More Information:**
Please watch our short premiere for our app at Youtube - https://youtu.be/ANCKJaKj-6w
