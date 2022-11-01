using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.API.Hubs
{
    // 100 client 100 request means 100 different object
    // That's why we use static List
    public class MyHub : Hub
    {
        public static List<string> Names { get; set; } = new List<string>();

        public async Task SendName(string name)
        {
            Names.Add(name);
            // method name (if client is subscribed to this method), message (can be multiple),
            await Clients.All.SendAsync("ReceiveName", name);
        }

        public async Task GetNames()
        {
            await Clients.All.SendAsync("ReceiveNames", Names);
        }
    }
}
