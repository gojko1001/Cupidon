using DatingApp.Data;
using DatingApp.Extensions;
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
            var repository = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var id = resultContext.HttpContext.User.GetId();
            var user = await repository.GetByIdAsync(id);
            user.LastActive = DateTime.Now;
            await repository.SaveAllAsync();
        }
    }
}
