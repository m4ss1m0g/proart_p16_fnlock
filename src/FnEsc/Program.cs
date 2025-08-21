// See https://aka.ms/new-console-template for more information

using FnEsc;

if (args.Length == 0)
{
    Console.WriteLine("Usage: FnEsc <ON|OFF>");
}
else
{
    var keyboard = new Keyboard();
    
    Console.WriteLine($"Keyboard found on {keyboard.Hidraw}");

    if (args[0] == "ON")
    {
        keyboard.FnEsc_On();
        Console.WriteLine("FnEsc is on");
    }
    else
    {
        keyboard.FnEsc_Off();
        Console.WriteLine("FnEsc is off");
    }

}
