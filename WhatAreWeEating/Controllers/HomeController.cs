using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WhatAreWeEating.Context;
using WhatAreWeEating.Models;

namespace WhatAreWeEating.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;

        public HomeController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Dishes = new SelectList(_context.Dishes.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult AddDish(string dishName)
        {
            if (_context.Dishes.Any(d => d.Name == dishName))
            {
                return Json(new { success = false });
            }

            var dish = new Dish { Name = dishName };
            _context.Dishes.Add(dish);
            _context.SaveChanges();

            return Json(new { success = true, dishId = dish.Id, dishName = dish.Name });
        }

        [HttpPost]
        public IActionResult SubmitForm(string name, string email, int dishId)
        {
            bool newUser = false;
            var user = _context.Users.FirstOrDefault(u => u.Name == name && u.Email == email);
            if (user == null)
            {
                user = new User { Name = name, Email = email };
                _context.Users.Add(user);
                _context.SaveChanges();
                newUser = true;
            }

            var userDish = new UserDish { UserId = user.Id, DishId = dishId, Date = DateTime.Now };
            _context.UserDishes.Add(userDish);
            _context.SaveChanges();

            return Json(new { success = true, userId = user.Id, dishId });
        }

        public IActionResult Result(int userId, int dishId)
        {

            var user = _context.Users.Find(userId);
            var dish = _context.Dishes.Find(dishId);

            if (user == null || dish == null)
            {
                ViewBag.Feed = _context.UserDishes
                .OrderByDescending(l => l.Date)
                .Take(20)
                .Select(l => new {
                    l.Date,
                    UserName = l.User.Name,
                    UserEmail = l.User.Email,
                    DishName = l.Dish.Name
                })
                .ToList()
                .Select(l => $"{l.Date:HH:mm dd/MM/yyyy} {l.UserName} ({l.UserEmail}) съел {l.DishName}");

                return View();
            }

            var totalDishCountToday = _context.UserDishes.Count(l => l.DishId == dishId && EF.Functions.DateDiffDay(l.Date, DateTime.Now) == 0);
            var userDishCount = _context.UserDishes.Count(l => l.UserId == user.Id && l.DishId == dishId);
            var userCount = _context.UserDishes.Count(l => l.UserId == user.Id);
            ViewBag.Message = userCount != 1
                ? $"{user.Name}, с возвращением! Вы только что съели {dish.Name}! Всего вы съели {dish.Name} {userDishCount} раз! За сегодня {dish.Name} было съедено {totalDishCountToday} раз!"
                : $"{user.Name}, мы рады приветствовать вас в нашем сообществе! Вы только что съели {dish.Name}! За сегодня {dish.Name} было съедено {totalDishCountToday} раз!";
            ViewBag.Feed = _context.UserDishes
                .OrderByDescending(l => l.Date)
                .Take(20)
                .Select(l => new {
                    l.Date,
                    UserName = l.User.Name,
                    UserEmail = l.User.Email,
                    DishName = l.Dish.Name
                })
                .ToList()
                .Select(l => $"{l.Date:HH:mm dd/MM/yyyy} {l.UserName} ({l.UserEmail}) съел {l.DishName}");

            return View();
        }
    }
}