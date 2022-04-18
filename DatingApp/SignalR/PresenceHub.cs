using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _unitOfWork;

        public PresenceHub(PresenceTracker tracker, IUnitOfWork unitOfWork)
        {
            _tracker = tracker;
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOnline)
            {
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());
                UpdateUserLastActive();
            }

            var currentUsers = await _tracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            if (isOffline)
            {
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
                UpdateUserLastActive();
            }

            await base.OnDisconnectedAsync(exception);
        }


        private async void UpdateUserLastActive()
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsername(Context.User.GetUsername());
            if (user != null)
            {
                user.LastActive = DateTime.UtcNow;
                await _unitOfWork.Complete();
            }
        }
    }
}
