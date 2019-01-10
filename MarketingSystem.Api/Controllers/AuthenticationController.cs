using CMarketingSystemApi.Models;
using MarketingSystem.Model.Entities;
using MarketingSystem.Service.Dtos.Token;
using MarketingSystem.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace MarketingSystem.Api.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILoginHistoryService _historyService;

        public AuthenticationController(IConfiguration configuration, ILoginHistoryService historyService)
        {
            _configuration = configuration;
            _historyService = historyService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            try
            {
                var client = new RestClient(_configuration["IdentityServerUrl"] + "/connect/token");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddParameter(
                    "application/x-www-form-urlencoded",
                    "grant_type=password&" +
                    "client_id=" + _configuration["IdentityServerConfiguration:ClientId"] + "&" +
                    "client_secret=" + _configuration["IdentityServerConfiguration:ClientSecret"] + "&" +
                    "username=" + loginViewModel.Username + "&" +
                    "password=" + loginViewModel.Password + "&" +
                    "scope=" + _configuration["IdentityServerConfiguration:Scope"], ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                var token = JsonConvert.DeserializeObject<TokenDto>(response.Content);
                if (token.access_token != null)
                {
                    var loginHistory = new LoginHistory
                    {
                        Username = loginViewModel.Username,
                        Token = token.access_token
                    };
                    await _historyService.Add(loginHistory);
                    var returnUrl = _configuration["WebManagerUrl"] + "/login/" + token.access_token;
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Login");
            }
            catch (Exception e)
            {
                return RedirectToAction("Login");
            }
        }
    }
}