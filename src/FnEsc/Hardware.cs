using FnEsc.Models;

namespace FnEsc;

/// <summary>
/// A utility class that provides methods for interacting with hardware devices on a Linux system.
/// </summary>
public class Hardware
{
    /// <summary>
    /// Read the /proc/bus/input/devices and compose a list of 
    /// all devices with related SysFs 
    /// </summary>
    /// <returns>List of system devices</returns>
    /// <exception cref="FileNotFoundException">If the /proc/bus/input/devices is not found</exception>
    public static IEnumerable<Device> GetDevices()
    {
        var devices = new List<Device>();
        var procFile = "/proc/bus/input/devices";

        if (File.Exists(procFile))
        {
            var rows = File.ReadLines(procFile);

            Device? device = null;

            foreach (var row in rows)
            {
                if (row.StartsWith("I:"))
                {
                    if (device != null)
                    {
                        devices.Add(device);
                    }

                    device = new Device();

                    var e = row.Split(" ");

                    device.Vendor = e[2].Split("=")[1];
                    device.Product = e[3].Split("=")[1];
                    device.Version = e[4].Split("=")[1];
                }

                if (row.StartsWith("N:") && device is not null)
                {
                    device.Name = row.Split("=")[1].Replace("\"", "");
                }

                if (row.StartsWith("S:") && device is not null)
                {
                    device.SysFs = row.Split("=")[1];
                }

            }

        }
        else
        {
            throw new FileNotFoundException($"{procFile} not found");
        }


        return devices;
    }


    /// <summary>
    /// Read all /sys/class/hidraw/*/uevent and returns a list of all hidraws
    /// with related HID_NAME
    /// </summary>
    /// <returns>List of Hidraws</returns>
    public static IEnumerable<Hidraw> GetHidraws()
    {
        var hidraws = new List<Hidraw>();

        // read all hidraws
        var basePath = "/sys/class/hidraw";
        var hidrawDirs = Directory.GetDirectories(basePath, "hidraw*");

        // For each hidraw 
        foreach (var item in hidrawDirs)
        {
            var hidrawn = item.Replace(basePath, string.Empty);
            var h = new Hidraw();

            var path = File.ResolveLinkTarget(item, true)?.FullName;
            h.Path = path?
                        .Replace(hidrawn, string.Empty)
                        .Replace("/hidraw", string.Empty)
                        .Replace("/sys", string.Empty);

            // look ad uevent for HID_NAME
            var uevent = Path.Combine(item, "device", "uevent");
            if (File.Exists(uevent))
            {
                // get HID_NAME from hidraw file
                var rows = File.ReadLines(uevent);
                var name = rows.Where(p => p.StartsWith("HID_NAME"))
                               .Select(p => p.Split("=")[1])
                               .First();
                h.Hid = item.Replace("/sys/class/hidraw/", string.Empty);
                h.Name = name;
            }


            hidraws.Add(h);
        }


        return hidraws;
    }


    /// <summary>
    /// Get a list of all hidraws with related HID_NAME and Device name
    /// </summary>
    /// <returns>List of Hidraws with related HID_NAME and Device name</returns>
    public static IEnumerable<HidDevice> GetHidDevices()
    {
        foreach (var item in GetHidraws())
        {
            var name = GetDevices().FirstOrDefault(p => (p.SysFs ?? "").StartsWith(item.Path ?? ""))?.Name;
            if (name is not null)
            {
                yield return new HidDevice { DeviceName = name, Hidraw = item.Hid, HidName = item.Name };
            }
        }
    }

    /// <summary>
    /// Send a report to the given hidraw
    /// </summary>
    /// <param name="hidraw">The hidraw to send the report to</param>
    /// <param name="report">The report to send</param>
    /// <exception cref="UnauthorizedAccessException">If the program is not run with sudo</exception>
    public static void SendReport(string hidraw, byte[] report)
    {
        try
        {
            using var fs = new FileStream($"/dev/{hidraw}", FileMode.Open, FileAccess.Write);
            fs.Write(report, 0, report.Length);
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("You must use sudo to execute the program");
            throw;
        }
    }
}
