using System;
public static class Log
{
    public static void Info(string message)
    {
        CitizenFX.Core.Debug.WriteLine($"^5[INFO]^7[{GetTimestamp()}] {message}");
    }

    public static void Warn(string message)
    {
        CitizenFX.Core.Debug.WriteLine($"^3[WARN]^7[{GetTimestamp()}] {message}");
    }

    public static void Error(Exception message)
    {
        Error($"{message.Message} - {message.StackTrace}");
    }
    public static void Error(string message)
    {
        CitizenFX.Core.Debug.WriteLine($"^1[ERROR]^7[{GetTimestamp()}] {message}");
    }

    private static string GetTimestamp()
    {
        try
        {
            return $"{DateTime.Now:dd/MM/yyyy HH:mm:ss}";
        }
        catch
        { return "0/0/0000 00:00:00"; }
    }

    internal static void Debug(string v)
    {
        CitizenFX.Core.Debug.WriteLine($"^2[DEBUG]^7[{GetTimestamp()}] {v}");
    }
}