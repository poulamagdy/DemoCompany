using AutoMapper;
using Demo.BLL.Interface;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUintOfWork _uintOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUintOfWork uintOfWork, IMapper mapper)
        {
            _uintOfWork = uintOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchValue = "")
        {
            IEnumerable<Employee> employees;
            IEnumerable<EmployeeViewModel> employeesViewModels;
            if (string.IsNullOrEmpty(searchValue))
            {
                employees = await _uintOfWork.EmployeeRepository.GetAllAsync();
                employeesViewModels = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }
            else
            {
                employees = _uintOfWork.EmployeeRepository.Search(searchValue);
                employeesViewModels = _mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
            }
            return View(employeesViewModels);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.department = await _uintOfWork.DepartmentRepository.GetAllAsync();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel model)
        {
            ModelState["Department"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                var mapperEmployee = _mapper.Map<Employee>(model);
                mapperEmployee.ImageName = DocementSettings.UploadFile(model.Image, "Images");
                await _uintOfWork.EmployeeRepository.AddAsync(mapperEmployee);
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

            var employee = await _uintOfWork.EmployeeRepository.GetByIdAsync(id);

            if (viewName == "Details" || viewName == "Delete")
            {
                var department = await _uintOfWork.DepartmentRepository.GetByIdAsync(employee.DepartmentId);

                ViewBag.departmentName = department.Name;
            }
            if (employee is null)
                return NotFound();

            var employeeViewModel = _mapper.Map<EmployeeViewModel>(employee);

            return View(viewName, employeeViewModel);
        }

        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.department = await _uintOfWork.DepartmentRepository.GetAllAsync();
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(EmployeeViewModel model, [FromRoute] int id)
        {
            if (id != model.Id)
                return BadRequest();

            ModelState["Department"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid;
            if (ModelState.IsValid)
            {
                string oldImageName = model.ImageName;
                try
                {
                    var mapperEmployee = _mapper.Map<Employee>(model);
                   
                    mapperEmployee.ImageName = DocementSettings.UploadFile(model.Image, "Images");
                    _uintOfWork.EmployeeRepository.Update(mapperEmployee);
                    int result = await _uintOfWork.CompleteAsync();

                    if (result > 0)
                        DocementSettings.DeleteFile(oldImageName, "Images");
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
        public async Task<IActionResult> Delete(EmployeeViewModel model, [FromRoute]int id)
        {
            if (id != model.Id)
                return BadRequest();

            try
            {
                var mapperEmployee = _mapper.Map<Employee>(model);
                _uintOfWork.EmployeeRepository.Delete(mapperEmployee);
                int result = await _uintOfWork.CompleteAsync();

                if (result > 0)
                    DocementSettings.DeleteFile(model.ImageName, "Images");
                
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
    }
}

