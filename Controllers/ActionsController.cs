﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rozetka.Data;
using Rozetka.Data.Entity;

namespace Rozetka.Controllers
{
    public class ActionsController : Controller
    {
        private readonly DataContext _context;

        public ActionsController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> GetActions()
        {
            // Получаем текущую дату и время
            var currentDateTime = DateTime.Now;

            // Выбираем только акционные товары, у которых акции активны
            var actions = await _context.Actions
                .Include(a => a.Product)
                    .ThenInclude(p => p.ProductType)
                .Include(a => a.Product)
                    .ThenInclude(p => p.Brand)
                .Include(a => a.Product)
                    .ThenInclude(p => p.Childcategory)
                .Include(a => a.Product)
                    .ThenInclude(p => p.ProductImages)
                .Include(a => a.Product)
                    .ThenInclude(p => p.ProductColor)
                .Include(a => a.Product)
                    .ThenInclude(p => p.Reviews)
                .Where(a => a.StartDate <= currentDateTime && a.EndDate >= currentDateTime) // Фильтрация по датам начала и окончания
                .ToListAsync();

            // Получаем ID текущего пользователя для избранных товаров, если пользователь авторизован
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Если пользователь авторизован, добавляем информацию об избранных продуктах
            if (userId != null)
            {
                var favoriteProductIds = await _context.Favorites
                    .Where(f => f.UserId == userId)
                    .Select(f => f.ProductId)
                    .ToListAsync();

                foreach (var action in actions)
                {
                    if (action.Product != null)
                    {
                        action.Product.IsInFavorites = favoriteProductIds.Contains(action.Product.Id);
                    }
                }
            }
            
            return View(actions);
        }

        // GET: Actions
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Actions.Include(a => a.Product);
            return View(await dataContext.ToListAsync());
        }

        // GET: Actions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actions == null)
            {
                return NotFound();
            }

            return View(actions);
        }

        // GET: Actions/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Actions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,StartDate,EndDate,NewPrice,Discount")] Actions actions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(actions);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", actions.ProductId);
            return View(actions);
        }

        // GET: Actions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions.FindAsync(id);
            if (actions == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", actions.ProductId);
            return View(actions);
        }

        // POST: Actions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,StartDate,EndDate,NewPrice,Discount")] Actions actions)
        {
            if (id != actions.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActionsExists(actions.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", actions.ProductId);
            return View(actions);
        }

        // GET: Actions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actions = await _context.Actions
                .Include(a => a.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actions == null)
            {
                return NotFound();
            }

            return View(actions);
        }

        // POST: Actions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actions = await _context.Actions.FindAsync(id);
            if (actions != null)
            {
                _context.Actions.Remove(actions);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActionsExists(int id)
        {
            return _context.Actions.Any(e => e.Id == id);
        }
    }
}
