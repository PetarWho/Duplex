using Duplex.Core.Common;
using Duplex.Core.Common.Constants;
using Duplex.Data;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Duplex.Models.Account;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
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
        public async Task<IActionResult> Profile(string id, ProfileViewModel model, IFormFile file)
        {
            FileStream stream;

            if (file != null)
            {
                string webRootPath = webHostEnvironment.ContentRootPath;
                string path = Path.Combine((webRootPath + @"Content\images\"), file.FileName);

                using (Stream fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                    await fileStream.DisposeAsync();
                }

                stream = new FileStream(Path.Combine(path), FileMode.Open);
                await file.CopyToAsync(stream);
                await stream.DisposeAsync();

                var user = await repo.GetByIdAsync<ApplicationUser>(id);

                user.UserName = model.UserName;
                user.PhoneNumber = model.PhoneNumber;


                var CSPath = webHostEnvironment.ContentRootPath;
                // Load the Service account credentials and define the scope of its access.
                var credential = GoogleCredential.FromFile(GoogleDriveConst.PathToServiceAccountKeyFile)
                                .CreateScoped(DriveService.ScopeConstants.Drive);

                // Create the  Drive service.
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential
                });

                // Upload file Metadata
                var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = file.FileName,
                    Parents = new List<string>() { GoogleDriveConst.DirectoryId }
                };
                // Create a new file on Google Drive

                await using (var fsSource = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    // Create a new file, with metadata and stream.
                    var request = service.Files.Create(fileMetadata, fsSource, "image/png, image/jpg, image/jpeg");
                    request.Fields = "*";
                    var results = await request.UploadAsync(CancellationToken.None);

                    if (results.Status == UploadStatus.Failed)
                    {
                        RedirectToAction("Index", "Home");
                        throw new Exception("Upload Failed.");
                    }

                    // the file id of the new file we created
                    var imageId = request.ResponseBody.Id;

                    // edit user's image
                    var imageUrl = @$"https://lh3.googleusercontent.com/d/{imageId}";
                    user.Image = imageUrl;
                    await repo.SaveChangesAsync();
                }

                //await Task.Run(() => UploadFileOnDrive(file, path, user));
            }

            return RedirectToAction("Index", "Home");
        }

        
        #endregion
    }
}
