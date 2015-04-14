
using System.Web.Http;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;



namespace WebApi.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : BaseController
    {
       
        public AccountController()
        {
          
        }
     

        // POST api/Account/Register
     /*   [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(SisUsuario userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await repository.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }



        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }*/

        
    }
}
