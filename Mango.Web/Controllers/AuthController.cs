﻿using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto? response = await _authService.LoginAsync(obj);

            if (response.isSuccess == true)
            {
                LoginResponseDto? loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                Debug.WriteLine(loginResponseDto?.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.Message);
                return View(obj);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>() {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };
            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _authService.RegisterAsync(obj);
            ResponseDto assignRole;

            if (result != null && result.isSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(obj);
                if (assignRole != null && assignRole.isSuccess)
                {
                    TempData["sucess"] = "Registration Successful";
                    return RedirectToAction(nameof(Login));
                }
            }

            var roleList = new List<SelectListItem>() {
                new SelectListItem{Text=SD.RoleAdmin, Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer, Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View(obj);
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
