namespace Aco228.Common.Extensions;

public static class TasksExtensions
{
    public static Task WaitWithToken(this Task task, CancellationToken? token)
    {
        if (token == null)
            return task;

        return task.WaitAsync(token.Value);
    }
    
    public static Task<T> WaitWithToken<T>(this Task<T> task, CancellationToken? token)
    {
        if (token == null)
            return task;

        return task.WaitAsync(token.Value);
    }
    
    public static void IgnoreException(Action function)
    {
        try
        {
            function();
        }
        catch
        {
            int a = 0;
        }
    }
    
    public static T IgnoreException<T>(Func<T> function, T defaultValue)
    {
        try
        {
            return function();
        }
        catch
        {

            return defaultValue;
        }
    }

    public static async Task IgnoreExceptionAsync(Task function)
    {
        try
        {
            await function;
        }
        catch
        {
            int a = 0;
        }
    }

    public static async Task IgnoreExceptionAsync<T>(Task<T> function)
    {
        try
        {
            await function;
        }
        catch
        {
            int a = 0;
        }
    }

    public static async Task IgnoreExceptionAsync(ValueTask function)
    {
        try
        {
            await function;
        }
        catch
        {
            int a = 0;
        }
    }

    public static void ResilienceVoid(Action function, int retries = 20, int timeoutInMs = 10000, bool throwException = true)
    {
        int index = 0;
        for (;;)
        {
            try
            {
                function();
                return;
            }
            catch (Exception ex)
            {
                index++;
                if (index < retries)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeoutInMs));
                    continue;
                }

                if (throwException)
                    throw;
                else
                    break;
            }
        }
    }

    public static T Resilience<T>(Func<T> function, int retries = 20, int timeoutInMs = 10000, T? defaultValue = default, bool throwException = true)
    {
        int index = 0;
        for (;;)
        {
            try
            {
                return function();
            }
            catch (Exception ex)
            {
                index++;
                if (index < retries)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeoutInMs));
                    continue;
                }

                if (defaultValue != null)
                    return defaultValue;

                if (throwException)
                    throw;
                else
                    return defaultValue ?? default;
            }
        }
    }

    public static async Task WaitForMinutes(int minutes, string consoleMessage)
    {
        Console.WriteLine("----");
        Console.WriteLine("---- ---- ----");
        Console.WriteLine($"{consoleMessage}.. Pause for {minutes} minutes");
        Console.WriteLine("---- ---- ----");
        Console.WriteLine("----");
        
        await Task.Delay(minutes * 60000);
    }

    public static async Task WaitForSeconds(int seconds, string consoleMessage)
    {
        if (!string.IsNullOrEmpty(consoleMessage))
        {
            Console.WriteLine("----");
            Console.WriteLine("---- ---- ----");
            Console.WriteLine($"{consoleMessage}.. Pause for {seconds} seconds");
            Console.WriteLine("---- ---- ----");
            Console.WriteLine("----");
        }
        await Task.Delay(seconds * 1000);
    }

    public static async Task RunWithDelay(int miliseconds, Action task)
    {
        await Task.Delay(miliseconds);
        task();
    }

    public static async Task<T> ResilienceAsync<T>(Func<Task<T>> function, int retries = 5, int timeoutInMs = 10000,
        T? defaultValue = default,
        string? affirmativeException = null,
        bool throwException = true)
    {
        int index = 0;
        for (;;)
        {
            try
            {
                return await Task.Run<T>(function);
            }
            catch (Exception ex)
            {
                // TODO: facebook check access token
                
                if (!string.IsNullOrEmpty(affirmativeException) && ex.ToString().Contains(affirmativeException))
                    return defaultValue;
                
                index++;
                if (index < retries)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeoutInMs));
                    continue;
                }

                if (defaultValue != null)
                    return defaultValue;

                if (throwException)
                {
                    throw;
                    break;
                }
                else
                    return defaultValue ?? default;
            }
        }
    }

    public static async Task ResilienceVoidAsync(Func<Task> function, int retries = 5, int timeoutInMs = 10000,
        string? affirmativeException = null,
        bool throwException = true)
    {
        int index = 0;
        for (;;)
        {
            try
            {
                await Task.Run(function);
            }
            catch (Exception ex)
            {
                // TODO: facebook check access token
                
                index++;
                if (index < retries)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(timeoutInMs));
                    continue;
                }

                if (throwException)
                    throw;
                else
                    break;
            }
        }
    }
}