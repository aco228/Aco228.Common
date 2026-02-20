namespace Aco228.Common.Models;

public class HostMachineContract
{
    public string MachineName { get; set; }
    public string ApplicationName { get; set; }

    public static HostMachineContract CreateFromEnvironment()
    {
        return new HostMachineContract()
        {
            ApplicationName = Environment.GetEnvironmentVariable("APP_NAME") ?? throw new Exception("APP_NAME is not set"),
            MachineName = Environment.GetEnvironmentVariable("MACHINE_NAME") ?? throw new Exception("MACHINE_NAME is not set"),
        };
    }
}