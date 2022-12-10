using Duplex.Core.Common;
using Duplex.Core.Common.Constants;
using Duplex.Core.Contracts;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models;
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
using System.Security.Claims;

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
        private readonly IRankService rankService;
        private readonly IRegionService regionService;

        public AccountController(SignInManager<ApplicationUser> _signInManager,
            UserManager<ApplicationUser> _userManager, IRepository _repo,
            ApplicationDbContext _context, IWebHostEnvironment _webHostEnvironment,
            IRankService _rankService, IRegionService _regionService)
        {
            signInManager = _signInManager;
            userManager = _userManager;
            repo = _repo;
            context = _context;
            webHostEnvironment = _webHostEnvironment;
            rankService = _rankService;
            regionService = _regionService;
        }
        #endregion

        #region Register
        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new RegisterViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                Regions = repo.AllReadonly<Region>().Where(x => x.Name != "Unknown").OrderBy(x => x.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                model.Regions = repo.AllReadonly<Region>().Where(x => x.Name != "Unknown").OrderBy(x => x.Name).ToList();

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
                TempData["UserImage"] = user.Image;
                await signInManager.PasswordSignInAsync(user, model.Password, false, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError("", item.Description);
            }

            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            model.Regions = repo.AllReadonly<Region>();
            return View(model);
        }
        #endregion

        #region Login

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                return View(model);
            }

            var user = await userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var result = await signInManager.PasswordSignInAsync(user, model.Password, false, false);

                if (result.Succeeded)
                {
                    TempData["UserImage"] = user.Image;
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid Login");
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(model);
        }

        #endregion

        #region Logout

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region ExternalLogin

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                                    new { ReturnUrl = returnUrl });

            var properties =
                signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult>
           ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins =
                        (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState
                    .AddModelError(string.Empty, $"Error from external provider: {remoteError}");

                return View("Login", loginViewModel);
            }

            // Get the login information about the user from the external login provider
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState
                    .AddModelError(string.Empty, "Error loading external login information.");

                return View("Login", loginViewModel);
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var applicationUser = repo.AllReadonly<ApplicationUser>().FirstOrDefault(x => x.Email == email);


            // If the user already has a login (i.e if there is a record in AspNetUserLogins
            // table) then sign-in the user with this external login provider
            var signInResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: false, bypassTwoFactor: true);


            if (signInResult.Succeeded)
            {
                if (applicationUser == null)
                {
                    return RedirectToAction("_404", "Error", new { area = "Errors" });
                }
                TempData["UserImage"] = applicationUser.Image;
                return LocalRedirect(returnUrl);

            }
            // If there is no record in AspNetUserLogins table, the user may not have
            // a local account
            else
            {
                // Get the email claim value

                if (email != null)
                {
                    // Create a new user without password if we do not have a user already
                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        var length = info.Principal.FindFirstValue(ClaimTypes.Email).IndexOf('@');
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email)[..length],
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                            RegionId = 12,
                        };

                        var result = await userManager.CreateAsync(user);

                        if (!result.Succeeded)
                        {
                            return RedirectToAction("_404", "Error", new { area = "Errors" });
                        }
                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);
                    await signInManager.SignInAsync(user, isPersistent: false);

                    TempData["UserImage"] = user.Image;

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on admin@duplex.com";

                return View("Error");
            }
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

            var user = await context.Users.Include(x => x.Region).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                ModelState.AddModelError("", "No such user!");
                return RedirectToAction("Index", "Home");
            }

            var rankIds = await context.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var ranks = rankService.GetAllAsync().Result.Where(r => rankIds.Contains(r.Id)).Select(r => r.Name);

            var model = new ProfileViewModel()
            {
                Id = id,
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Image = user.Image,
                Coins = user.Coins,
                Region = user.Region.ToString(),
                Ranks = ranks,
                Wins = user.Wins,
                Loses = user.Loses
            };
            if (user.Region.ToString() == "Unknown")
            {
                model.Regions = regionService.GetAllAsync().Result.Select(x => new RegionModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string id, ProfileViewModel model, IFormFile? file)
        {
            var user = await context.Users.Include(x => x.Region).FirstOrDefaultAsync(x => x.Id == id);

            var senderId = userManager.GetUserId(User);

            if (user == null || senderId != user.Id)
            {
                return RedirectToAction("_403", "Error", new { area = "Errors" });
            }

            if (!ModelState.IsValid)
            {
                model.Id = id;
                model.UserName = user.UserName;
                model.Email = user.Email;
                model.PhoneNumber = model.PhoneNumber?.Trim() == "" ? user.PhoneNumber : model.PhoneNumber?.Trim();
                model.EmailConfirmed = user.EmailConfirmed;
                model.PhoneConfirmed = user.PhoneNumberConfirmed;
                model.TwoFactorEnabled = user.TwoFactorEnabled;
                model.Image = user.Image;
                model.Coins = user.Coins;
                model.Region = user.Region.ToString();
                model.Wins = user.Wins;
                model.Loses = user.Loses;
                return View(model);
            }

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

                var CSPath = webHostEnvironment.ContentRootPath;
                // Load the Service account credentials and define the scope of its access.
                var credential = GoogleCredential.FromFile(GoogleConst.PathToServiceAccountKeyFile)
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
                    Parents = new List<string>() { GoogleConst.DirectoryId }
                };
                // Create a new file on Google Drive

                await using var fsSource = new FileStream(path, FileMode.Open, FileAccess.Read);
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
                TempData["UserImage"] = user.Image;
            }

            user.UserName = model.UserName;
            user.NormalizedUserName = model.UserName.ToUpperInvariant();
            user.PhoneNumber = model.PhoneNumber;
            user.Email = model.Email;
            user.NormalizedEmail = model.Email.ToUpperInvariant();
            user.RegionId = model.RegionId;

            await repo.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Confirm Email

        [HttpGet]
        public IActionResult ConfirmEmail(string? userId)
        {
            //ApplicationUser user;
            //if (userId == null)
            //{
            //    //userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            //    user = await userManager.GetUserAsync(User);
            //}
            //else
            //{
            //    user = await userManager.FindByIdAsync(userId);
            //}

            //string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            //var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, "https");

            //using (var client = new SmtpClient())
            //{
            //    client.Connect(GoogleConst.ClientHost);
            //    client.Authenticate(GoogleConst.FromEmail, GoogleConst.CredentialPassword);

            //    var bodyBuilder = new BodyBuilder()
            //    {
            //        HtmlBody = $"<p>Click here <a src =\"{callbackUrl}\"</a> to confirm your email.</p>",
            //        TextBody = "{callbackUrl}"
            //    };

            //    var message = new MimeMessage()
            //    {
            //        Body = bodyBuilder.ToMessageBody()
            //    };

            //    message.From.Add(new MailboxAddress("Noreply Duplex", GoogleConst.FromEmail));
            //    message.To.Add(new MailboxAddress("Confirmation", user.Email));
            //    message.Subject = "Confirm your email - Duplex.gg";
            //    client.Send(message);
            //    client.Disconnect(true);
            //}

            //return RedirectToAction("Index", "Home");



            //      -V- SENDGRID -V-

            //    ApplicationUser user;
            //    if (userId == null)
            //    {
            //        //userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            //        user = await userManager.GetUserAsync(User);
            //    }
            //    else
            //    {
            //        user = await userManager.FindByIdAsync(userId);
            //    }

            //    string code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, "https");
            //    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");  
            //    string body = string.Empty;

            //    string webRootPath = webHostEnvironment.ContentRootPath;
            //    string path = Path.Combine(webRootPath + @"Content\EmailConfirmation.html");

            //    using (StreamReader reader = new StreamReader(path))
            //    {
            //        body = reader.ReadToEnd();
            //    }
            //    body = body.Replace("{ConfirmationLink}", callbackUrl);
            //    body = body.Replace("{UserName}", user.Email);
            //    MailMessage message = new MailMessage()
            //    {
            //        Body = body,
            //        From = new MailAddress(GoogleConst.FromEmail),
            //        Subject = "Confirm your email"
            //    };
            //    //bool IsSendEmail = SendEmail.EmailSend(user.Email, "Confirm your account", body, true);

            //    var thread = new Thread(() => SendMailThread(message, user.Email, body));
            //    thread.Start();


            return RedirectToAction("Index", "Home");

            //    //return RedirectToAction("Erorr", "_502", new { area = "Errors" });
            //}


            //private static void SendMailThread(MailMessage message, string recipient, string body)
            //{
            //    using (var server = new SmtpClient(GoogleConst.ClientHost))
            //    {
            //        server.Port = int.Parse(GoogleConst.Port);
            //        server.EnableSsl = true;
            //        server.Credentials = new NetworkCredential(GoogleConst.FromEmail, GoogleConst.CredentialPassword);
            //        server.Send(GoogleConst.FromEmail, recipient, "Confirm your email", body);
            //    }
            //}
        }
        #endregion
    }
}
