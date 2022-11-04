using AspNetCoreSignalR.API.Models;

namespace AspNetCoreSignalR.API.Hubs
{
    public interface IProductHub
    {
        Task ReceiveProduct(Product p);
    }
}
