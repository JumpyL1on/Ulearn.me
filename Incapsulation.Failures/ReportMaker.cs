using System;
using System.Collections.Generic;
using System.Linq;

namespace Incapsulation.Failures
{
    public class ReportMaker
    {
        public static IEnumerable<string> FindDevicesFailedBeforeDateObsolete(
            int day,
            int month,
            int year,
            int[] failureTypes,
            int[] deviceId,
            object[][] times,
            List<Dictionary<string, object>> list)
        {
            var devices = new Device[list.Count];
            for (var i = 0; i < devices.Length; i++)
            {
                var id = deviceId[i];
                var name = list[i]["Name"] as string;
                var failureType = failureTypes[i];
                var time = times[i];
                var failureDate = new DateTime((int) time[2], (int) time[1], (int) time[0]);
                devices[i] = new Device(id, name, failureType, failureDate);
            }

            return FindDevicesFailedBeforeDate(devices, new DateTime(year, month, day));
        }

        private static IEnumerable<string> FindDevicesFailedBeforeDate(Device[] devices, DateTime dateTime)
        {
            return devices
                .Where(device => Failure.IsFailureSerious(device.FailureType) && device.FailureDate < dateTime)
                .Select(device => device.Name)
                .ToList();
        }
    }

    public class Device
    {
        public int Id { get; }
        public string Name { get; }
        public int FailureType { get; }
        public DateTime FailureDate { get; }

        public Device(int id, string name, int failureType, DateTime failureDate)
        {
            Id = id;
            Name = name;
            FailureType = failureType;
            FailureDate = failureDate;
        }
    }

    public class Failure
    {
        public static bool IsFailureSerious(int failureType)
        {
            return failureType % 2 == 0;
        }
    }
}
