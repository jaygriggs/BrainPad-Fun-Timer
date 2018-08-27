using System;
using System.Collections;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Drivers.ShijiLighting.APA102C;

namespace FunTimer
{
    class Program
    {
        public static void LEDThread()
        {
            while (true)
            {
                BrainPad.LightBulb.TurnRed();
                BrainPad.Wait.Seconds(0.5);
                BrainPad.LightBulb.TurnOff();
                BrainPad.Wait.Seconds(0.5);
            }
        }
       
        const int _LedCount = 25;
        static APA102C Strip = new APA102C(_LedCount, GHIElectronics.TinyCLR.Pins.BrainPad.Expansion.SpiBus.Spi1);

        static void ControlLEDs(int count)
        {

                //clear all leds
                for ( int i = 0; i < _LedCount; i++)
                {
                    Strip.SetLED(i, 0, 0, 0, 5);
                }
            //set the "count" to on
            for ( int i = 0; i < count; i++)
                {
                    double color = ((double)i / _LedCount) * 255;
                    Strip.SetLED(i, (int)color, (int)(255 - color), 0, 5);
                }
                //refresh
                Strip.RefreshLEDs();
        }

        static void Main()
        {
            
            Thread Blinker = new Thread(LEDThread);
            Blinker.Start();
            int delaytime = 60;
            int currenttime = delaytime;

            while (true)
            {

                if (BrainPad.Buttons.IsDownPressed())
                {
                    currenttime = 60;
                }
                BrainPad.Display.DrawText(0, 0, currenttime.ToString());
                BrainPad.Display.RefreshScreen();             
                int howmanyledstoturnon = (int)(((double)currenttime / delaytime * _LedCount));
                currenttime--;
                ControlLEDs(howmanyledstoturnon);
                Thread.Sleep(500);
                ControlLEDs(howmanyledstoturnon--);
                Thread.Sleep(500);
                ControlLEDs(howmanyledstoturnon++);


                if (currenttime<=10 && currenttime >= 0 )
                  {
              BrainPad.Buzzer.Beep();

                  }
               if (currenttime <= 0)
                {
                    for (int i = 0; i < _LedCount; i++)
                    {
                        Strip.SetLED(i, 0, 0, 255, 5);
                    }
                    Strip.RefreshLEDs();
                }
            }

        }

      
    }
}
