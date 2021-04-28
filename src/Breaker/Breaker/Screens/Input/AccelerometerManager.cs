//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

using System;
using System.Collections.Generic;

using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Devices.Sensors;


namespace Breaker.Wp7.Xna4.Input
{
    class AccelerometerManager
    {

        Accelerometer accelerometer;
        bool useEmulation = false;
 
        public AccelerometerManager()
        {
        }

        private void StrartAccelerometer()
        {
            if (!useEmulation)
            {
                // If emulation is not being used, instantiate the AccelerometerSensor
                accelerometer = new Accelerometer();

                // Obtain an Observable sequence using the FromEvent method
                IObservable<IEvent<AccelerometerReadingEventArgs>> accelerometerReadingAsObservable =
                    Observable.FromEvent<AccelerometerReadingEventArgs>(
                    ev => accelerometer.ReadingChanged += ev,
                    ev => accelerometer.ReadingChanged -= ev);

                // Use from and select to create a stream of Vector3 from AccelerometerReadingEventArgs stream 
                var vector3FromAccelerometerEventArgs = from args in accelerometerReadingAsObservable
                                                        select new Vector3((float)args.EventArgs.X, (float)args.EventArgs.Y, (float)args.EventArgs.Z);


                // Subscribe to the Observable sequence of Vector3 objects
                // InvokeAccelerometerReadingChanged will be called whenever a new Vector3 is available in the stream
                vector3FromAccelerometerEventArgs.Subscribe(args => InvokeAccelerometerReadingChanged(args));

                // Start the accelerometer
                try
                {
                    accelerometer.Start();
                }
                catch (AccelerometerFailedException ex)
                {
                    readingTextBlock.Text = "error starting accelerometer";
                }
                accelerometer.Start();
            }
            else
            {
                // Otherwise, start a new thread for emulation
                Thread t = new Thread(StartAccelerometerEmulation);
                t.Start();
            }
        }


    }
}
