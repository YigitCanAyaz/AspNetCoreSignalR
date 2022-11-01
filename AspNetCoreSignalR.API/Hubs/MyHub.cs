using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.API.Hubs
{
    // 100 client 100 request means 100 different object
    // That's why we use static List
    public class MyHub : Hub
    {
        private static List<string> Names { get; set; } = new List<string>();

        private static int ClientCount { get; set; } = 0;

        public static int TeamCount { get; set; } = 7;

        public async Task SendName(string name)
        {
            if (Names.Count > TeamCount)
            {
                await Clients.Caller.SendAsync("Error", $"Team is full!, it can be maximum {TeamCount} people.");
            }

            else
            {
                Names.Add(name);
                // method name (if client is subscribed to this method), message (can be multiple),
                await Clients.All.SendAsync("ReceiveName", name);
            }
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }

        public async override Task OnConnectedAsync()
        {
            ClientCount++;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnConnectedAsync();
        }

        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            ClientCount--;
            await Clients.All.SendAsync("ReceiveClientCount", ClientCount);

            await base.OnDisconnectedAsync(exception);
        }
    }
}
