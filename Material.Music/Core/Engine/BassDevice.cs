using ManagedBass;
using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Music.Core.Engine
{
    public sealed class BassDevice
    {
        #region Private members
        DeviceInfo devInfo;
        int identicator;
        #endregion

        private BassDevice(DeviceInfo info, int id)
        {
            if (!info.IsEnabled)
                throw new AccessViolationException("Device is not available for use or create object for reference.");
            identicator = id;
            devInfo = info;
        }


        public bool IsDefault => devInfo.IsDefault;
        public string DeviceName => devInfo.Name;
        public string DeviceId => devInfo.Driver;
        public DeviceType DeviceType => devInfo.Type;
        public int Identicator => identicator;


        public static IEnumerable<BassDevice> GetPlaybackDevices()
        {
            List<BassDevice> list = new List<BassDevice>();
            int dCount = Bass.DeviceCount;
            for (int i = 0; i < dCount; i++)
            {
                var dev = Bass.GetDeviceInfo(i);
                if (dev.IsEnabled && dev.Type != DeviceType.Microphone)
                {
                    BassDevice obj = new BassDevice(dev, i);
                    list.Add(obj);
                }
            }
            return list;
        }
        public static BassDevice GetDefaultDevice()
        {
            foreach (var dev in GetPlaybackDevices())
            {
                if (dev.IsDefault)
                {
                    return dev;
                }
            }
            return null;
        }
        public static BassDevice GetBassDevice(string devId)
        {
            if (devId == "")
                return null;
            foreach (var dev in GetPlaybackDevices())
            {
                if (dev.DeviceId == devId)
                {
                    return dev;
                }
            }
            return null;
        }
    }
}
