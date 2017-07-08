using System;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace TelegramService.Filters
{
    /// <summary>
    /// Handle authentication of a specific endpoint
    /// </summary>
    public class AuthorizeKeyAttribute : ActionFilterAttribute
    {
        /// <summary>
        ///     Called when [action executing].
        /// </summary>
        /// <param name="context">The context.</param>
        public override void OnActionExecuting(HttpActionContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var key = context.Request.Headers.Where( x => x.Key == "Authorization").Select(x => x.Value.FirstOrDefault()).FirstOrDefault();
            if (String.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            // Todo: Add key validation
        }
    }
}