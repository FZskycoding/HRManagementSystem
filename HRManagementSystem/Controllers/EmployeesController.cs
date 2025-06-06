﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HRManagementSystem.Data;
using HRManagementSystem.Models;

namespace HRManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index(string? search, string? departmentFilter)
        {
            // 傳遞部門下拉選單資料到 View
            ViewData["DepartmentFilter"] = new SelectList(
                new[] { "篩選全部部門", "A", "B", "C", "D", "E" },
                departmentFilter
            );

            var employees = from e in _context.Employees select e;

            // 關鍵字搜尋
            if (!string.IsNullOrEmpty(search))
            {
                employees = employees.Where(e => e.Name.Contains(search) || e.Email.Contains(search));
            }


            // 部門篩選（忽略 ALL）
            if (!string.IsNullOrEmpty(departmentFilter) && departmentFilter != "篩選全部部門")
            {
                employees = employees.Where(e => e.Department == departmentFilter);
            }

            return View(await employees.ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewData["Departments"] = new SelectList(new[] { "A", "B", "C", "D", "E" });
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Email,Department")] Employee employee, IFormFile? photo)
        {
            // 檢查是否有重複的姓名
            bool isNameExists = await _context.Employees.AnyAsync(e => e.Name == employee.Name);
            if (isNameExists)
            {
                ModelState.AddModelError("Name", "已有相同的員工姓名，請重新輸入。");
            }
            // 檢查是否有重複的 Email
            bool isEmailExists = await _context.Employees.AnyAsync(e => e.Email == employee.Email);
            if (isEmailExists)
            {
                ModelState.AddModelError("Email", "此 Email 已被使用，請重新輸入。");
            }

            if (ModelState.IsValid)
            {
                // 上傳照片邏輯
                if(photo != null && photo.Length > 0)
                {
                    // 建立唯一檔案名稱（避免覆蓋）
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                    // 確保目錄存在
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    var filePath = Path.Combine(uploadPath, fileName);

                    // 寫入檔案
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }

                    // 記錄檔案名稱到資料庫
                    employee.Photo = fileName;

                }
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["Departments"] = new SelectList(new[] { "A", "B", "C", "D", "E" });
            return View(employee);
        }

        // GET: Employees/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["Departments"] = new SelectList(new[] { "A", "B", "C", "D", "E" }, employee.Department);

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Department")] Employee employee, IFormFile? photo)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }
            // 檢查是否有重複的姓名
            bool isNameExists = await _context.Employees.AnyAsync(e => e.Id != employee.Id && e.Name == employee.Name);
            if (isNameExists)
            {
                ModelState.AddModelError("Name", "已有相同的員工姓名，請重新輸入。");
            }
            // 檢查是否有重複的 Email
            bool isEmailExists = await _context.Employees.AnyAsync(e => e.Id != employee.Id && e.Email == employee.Email);
            if (isEmailExists)
            {
                ModelState.AddModelError("Email", "此 Email 已被使用，請重新輸入。");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 更新基本欄位
                    existingEmployee.Name = employee.Name;
                    existingEmployee.Email = employee.Email;
                    existingEmployee.Department = employee.Department;

                    

                    // 若有上傳新照片
                    if (photo != null && photo.Length > 0)
                    {
                        // 刪除舊照片（可選）
                        if (!string.IsNullOrEmpty(existingEmployee.Photo))
                        {
                            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", existingEmployee.Photo);
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }

                        // 儲存新照片
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        var filePath = Path.Combine(uploadPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(stream);
                        }

                        existingEmployee.Photo = fileName;
                    }

                    _context.Update(existingEmployee);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["Departments"] = new SelectList(new[] { "A", "B", "C", "D", "E" }, employee.Department);

            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
