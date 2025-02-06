using System;
using System.Net.Sockets;

namespace CommonUtilities
{
    public static class PulsarHealthCheck
    {
        public static async Task EnsurePulsarRunningAsync(string pulsarServiceUrl)
        {
            if (!await IsPulsarRunningAsync(pulsarServiceUrl))
            {
                Console.WriteLine("Pulsar container is not running. Please start Pulsar before running this service.");
                Environment.Exit(1);
            }
            Console.WriteLine("Pulsar is running.");
        }

        private static async Task<bool> IsPulsarRunningAsync(string pulsarServiceUrl)
        {
            try
            {
                using var client = new TcpClient();
                var uri = new Uri(pulsarServiceUrl);
                var connectTask = client.ConnectAsync(uri.Host, uri.Port);
                var timeoutTask = Task.Delay(2000);

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);
                return completedTask == connectTask && client.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}
