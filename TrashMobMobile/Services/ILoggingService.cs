namespace TrashMobMobile.Services;

using System;
using Sentry;

public interface ILoggingService
{
    void LogError(Exception exception);
    void LogMessage(string message);
}

public class LoggingService : ILoggingService
{
    public void LogError(Exception exception)
    {
        LogMessage(exception.Message);
        LogMessage(exception.StackTrace);
        SentrySdk.CaptureException(exception);
    }

    public void LogMessage(string message)
    {
        SentrySdk.CaptureMessage(message);
    }
}

public class DebugLoggingService : ILoggingService
{
    public void LogError(Exception exception)
    {
        Console.WriteLine(exception.Message);
        Console.WriteLine(exception.StackTrace);
    }

    public void LogMessage(string message)
    {
        Console.WriteLine(message);
    }
}