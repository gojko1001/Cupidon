using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Utils
{
    // Service that executes before any API specfied by annotation [ServiceFilter(typeof(LogUserActivity))]
    // Should be added as scoped: services.AddScoped<LogUserActivity>();
    // Deprecated, replaced by PrecenceHub onDisconnect to log last user activity time 
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var id = resultContext.HttpContext.User.GetId();
            var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.Complete();
        }
    }
}
