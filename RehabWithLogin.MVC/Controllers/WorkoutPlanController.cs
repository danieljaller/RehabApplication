using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Controllers
{
    public class WorkoutPlanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationUser _user;

        public WorkoutPlanController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var c = _userManager.Users.Select(x => x.Email).First();
            ViewBag.Email = c;
            ViewBag.Workouts = _unitOfWork.WorkoutRepository.Get(null, null, "WorkoutPlanWorkouts.WorkoutPlan");
            return View(_unitOfWork.WorkoutPlanRepository.Get(null, null, "WorkoutPlanWorkouts.Workout"));
        }

        [HttpPost]
        public IActionResult Create(string name, string description)
        {
            var workoutPlan = new WorkoutPlan
            {
                Name = name,
                Description = description,
                WorkoutPlanWorkouts = new List<WorkoutPlanWorkout>()
            };
            _unitOfWork.WorkoutPlanRepository.Insert(workoutPlan);
            _unitOfWork.Save();

            return Content($"Workoutplan {name} was successfully added");
        }

        [HttpPost]
        public IActionResult Update(int id, [Bind("Id,Name,Description")] WorkoutPlan workoutPlan)
        {
            _unitOfWork.WorkoutPlanRepository.Update(workoutPlan);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddWorkoutToPlan(int id, int workoutId, string date)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                ModelState.AddModelError("empty string", "No dates were provided");
                return RedirectToAction("Index");
            }
            var workoutPlan = _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == id, null, "WorkoutPlanWorkouts").First();
            var workout = _unitOfWork.WorkoutRepository.GetById(workoutId);
            var dates = date.Split(',');

            foreach (var dateString in dates)
            {

                var convertedDate = Convert.ToDateTime(dateString);
                if (workoutPlan.WorkoutPlanWorkouts.Select(x => x.ScheduledTime).Contains(convertedDate))
                {
                    ModelState.AddModelError(string.Empty, $"The date {dateString}");
                }

                workoutPlan.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = Convert.ToDateTime(dateString)
                });
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.WorkoutPlanRepository.Update(workoutPlan);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult CreateWorkout(int workoutPlanId, string name, string description, string dates)
        {
            var workout = new Workout
            {
                Name = name,
                Description = description,
                WorkoutPlanWorkouts = new List<WorkoutPlanWorkout>()
            };

            var workoutPlan = _unitOfWork.WorkoutPlanRepository.GetById(workoutPlanId);
            var dateArray = dates.Split(',');

            foreach (var date in dateArray)
            {
                workout.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = Convert.ToDateTime(date)
                });
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.WorkoutRepository.Insert(workout);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteWorkoutPlan(int workoutPlanId)
        {

            var workoutPlan =
                _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == workoutPlanId, null, "WorkoutPlanWorkouts.Workout").First();
            workoutPlan.WorkoutPlanWorkouts.Clear();
            _unitOfWork.WorkoutPlanRepository.Delete(workoutPlan);
            _unitOfWork.Save();


            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeleteWorkoutPlanWorkout(int workoutPlanWorkoutId)
        {
            _unitOfWork.WorkoutPlanWorkoutRepository.Delete(workoutPlanWorkoutId);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}