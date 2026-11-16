using AutoMapper;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
	[Authorize(Roles = "Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
			_userManager = userManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchValue = "")
		{
            if (string.IsNullOrEmpty(searchValue))
			{
				var users = await _userManager.Users.Select(
														u => new UserViewModel()
														{
															Id = u.Id,
															UserName = u.UserName,
															Email = u.Email,
															Roles = _userManager.GetRolesAsync(u).Result
														}).ToListAsync();
                return View(users);
            }
			else
			{
				var users = await _userManager.FindByNameAsync(searchValue);
				var mapperUser = new UserViewModel()
				{
					Id = users.Id,
					UserName = users.UserName,
					Email = users.Email,
					Roles = _userManager.GetRolesAsync(users).Result
				};
                return View(new List<UserViewModel> { mapperUser } );
            }
		}
		public async Task<IActionResult> Details(string? id , string viewName = "Details")
		{
			if (id is null)
				return BadRequest();

			var user = await _userManager.FindByIdAsync(id);

			if (user is null)
				return NotFound();

			var userViewMode = _mapper.Map<UserViewModel>(user);

			return View(viewName, userViewMode);
		}
        public async Task<IActionResult> Update(string? id, string viewName = "Update")
        {
            return await Details(id, viewName);
        }
		[HttpPost]
        public async Task<IActionResult> Update(UserViewModel model, [FromRoute]string id)
        {
            if(model.Id != id)
				return BadRequest();

			if (ModelState.IsValid)
			{
				try
				{
                    var user = await _userManager.FindByIdAsync(id);
                    user.UserName = model.UserName;

                    if (user.UserName != null)
                        user.NormalizedUserName = model.UserName.ToUpper();
                    else
                        user.NormalizedUserName = "";

                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                    {
                        foreach (var item in result.Errors)
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
		public async Task<IActionResult> Delete(string? id)
		{
			return await Details(id, "Delete");
		}
		[HttpPost]
        public async Task<IActionResult> Delete(UserViewModel model, [FromRoute]string id)
        {
            if (model.Id != id)
				return BadRequest();

			try
			{
				var user = await _userManager.FindByIdAsync(id);

				if (user is null)
					return NotFound();

				var result = await _userManager.DeleteAsync(user);

				if (result.Succeeded)
					return RedirectToAction("Index");
				else
				{
					foreach (var item in result.Errors)
						ModelState.AddModelError(string.Empty, item.Description);
				}

			}
			catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
			}

			return View(model);
        }
    }
}
