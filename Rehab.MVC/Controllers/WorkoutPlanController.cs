﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Rehab.MVC.DAL;
using Rehab.MVC.Models;

namespace Rehab.MVC.Controllers
{
    public class WorkoutPlanController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public WorkoutPlanController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

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
            var workoutPlan = _unitOfWork.WorkoutPlanRepository.GetById(id);
            workoutPlan.WorkoutPlanWorkouts = new List<WorkoutPlanWorkout>();
            var workout = _unitOfWork.WorkoutRepository.GetById(workoutId);
            var dates = date.Split(',');

            foreach (var dateString in dates)
                workoutPlan.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = Convert.ToDateTime(dateString)
                });

            _unitOfWork.WorkoutPlanRepository.Update(workoutPlan);
            _unitOfWork.Save();

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
                workout.WorkoutPlanWorkouts.Add(new WorkoutPlanWorkout
                {
                    WorkoutPlan = workoutPlan,
                    Workout = workout,
                    ScheduledTime = Convert.ToDateTime(date)
                });
            _unitOfWork.WorkoutRepository.Insert(workout);
            _unitOfWork.Save();

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