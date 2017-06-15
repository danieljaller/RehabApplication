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
    public class WorkoutController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index(int id)
        {
            var workout = _unitOfWork.WorkoutRepository.Get(x => x.Id == id, null, "WorkoutExercises.Exercise.Tool")
                .First();
            var workoutVM = new WorkoutViewModel
            {
                Id = workout.Id,
                Name = workout.Name,
                Description = workout.Description,
                UserEmail = workout.UserEmail,
                WorkoutPlanWorkouts = _unitOfWork.WorkoutPlanWorkoutRepository.Get(x => x.WorkoutId == workout.Id)
                    .ToList(),
                WorkoutExercises = workout.WorkoutExercises,
                Tools = _unitOfWork.ToolRepository.Get(x => x.UserEmail == workout.UserEmail),
                Exercises = _unitOfWork.ExerciseRepository.Get(x => x.UserEmail == workout.UserEmail)
            };
            return View(workoutVM);
        }

        [Authorize]
        [HttpPut]
        public IActionResult ToggleIsDone(int workoutPlanWorkoutId, bool isDone)
        {
            var workoutPlanWorkout = _unitOfWork.WorkoutPlanWorkoutRepository.GetById(workoutPlanWorkoutId);
            workoutPlanWorkout.IsDone = isDone;
            _unitOfWork.WorkoutPlanWorkoutRepository.Update(workoutPlanWorkout);
            _unitOfWork.Save();
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddWorkoutToPlan(int id, int workoutId, string date, string time)
        {
            if (string.IsNullOrWhiteSpace(date))
            {
                ModelState.AddModelError(string.Empty, "No dates were provided");
                return RedirectToAction("Index", "WorkoutPlan");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                ModelState.AddModelError(string.Empty, "No time was provided");
                return RedirectToAction("Index", "WorkoutPlan");
            }
            var workoutPlan = _unitOfWork.WorkoutPlanRepository.Get(x => x.Id == id, null, "WorkoutPlanWorkouts")
                .First();
            var workout = _unitOfWork.WorkoutRepository.GetById(workoutId);
            var dates = date.Split(',');

            foreach (var day in dates)
            {
                var dateString = day;
                dateString += $" {time}";
                var convertedDate = DateTime.Parse(dateString,
                    CultureInfo.InvariantCulture);
                if (workoutPlan.WorkoutPlanWorkouts.Select(x => x.ScheduledTime).Contains(convertedDate))
                    ModelState.AddModelError(string.Empty, $"The date {dateString}");

                workoutPlan.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = convertedDate
                });
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.WorkoutPlanRepository.Update(workoutPlan);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index", "WorkoutPlan");
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateWorkout(int workoutPlanId, string name, string description, string dates, string time)
        {
            if (string.IsNullOrWhiteSpace(dates))
            {
                ModelState.AddModelError(string.Empty, "No dates were provided");
                return RedirectToAction("Index", "WorkoutPlan");
            }
            if (string.IsNullOrWhiteSpace(time))
            {
                ModelState.AddModelError(string.Empty, "No time was provided");
                return RedirectToAction("Index", "WorkoutPlan");
            }
            var workout = new Workout
            {
                UserEmail = User.Identity.Name,
                Name = name,
                Description = description,
                WorkoutPlanWorkouts = new List<WorkoutPlanWorkout>()
            };

            var workoutPlan = _unitOfWork.WorkoutPlanRepository.GetById(workoutPlanId);
            var dateArray = dates.Split(',');

            foreach (var date in dateArray)
            {
                var dateString = date;
                dateString += $" {time}";
                var convertedDate = DateTime.Parse(dateString,
                    CultureInfo.InvariantCulture);
                workout.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = convertedDate
                });
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.WorkoutRepository.Insert(workout);
                _unitOfWork.Save();
            }

            return RedirectToAction("Index", "WorkoutPlan");
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteWorkoutPlanWorkout(int workoutPlanWorkoutId)
        {
            _unitOfWork.WorkoutPlanWorkoutRepository.Delete(workoutPlanWorkoutId);
            _unitOfWork.Save();
            return RedirectToAction("Index", "WorkoutPlan");
        }
    }
}