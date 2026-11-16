using AutoMapper;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RolesController(RoleManager<ApplicationRoles> roleManager, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string searchValue = "")
        {
            if(string.IsNullOrEmpty(searchValue))
            {
                var roles = await _roleManager.Roles.ToListAsync();
                var rolesView = _mapper.Map<IEnumerable<RolesViewModel>>(roles);
                return View(rolesView);
            }
            else
            {
                var role = _roleManager.FindByNameAsync(searchValue);
                var rolesView = _mapper.Map<RolesViewModel>(role);
                return View(new List<RolesViewModel> { rolesView});
            }
        }

        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RolesViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var mapperRole = _mapper.Map<ApplicationRoles>(model);

                var result = await _roleManager.CreateAsync(mapperRole);

                if (result.Succeeded) 
                    return RedirectToAction("Index");
                else
                {
                    foreach (var role in result.Errors)
                        ModelState.AddModelError(string.Empty, role.Description);
                }
            }
            return View(model);
        }
        
        public async Task<IActionResult> Details(string? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);

            if (role is null)
                return NotFound();

            var roleView = _mapper.Map<RolesViewModel>(role);

            return View(viewName, roleView);
        }
        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(RolesViewModel model, [FromRoute]string id)
        {
            if (model.Id != id)
                return BadRequest();

            if (ModelState.IsValid) 
            {
                try
                {
                    var mapperRole = _mapper.Map<ApplicationRoles>(model);

                    var role = await _roleManager.FindByIdAsync(id);

                    if (role is null)
                        return NotFound();

                    role.Name = mapperRole.Name;

                    if (role.Name != null)
                        role.NormalizedName = mapperRole.Name.ToUpper();
                    else
                        role.NormalizedName = "";

                    var result = await _roleManager.UpdateAsync(role);

                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, item.Description);
                        }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(RolesViewModel model, [FromRoute] string id)
        {
            if (model.Id != id)
                return BadRequest();

            try
            {
                var role = await _roleManager.FindByIdAsync(id);

                if (role is null)
                    return NotFound();

                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }

        public async Task<ActionResult> AddOrRemoveUsers(string roleId)
        {
            if (roleId is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return NotFound();

            var usersInRole = new List<UserInRoleViewModel>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userInRole = new UserInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;

                usersInRole.Add(userInRole);
            }
            ViewBag.RoleId = roleId;
            return View(usersInRole);

        }
        [HttpPost]
        public async Task<ActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {

            if (roleId is null)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role is null)
                return NotFound();

            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await _userManager.FindByIdAsync(user.UserId);

                    if (appUser is null)
                        return NotFound();

                    if (user.IsSelected && !await _userManager.IsInRoleAsync(appUser, role.Name))
                        await _userManager.AddToRoleAsync(appUser, role.Name);
                    else if (!user.IsSelected && await _userManager.IsInRoleAsync(appUser, role.Name))
                        await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                }

                return RedirectToAction(nameof(Update), new {id = roleId});
            }
            return View(users);
        }
    }
}
