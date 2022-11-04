using AspNetCoreSignalR.API.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreSignalR.API.Hubs
{
    // 100 client 100 request means 100 different object
    // That's why we use static List
    public class MyHub : Hub
    {
        private readonly AppDbContext _context;

        public MyHub(AppDbContext context)
        {
            _context = context;
        }

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

        // Groups Add - Remove

        public async Task AddToGroup(string teamName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task RemoveToGroup(string teamName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, teamName);
        }

        public async Task SendNameByGroup(string name, string teamName)
        {
            var team = _context.Teams.Where(x => x.Name == teamName).FirstOrDefault();

            if (team != null)
            {
                // adding to db, same with context.add
                team.Users.Add(new User { Name = name });
            }

            else
            {
                var newTeam = new Team { Name = teamName };

                newTeam.Users.Add(new User { Name = name });

                _context.Teams.Add(newTeam);
            }

            await _context.SaveChangesAsync();

            await Clients.Group(teamName).SendAsync("ReceiveMessageByGroup", name, team.Id);
        }

        // SignalR converts object to json automatically
        public async Task GetNamesByGroup()
        {
            var teams = _context.Teams.Include(x => x.Users).Select(x => new
            {
                teamId = x.Id,
                Users = x.Users.ToList()
            }).ToList();

            await Clients.All.SendAsync("ReceiveNamesByGroup", teams);
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
