using System;
using System.Collections;
using System.Text;
using System.Threading;

using GHIElectronics.TinyCLR.Devices.Spi;

namespace GHIElectronics.TinyCLR.Drivers.ShijiLighting.APA102C
{
    public class APA102C
    {
        private SpiController spiBusContoller;
        private SpiDevice spiBus;
        private readonly string spiID;
        private readonly SpiConnectionSettings spiSettings;
        private readonly byte[] startFrame;
        private readonly byte[] stopFrame;
        private byte[] ledFrame;
        private readonly int pixelCount;
        private int ledFrameIndex;
        private int pixelIntensity;




        public APA102C(int pixelCount, string spiID)
        {
            this.pixelCount = pixelCount;
            startFrame = new byte[4];                           // Initializes all elements in startFrame array to 0x0
            stopFrame = new byte[] { 0xff, 0xff, 0xff, 0xff };
            ledFrame = new byte[this.pixelCount * 4];           // Sets up 32 bit data buffer per each LED Frame (pixel)

            for (int i = 0; i < ledFrame.Length; i += 4)        // Initializes frame buffer to active LED frame data
            {
                ledFrame[i] = 0xE0;
            }

            spiSettings = new SpiConnectionSettings(7)
            {
                Mode = SpiMode.Mode0,
                ClockFrequency = 1_200_000,
                DataBitLength = 8
            };

            this.spiID = spiID;
            spiBusContoller = SpiController.FromName(this.spiID);
            spiBus = spiBusContoller.GetDevice(spiSettings);
        }
        /*
        static public short Set16bitColor(byte Red, byte Green, byte Blue)
        {
            if (Red > 0x1f) throw new Exception();
            if (Green > 0x2f) throw new Exception();

            short color = 0;
            color = (short)((Blue & 0xF8) >> 3);
            color |= (short)(((Green & 0xFC) >> 2) << 5);
            color |= (short)(((Red & 0xF8) >> 3) << 11);
            return color;
        }
        */

        //static public int SetColor(byte Red, byte Green, byte Blue)
        //{
        //    int color;
        //    color = Red << 16;
        //    color |= Green << 8;
        //    color |= Blue << 0;
        //    return color;
        //}

        /// <param name="pixelIndex">The pixel in the chain to draw</param>
        /// <param name="colorData16bit565">The 16 bit color to draw. It is in the RGB 565 format.</param>
        /// <param name="pixelIntensity">The level from 0 (off) to 31 (highest brightness)</param>
        public void SetLED(int pixelIndex, int red, int green, int blue, int pixelIntensity = 1)
        {
            if (pixelCount < pixelIndex)
            {
                throw new Exception("Pixel Index is out of range of Pixel Count.");
            }

            ledFrameIndex = pixelIndex * 4; // Positions index to beginning of each LED frame

            this.pixelIntensity = pixelIntensity;
            this.pixelIntensity |= 0x7 << 5;

            ledFrame[ledFrameIndex] = (byte)this.pixelIntensity;
            ledFrame[ledFrameIndex + 1] = (byte)(blue);         // Blue byte
            ledFrame[ledFrameIndex + 2] = (byte)(green);        // Green byte
            ledFrame[ledFrameIndex + 3] = (byte)(red);          // Red byte
        }

        public void RefreshLEDs()
        {
            // The startFrame sends all zeros first in the chain to let the addressable LEDs know the next set of data is the LED Frame Data field.
            spiBus.Write(startFrame);
            // ledFrame is the entire pixel chain data.
            spiBus.Write(ledFrame);
            // Unknown if this is necessary
            spiBus.Write(stopFrame);
        }
    }
}
