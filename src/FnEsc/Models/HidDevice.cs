namespace FnEsc.Models;

public class HidDevice
{
    public required string DeviceName { get; set; } 
    public string? HidName { get; set; } 
    public string? Hidraw { get; set; }
}