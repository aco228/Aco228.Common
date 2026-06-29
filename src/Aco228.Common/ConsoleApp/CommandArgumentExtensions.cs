namespace Aco228.Common.ConsoleApp;

public static class CommandArgumentExtensions
{
    /// <summary>
    /// Execute arguments
    /// </summary>
    /// <returns>Should execution continue</returns>
    public static async Task<bool> Execute<T>(this T commandArguments) where T : CommandArgument
    {
        if (commandArguments.ShowHelp)
        {
            commandArguments.PrintHelp();
            return false;
        }

        var shouldContinue = await commandArguments.ShouldContinueAfterArguments();
        return shouldContinue;
    }
    
}