using FnEsc.Models;

namespace FnEsc;

/// <summary>
/// Interact with Asus keyboard sending report to enable/disable FnEsc
/// </summary>
public class Keyboard : Hardware
{
    /// <summary>
    /// The keyboard name
    /// </summary>
    private readonly string _keyboard_name = "Asus Keyboard";

    /// <summary>
    /// The report to enable FnEsc
    /// </summary>
    private readonly byte[] _reportOn = [0x5a, 0xd0, 0x4e, 0x01];

    /// <summary>
    /// The report to disable FnEsc
    /// </summary>
    private readonly byte[] _reportOff = [0x5a, 0xd0, 0x4e, 0x00];

    /// <summary>
    /// The hidraw of the Asus Keyboard
    /// </summary>
    private readonly string _keyboard;


    /// <summary>
    /// Constructor for the Keyboard class. It looks for a device containing the
    /// KEYBOARD_STRING in its name and sets the _keyboard field to the hidraw of
    /// that device. If no device is found, it throws a KeyNotFoundException.
    /// </summary>
    /// <exception cref="KeyNotFoundException">If the keyboard is not found</exception>
    public Keyboard()
    {
        var device = GetHidDevices()
                        .Where(p => p.DeviceName.Contains(_keyboard_name, StringComparison.InvariantCultureIgnoreCase))
                        .FirstOrDefault();

        if (device != null && device.Hidraw != null)
        {
            _keyboard = device.Hidraw;
        }
        else
        {
            throw new KeyNotFoundException($"Keyboard [${_keyboard_name}] not found");
        }
    }

    public string Hidraw => _keyboard;

    /// <summary>
    /// Send the report to enable the Fn key on the keyboard.
    /// </summary>
    public void FnEsc_On() => SendReport(_keyboard, _reportOn);

    /// <summary>
    /// Send the report to disable the Fn key on the keyboard.
    /// </summary>
    /// <param name="hidraw">The hidraw to send the report to</param>
    public void FnEsc_Off() => SendReport(_keyboard, _reportOff);

}