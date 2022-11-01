﻿using AspNetCoreSignalR.API.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AspNetCoreSignalR.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<MyHub> _hubContext;

        public NotificationController(IHubContext<MyHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpGet("{teamCount}")]
        public async Task<IActionResult> SetTeamCount(int teamCount)
        {
            await _hubContext.Clients.All.SendAsync("Notify", $"Team is going to be {teamCount}.");

            return Ok();
        }
    }
}
