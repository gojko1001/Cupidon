using DatingApp.Extensions;
using DatingApp.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DatingApp.Utils
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
                return;
            var unitOfWork = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var id = resultContext.HttpContext.User.GetId();
            var user = await unitOfWork.UserRepository.GetByIdAsync(id);
            user.LastActive = DateTime.UtcNow;
            await unitOfWork.Complete();
        }
    }
}
