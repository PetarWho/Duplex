using Duplex.Core.Common;
using Duplex.Core.Common.Constants;
using Duplex.Data;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Duplex.Models.Account;
using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Duplex.Controllers
{
    public class AccountController : Controller
    {
        #region Injection
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IRepository repo;
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(SignInManager<ApplicationUser> _signInManager, 
            UserManager<ApplicationUser> _userManager, IRepository _repo, 
            ApplicationDbContext _context, IWebHostEnvironment _webHostEnvironment)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            repo = _repo;
            context = _context;
            webHostEnvironment = _webHostEnvironment;
        }
        #endregion

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel()
            {
                Regions = repo.AllReadonly<Region>()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Regions = repo.AllReadonly<Region>();
                return View(model);
            }

            var user = new ApplicationUser()
            {
                Email = model.Email,
                UserName = model.UserName,
                RegionId = model.RegionId
            };

            var result = await userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            model.Regions = repo.AllReadonly<Region>();
            return View(model);
        }
        #endregion

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid Login");
            return View(model);
        }

        #endregion

        #region Logout

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Management

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await context.Users.Include(x => x.Region).FirstAsync(x => x.Id == id);

            var model = new ProfileViewModel()
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneConfirmed = user.PhoneNumberConfirmed,
                Image = user.Image,
                Coins = user.Coins,
                Region = user.Region.ToString(),
                Wins = user.Wins,
                Loses = user.Loses
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string id, IFormFile file)
        {
            FileStream stream;

            if (file != null)
            {
                string webRootPath = webHostEnvironment.ContentRootPath;
                string path = Path.Combine((webRootPath + @"Content\images\"), file.FileName);

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                stream = new FileStream(Path.Combine(path), FileMode.Open);
                file.CopyTo(stream);

                await Task.Run(() => Upload(stream, file.FileName));

                stream.Close();
            }

            return RedirectToAction("Index", "Home");
        }


        private static string ApiKey = FirebaseInfo.APIKey;
        private static string Bucket = FirebaseInfo.Bucket;
        private static string AuthEmail = FirebaseInfo.AuthEmail;
        private static string AuthPassword = FirebaseInfo.AuthPassword;
        public async void Upload(FileStream stream, string fileName)
        {
            var auth = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            var a = await auth.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword);

            //Use the cancellation token source to cancel the upload midway
            var cancellation = new CancellationTokenSource();

            var task = new FirebaseStorage(Bucket, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
                ThrowOnCancel = true  // when you cancel the upload, exception is thrown. Default = false 
            })
                .Child("images")
                .Child(fileName)
                .PutAsync(stream, cancellation.Token);

            try
            {
                // Error during upload will be thrown when task is awaited
                string link = await task;
            }
            catch(Exception ex)
            {
                
            }
        }

        #endregion
    }
}
