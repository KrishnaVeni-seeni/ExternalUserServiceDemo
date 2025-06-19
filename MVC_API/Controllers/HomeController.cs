using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVC_API.Models;
using API_Project.Interfaces;
using API_Project.Model;

namespace MVC_API.Controllers;

public class HomeController : Controller
{
    

    private readonly IExternalUserService _userService;

 
    public HomeController(IExternalUserService service)
    {
        _userService = service;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [Route("Users")]
    public async Task<IActionResult> Users()
    {
        var users = await _userService.GetAllUsersAsync();
        return View(users); // Passing only the list
    }

   [HttpPost]
    public async Task<IActionResult> UsersByID(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            ViewBag.Error = "User not Found";
            return View("Index");
        }
        ViewBag.User = user;
        return View("Index");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
