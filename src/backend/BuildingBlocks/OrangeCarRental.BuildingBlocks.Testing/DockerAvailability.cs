using System.Diagnostics;
using Xunit;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Testing;

/// <summary>
///     Utility class to check Docker availability for integration tests.
/// </summary>
public static class DockerAvailability
{
    private static bool? cachedResult;

    /// <summary>
    ///     Checks if Docker is available and running.
    /// </summary>
    /// <returns>True if Docker is available, false otherwise.</returns>
    public static bool IsDockerAvailable()
    {
        if (cachedResult.HasValue) return cachedResult.Value;

        cachedResult = CheckDockerAvailability();
        return cachedResult.Value;
    }

    /// <summary>
    ///     Skips the test if Docker is not available.
    /// </summary>
    public static void SkipIfDockerUnavailable()
    {
        if (!IsDockerAvailable())
            Assert.Skip("Docker is not available. Start Docker Desktop to run integration tests.");
    }

    private static bool CheckDockerAvailability()
    {
        try
        {
            // Use docker info command to check if Docker is running
            var psi = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = "info",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null) return false;

            process.WaitForExit(5000);
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
