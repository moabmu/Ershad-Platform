using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ModepEduYanbu.Models;
using ModepEduYanbu.Models.AccountViewModels;
using ModepEduYanbu.Services;
using Microsoft.Extensions.Configuration;
using ModepEduYanbu.Data;
using ModepEduYanbu.Repositories;
using AutoMapper;
using Newtonsoft.Json;
using ModepEduYanbu.Repositories.Interfaces;
using System.Diagnostics;
using ModepEduYanbu.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using ModepEduYanbu.DAL.DbContexts;
using System.Net;

namespace ModepEduYanbu.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IPhoneCaller _phoneCaller;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizedPeopleRepo _authorizedPeopleRepo;
        private readonly ISchoolRepo _schoolRepo;
        private readonly IHostingEnvironment _env;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IHostingEnvironment env,
            IEmailSender emailSender,
            ISmsSender smsSender,
            IPhoneCaller phoneCaller,
            ILoggerFactory loggerFactory,
            IConfiguration config,
            ApplicationDbContext context,
            IAuthorizedPeopleRepo authorizedPeopleRepo,
            ISchoolRepo schoolRepo
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _env = env;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _phoneCaller = phoneCaller;
            _logger = loggerFactory.CreateLogger<AccountController>();
            _config = config;
            _context = context;
            _authorizedPeopleRepo = authorizedPeopleRepo;
            _schoolRepo = schoolRepo;
        }

        [Authorize(Roles = "sysowner")]
        public async Task<IActionResult> GetSupervisiors()
        {
            var result = await _userManager.GetUsersInRoleAsync("eduemployee");
            return Json(result.Select(x => x.FullName));
        }

        //[HttpGet]
        //[AllowAnonymous]
        //public async Task<IActionResult> MakePhoneCall(string number = null)
        //{
        //    var result = await _phoneCaller.MakePhoneCallAsync("+966567894439","hello", this.HttpContext);

        //    return result;
        //}

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);

                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation(1, $"User {user.UserName} logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, $"User account {user.UserName} locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "محاولة دخول غير صالحة.");
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/NewUser
        [HttpGet]
        [AllowAnonymous]
        public IActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NewUser(NewUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //ModelState.AddModelError(string.Empty, "خطأ بالبيانات المدخلة");
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.UserIdNo);
            if (user != null)
            {
                ModelState.AddModelError("UserIdNo", "لا يمكن متابعة التسجيل لرقم الهوية المدخل.");
                return View(model);
            }

            return RedirectToAction(nameof(VerifySchools), new { idNo = model.UserIdNo });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifySchools(string idNo = null)
        {
            if (string.IsNullOrEmpty(idNo) || !_authorizedPeopleRepo.IsIdNoValid(idNo))
            {
                return View("Error");
            }

            ViewData["IdNo"] = idNo;

            #region Commented
            //var personSchools = _schoolRepo.GetByPersonIdNo(idNo);
            //if(personSchools == null) // I think it will be null always because lazy nature.
            //{
            //    // Department Employee
            //    return RedirectToAction(nameof(Register), new { idNo = idNo });
            //}
            #endregion

            var personSchools = _schoolRepo.GetAllByAuthorizedPersonIdNo(idNo).ToList();
            if (personSchools == null || personSchools.Count < 1)
            {
                // Department Employee
                return RedirectToAction(nameof(Register), new { idNo = idNo });
            }

            List<VerifySchoolViewModel> model =
                Mapper.Map<IEnumerable<VerifySchoolViewModel>>(personSchools).ToList();

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult VerifySchools(IEnumerable<VerifySchoolViewModel> model, string idNo = null)
        {
            ViewData["IdNo"] = idNo;

            var redirect = true;
            #region Before helper method SchoolsMatchesMinsitryNos()
            //foreach(var item in model)
            //{
            //    var schoolMatches = _schoolRepo.SchoolIdMatchesMinistryNo(item.SchoolId, item.SchoolMinistryNo);
            //    if (!schoolMatches)
            //    {

            //    }
            //}
            #endregion
            SchoolsMatchesMinistryNos(model, (school) =>
            {
                ModelState.AddModelError(string.Empty, $"الرقم الوزاري لمدرسة: {school.SchoolName} خاطيء");
                redirect = false;
            });

            if (!redirect)
            {
                return View(model);
            }
            else
            {
                var schools = JsonConvert.SerializeObject(model);
                return RedirectToAction(nameof(Register), new { idNo = idNo, schoolsList = schools });
            }
        }

        // Helper method
        public void SchoolsMatchesMinistryNos(IEnumerable<VerifySchoolViewModel> schoolsList,
            Action<VerifySchoolViewModel> doIfSchoolNotMatchMinistryNo)
        {
            if (schoolsList != null)
            {
                foreach (var item in schoolsList)
                {
                    var schoolMatches = _schoolRepo.SchoolIdMatchesMinistryNo(item.SchoolId, item.SchoolMinistryNo);
                    if (!schoolMatches)
                    {
                        doIfSchoolNotMatchMinistryNo(item);
                    }
                }
            }
        }

        // Helper method SchoolsMatchesPersonIdNo
        public void SchoolsMatchesPersonIdNo(IEnumerable<VerifySchoolViewModel> schoolsList,
            string personIdNo,
            Action<VerifySchoolViewModel> doIfSchoolNotMatchPersonId)
        {
            if (schoolsList != null)
            {
                foreach (var item in schoolsList)
                {

                    if (!_authorizedPeopleRepo
                        .PersonMatchesSchoolMinistryNo(personIdNo, item.SchoolMinistryNo))
                    {
                        doIfSchoolNotMatchPersonId(item);
                    }
                }
            }
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string schoolsList = null,
            string idNo = null,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            List<VerifySchoolViewModel> schs = null;
            try
            {
                if (!String.IsNullOrEmpty(schoolsList))
                {
                    schs = JsonConvert
                       .DeserializeObject<IEnumerable<VerifySchoolViewModel>>(schoolsList).ToList();
                }
            }
            catch
            {
                return View("Error");
            }

            if (string.IsNullOrEmpty(idNo) || !_authorizedPeopleRepo.IsIdNoValid(idNo))
            {
                return View("Error");
            }
            ViewData["IdNo"] = idNo;
            ViewData["SchoolsList"] = schoolsList;

            #region Before helper methods
            //foreach (var item in schoolsList)
            //{
            //    var schoolMatches =
            //        _schoolRepo.SchoolIdMatchesMinistryNo(item.SchoolId, item.SchoolMinistryNo);
            //    if (!schoolMatches
            //        || !_authorizedPeopleRepo
            //        .PersonMatchesSchoolMinistryNo(idNo, item.SchoolMinistryNo))
            //    {
            //        return View("Error");
            //    }
            //}
            #endregion

            bool returnError = false;
            SchoolsMatchesMinistryNos(schs, (school) => { returnError = true; return; });

            if (!returnError)
            {
                SchoolsMatchesPersonIdNo(schs, idNo, (school) =>
                {
                    returnError = true;
                    return;
                });
            }

            if (returnError)
            {
                return View("Error");
            }

            var person = _authorizedPeopleRepo.GetByIdNo(idNo);
            var model = new RegisterViewModel
            {
                FullName = person.FullName,
                PhoneNumber = person.PhoneNumber
            };
            return View(model);
        }



        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model,
            IEnumerable<VerifySchoolViewModel> schoolsList,
            string idNo = null,
            string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            #region Check sent data
            if (string.IsNullOrEmpty(idNo) || !_authorizedPeopleRepo.IsIdNoValid(idNo))
            {
                return View("Error");
            }
            ViewData["IdNo"] = idNo;
            ViewData["SchoolsList"] = schoolsList;

            bool returnError = false;
            SchoolsMatchesMinistryNos(schoolsList, (school) => { returnError = true; return; });

            if (!returnError)
            {
                SchoolsMatchesPersonIdNo(schoolsList, idNo, (school) =>
                {
                    returnError = true;
                    return;
                });
            }

            if (returnError)
            {
                return View("Error");
            }
            #endregion



            if (ModelState.IsValid)
            {
                var authorizedPerson = _authorizedPeopleRepo.GetByIdNo(idNo);

                var user = new ApplicationUser
                {
                    UserName = idNo,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = $"+966{model.PhoneNumber.Remove(0, 1)}",
                    AddedByUser = authorizedPerson.AddedByUser,
                    RegDate = DateTime.Now
                };
                #region Commented
                //user.PositionTitle = new UserPositionTitle {
                //    PositionTitle = (!_authorizedPeopleRepo.IsEduDepEmployee(user.UserName) ? 
                //    model.PositionTitle : null)//_authorizedPeopleRepo.GetByIdNo(user.UserName).Role.Name)
                //    };
                #endregion
                if (_authorizedPeopleRepo.IsEduDepEmployee(user.UserName))
                {
                    user.PositionTitle = new UserPositionTitle { PositionTitle = model.PositionTitle };
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    #region Commented
                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                    // Send an email with this link
                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //var callbackUrl = Url.Action(nameof(ConfirmEmail), "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);
                    //await _emailSender.SendEmailAsync(model.Email, "Confirm your account",
                    //    $"Please confirm your account by clicking this link: <a href='{callbackUrl}'>link</a>");
                    #endregion

                    #region Commented SMS for Phone Confirmation
                    //// Send an SMS with this link
                    //var smsCode = await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
                    ////var smsCallbackUrl = Url.Action(nameof(ConfirmPhoneNumber), "Account", new { userId = user.Id, code = smsCode}, protocol: HttpContext.Request.Scheme); // Send activation link via SMS
                    //_smsSender.SendSmsAsync(user.PhoneNumber, $"رمز التحقق: {smsCode}");
                    //_logger.LogInformation(3, "User created a new account with password. SMS code is sent.");
                    //return View("RegistrationSuccessful");
                    ////return RedirectToAction(nameof(ConfirmPhoneNumber), new { userId = user.Id });
                    #endregion

                    #region Commented
                    //await _signInManager.SignInAsync(user, isPersistent: false);
                    //_logger.LogInformation(3, "User created a new account with password.");
                    //return RedirectToLocal(returnUrl);
                    #endregion

                    #region Add user to his schools and his role.
                    var roleName = _authorizedPeopleRepo.GetByIdNo(user.UserName).Role.Name;

                    // Add user to his schools
                    if (!_authorizedPeopleRepo.IsEduDepEmployee(user.UserName))
                    {
                        List<School> userSchools = _schoolRepo.GetAllByAuthorizedPersonIdNo(user.UserName).ToList();
                        foreach (var school in userSchools)
                        {
                            _context.Add(new UserSchool { School = school, User = user });
                        }

                        // if user is a principle, then update the school to the new principle.
                        if (roleName == Templates.Roles[3].Name)
                        {
                            foreach (var school in userSchools)
                            {
                                // Get old principle to delete his relationship with the school, and assign a new relationship for the school with the new principle
                                var oldPrinciple = _context.Users.FirstOrDefault(x => x.UserName == school.PrincipleIdNo);
                                if (oldPrinciple == null) // If there is no principle for the school, then update the school with this principle.
                                {
                                    _schoolRepo.UpdatePrincipleIdNo(school, user.UserName);
                                    continue;
                                }

                                // If current principle of the school is the new user, then continue.
                                if (oldPrinciple.Id == user.Id) continue;

                                // Delete the relationshiop between current school and old principle.
                                var userSchoolEntityForOldPrinciple = oldPrinciple.UserSchools.FirstOrDefault(x => x.SchooId == school.SchoolId && x.UserId == oldPrinciple.UserName);
                                if (userSchoolEntityForOldPrinciple == null) goto returnError;
                                _context.Remove(userSchoolEntityForOldPrinciple);

                                // If the current school for old principle is the same as the school for the new principle, then make oldPrinciple.CurrentSchoolId = null
                                if (oldPrinciple.CurrentSchoolId == school.SchoolId) oldPrinciple.CurrentSchoolId = null;

                                // The new user is now the principle of the school
                                _schoolRepo.UpdatePrincipleIdNo(school, user.UserName);
                            }

                            // If principle has one school, then make user.CurrentSchoolId equal that school
                            if (userSchools.Count == 1)
                                user.CurrentSchool = userSchools[0];
                        }

                        // Save changes
                        if ((await _context.SaveChangesAsync() < 1)) // || await _schoolRepo.SaveChangesAsync())
                            goto returnError;
                    }

                    // Add user to his roles  
                    var role = await _roleManager.FindByNameAsync(roleName);
                    var userAddedToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    if (userAddedToRoleResult.Succeeded)
                    {
                        // Remove the new user from the AuthorizedPeople table.
                        try
                        {
                            _authorizedPeopleRepo.RemoveAllRecordsByIdNo(authorizedPerson.IdNo);
                            await _authorizedPeopleRepo.SaveChangesAsync();
                        }
                        catch { }

                        // Sign in user to redirect him to Dashboard index.
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Dashboard");
                    }

                    returnError:
                    this.ModelState.AddModelError(string.Empty, "حدث خطأ أثناء التسجيل، الرجاء المحاولة لاحقاً.");
                    return View("Error");
                    #endregion
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            if (!_env.IsDevelopment())
            {
                ViewData["ReCaptchaKey"] = _config.GetSection("GoogleReCaptcha:key").Value;
            }
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_env.IsDevelopment())
                {
                    if (!GoogleReCaptchaHelper.ReCaptchaPassed(
                 Request.Form["g-recaptcha-response"], // that's how you get it from the Request object
                _config.GetSection("GoogleReCaptcha:secret").Value,
                _logger)
                )
                    {
                        ViewData["ReCaptchaKey"] = _config.GetSection("GoogleReCaptcha:key").Value;
                        ModelState.AddModelError(string.Empty, "فشل اختبار التحقق من صحة الإدخال البشري.");
                        return View(model);
                    }
                }
                var user = await _userManager.FindByNameAsync(model.IdNo);
                if (user == null) // || !(await _userManager.IsPhoneNumberConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=532713
                // Send an SMS with this link
                var resetPasswordCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action(nameof(ResetPassword), "Account", new { userId = user.Id, code = WebUtility.UrlEncode(resetPasswordCode) }, protocol: HttpContext.Request.Scheme);
                await _emailSender.SendEmailAsync(user.Email, "استعادة كلمة المرور",
                    $"رابط استعادة كلمة المرور: <a href=\"{callbackUrl}\">{callbackUrl}</a>");
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByNameAsync(model.IdNo);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            var result = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(model.Code), model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            
            ModelState.AddModelError(string.Empty,
                "حدث خطأ أثناء استعادة كلمة المرور، يرجى التأكد من صحة رقم الهوية وأن طول كلمة المرور هو 6 خانات على الأقل محتوية على أحرف انجليزية كبيرة وصغيرة ورموز وأرقام. في حال استمرار المشكلة يرجى تصوير البيانات الخطأ بالأسفل والتواصل مع الدعم الفني.");
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "رمز التحقق للدخول: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _emailSender.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _smsSender.SendSmsAsync(user.PhoneNumber, message); //_userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);
            if (result.Succeeded)
            {
                return RedirectToLocal(model.ReturnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "الرمز المدخل غير صحيح.");
                return View(model);
            }
        }

        //
        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Testing()
        {
            return View();
        }
        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Dashboard");
            }
        }

        #endregion
    }
}
