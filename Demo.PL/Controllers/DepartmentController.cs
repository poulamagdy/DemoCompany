using AutoMapper;
using Demo.BLL.Interface;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUintOfWork _uintOfWork;
        private readonly IMapper _mapper;

        public DepartmentController(IUintOfWork uintOfWork, IMapper mapper)
        {
            _uintOfWork = uintOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchValue = "")
        {
            IEnumerable<Department> departments;
            IEnumerable<DepartmentViewModel> departmentViewModels;

            if (string.IsNullOrEmpty(searchValue))
            {
                 departments = await _uintOfWork.DepartmentRepository.GetAllAsync();
                 departmentViewModels = _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);
            }
            else
            {
                departments = _uintOfWork.DepartmentRepository.Search(searchValue);
                departmentViewModels = _mapper.Map<IEnumerable<DepartmentViewModel>>(departments);
            }
            return View(departmentViewModels);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentViewModel model)
        {
            if (ModelState.IsValid) 
            {
                var mapperDepartment = _mapper.Map<Department>(model);

                await _uintOfWork.DepartmentRepository.AddAsync(mapperDepartment);

                await _uintOfWork.CompleteAsync();

                return RedirectToAction("Index");
            }
            else
                return View(model);
        }
        public async Task<IActionResult> Details(int? id, string viewName = "Details")
        {
            if (id is null)
                return BadRequest();

            var department = await _uintOfWork.DepartmentRepository.GetByIdAsync(id.Value);

            if (department == null)
                return NotFound();

            var departViewModel = _mapper.Map<DepartmentViewModel>(department);

            return View(viewName, departViewModel);
        }

        public async Task<IActionResult> Update(int? id)
            => await Details(id, "Update");

        [HttpPost]
        public async Task<IActionResult> Update(DepartmentViewModel model, [FromRoute]int id)
        {
            if (id != model.Id)
                return BadRequest();

            if (ModelState.IsValid) 
            {
                try
                {
                    var mapperDepartmrnt = _mapper.Map<Department>(model);

                    _uintOfWork.DepartmentRepository.Update(mapperDepartmrnt);

                    await _uintOfWork.CompleteAsync();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int? id)
            => await Details(id, "Delete");

        [HttpPost]
        public async Task<IActionResult> Delete(DepartmentViewModel model, [FromRoute]int id)
        {
            if (id != model.Id)
                return BadRequest();
            try
            {
                var mapperDepartment = _mapper.Map<Department>(model);
                _uintOfWork.DepartmentRepository.Delete(mapperDepartment);
                await _uintOfWork.CompleteAsync();
                return RedirectToAction("Index");
            }
            catch (Exception Ex)
            {

                ModelState.AddModelError(string.Empty, Ex.Message);
            }
            return View(model);
        }
    }
}
