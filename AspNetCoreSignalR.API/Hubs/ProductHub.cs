using AspNetCoreSignalR.API.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.API.Hubs
{
    public class ProductHub : Hub<IProductHub>
    {
        public async Task SendProduct(Product p)
        {
            await Clients.All.ReceiveProduct(p);
        }
    }
}
