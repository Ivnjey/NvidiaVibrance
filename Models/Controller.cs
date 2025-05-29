using System;
using NvAPIWrapper;
using NvAPIWrapper.Display;

namespace NvidiaVibrance.Models
{
    public class NvidiaDisplayController
    {
        private Display displayDevice;
        // private uint displayId;

        public NvidiaDisplayController()
        {
            NVIDIA.Initialize();
            Display[] displays = Display.GetDisplays();
            if (displays.Length == 0)
            {
                throw new Exception("No NVIDIA display devices found.");
            }
            displayDevice = displays[0];
        }

        public void SetDigitalVibrance(int level)
        {
            if (level < 0 || level > 100)
            {
                throw new ArgumentOutOfRangeException("Digital vibrance level must be between 0 and 100.");
            }
            var colorControl = displayDevice.DigitalVibranceControl;
            colorControl.CurrentLevel = level;
        }

        public int GetDigitalVibrance()
        {
            return displayDevice.DigitalVibranceControl.CurrentLevel;
        }
        public void SetMonochrome(bool IsMonochrome)
        {
            int _level = IsMonochrome ? 0 : 50;
            SetDigitalVibrance(_level);
        }
        public void SetDigitalVibranceDefault()
        {
            int colorControl = displayDevice.DigitalVibranceControl.DefaultLevel;
            SetDigitalVibrance(colorControl);
        }

    }
}