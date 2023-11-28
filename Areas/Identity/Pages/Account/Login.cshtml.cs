// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

// Your namespace and any other using statements...

namespace Persol_HMS.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<User> _userManager;

        public LoginModel(SignInManager<User> signInManager, ILogger<LoginModel> logger, UserManager<User> userManager)  // Add UserManager to the constructor
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;  // Initialize _userManager
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        private async Task<(bool IsLocked, DateTime? LockEnd)> LockoutUser(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user != null)
            {
				if(user.Attempts % 3 != 0)
				{
					user.Attempts++;
					if (user.Attempts % 3 == 0)
					{
						user.LockEnabled = true;
						user.LockEnd = DateTime.Now.AddMinutes((user.Attempts / 3) * 1);
					}
				}

                await _userManager.UpdateAsync(user);

                return (user.LockEnabled, user.LockEnd);
            }

            return (false, null);
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    // Reset attempts on successful login
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    user.Attempts = 0;
                    user.LockEnabled = false;
                    user.LockEnd = null;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User logged in.");

                    // Redirect based on DepartmentId
                    switch (user.DepartmentId)
                    {
                        case 1:
                            returnUrl = Url.Action("RecordsClerk", "Staff");
                            break;
                        case 2:
                            returnUrl = Url.Action("NurseQueue", "Staff");
                            break;
                        case 3:
                            returnUrl = Url.Action("Doctor", "Staff");
                            break;
                        case 4:
                            returnUrl = Url.Action("LabQueue", "Staff");
                            break;
                        default:
                            returnUrl = Url.Content("~/");
                            break;
                    }

                    return LocalRedirect(returnUrl);
                }
				var lockoutInfo = await LockoutUser(Input.Username);
                if (lockoutInfo.IsLocked)
                {
                    _logger.LogWarning("User account locked out.");
                    if (lockoutInfo.LockEnd.HasValue && lockoutInfo.LockEnd.Value > DateTime.Now)
                    {
                        ModelState.AddModelError(string.Empty, $"User account is locked out until {lockoutInfo.LockEnd}. Please try again later.");
                        return Page();
                    }
                    // If the lockout period has passed, unlock the account
                    var user = await _userManager.FindByNameAsync(Input.Username);
                    user.LockEnabled = false;
                    user.LockEnd = null;
                    await _userManager.UpdateAsync(user);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "User account is locked out. Please try again later.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your username and password.");
                }
            }

            return Page();
        }
    }
}
