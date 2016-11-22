using System;

public static class Settings 
{
	public static string GetSetting(string settingName)
	{
        return System.Environment.GetEnvironmentVariable(settingName, EnvironmentVariableTarget.Process);
	}
}