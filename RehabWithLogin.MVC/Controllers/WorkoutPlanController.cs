using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Controllers
{
    public class WorkoutPlanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutPlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            ViewBag.Workouts = _unitOfWork.WorkoutRepository.Get(x => x.UserEmail == User.Identity.Name);
            return View(_unitOfWork.WorkoutPlanRepository.Get(x => x.UserEmail == User.Identity.Name, null,
                "WorkoutPlanWorkouts.Workout"));
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create(string name, string description)
        {
            var workoutPlan = new WorkoutPlan
            {
                Name = name,
                Description = description,
                UserEmail = User.Identity.Name,
                WorkoutPlanWorkouts = new List<WorkoutPlanWorkout>()
            };
            _unitOfWork.WorkoutPlanRepository.Insert(workoutPlan);
            _unitOfWork.Save();

            return Content($"Workoutplan {name} was successfully added");
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update(int id, [Bind("Id,Name,Description")] WorkoutPlan workoutPlan)
        {
            workoutPlan.UserEmail = User.Identity.Name;
            _unitOfWork.WorkoutPlanRepository.Update(workoutPlan);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpPost]
        public IActionResult DeleteWorkoutPlan(int workoutPlanId)
        {
            var workoutPlan =
                _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == workoutPlanId, null, "WorkoutPlanWorkouts.Workout")
                    .First();
            workoutPlan.WorkoutPlanWorkouts.Clear();
            _unitOfWork.WorkoutPlanRepository.Delete(workoutPlan);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}