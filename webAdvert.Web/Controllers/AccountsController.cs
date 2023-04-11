using Amazon.Extensions.CognitoAuthentication;
using Amazon.AspNetCore.Identity.Cognito;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using webAdvert.Web.Models.Accounts;

namespace webAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly UserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, 
            UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.pool = pool;
        }

        [HttpGet]
        public async Task<IActionResult> Signup() 
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model) 
        {
            if (ModelState.IsValid) 
            {
                var user = pool.GetUser(model.Email);
                if (user?.Status != null) 
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);

                var createdUser = await userManager.CreateAsync(user, model.Password);
                if (createdUser.Succeeded) 
                {
                    RedirectToAction("Confirm");
                }

            }
            return View(model);
        }
    }
}
