﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RehabWithLogin.MVC.Data;
using RehabWithLogin.MVC.Models;

namespace RehabWithLogin.MVC.Controllers
{
    public class ExerciseController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExerciseController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateExerciseNotes(int id, string notes)
        {
            var workoutExercise = _unitOfWork.WorkoutExerciseRepository.Get(x => x.Id == id, null, "Workout").First();
            workoutExercise.Notes = notes;
            _unitOfWork.WorkoutExerciseRepository.Update(workoutExercise);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Workout", new { id = workoutExercise.Workout.Id });
        }

        [Authorize]
        [HttpPost]
        public IActionResult NewExercise(int workoutId, string name, string description, int? toolId,
            string toolName, string videoUrl, int reps, int sets, string resistance, string notes)
        {
            var tool = toolId != null
                ? _unitOfWork.ToolRepository.GetById(toolId)
                : new Tool { Name = toolName, UserEmail = User.Identity.Name };
            var exercise = new Exercise
            {
                UserEmail = User.Identity.Name,
                Name = name,
                Description = description,
                Tool = tool,
                VideoUrl = videoUrl,
                WorkoutExercises = new List<WorkoutExercise>()
            };

            var workout = _unitOfWork.WorkoutRepository.GetById(workoutId);

            exercise.WorkoutExercises.Add(new WorkoutExercise
            {
                Exercise = exercise,
                Workout = workout,
                Resistance = resistance,
                Reps = reps,
                Sets = sets,
                Notes = notes
            });
            _unitOfWork.ExerciseRepository.Insert(exercise);
            _unitOfWork.Save();
            return RedirectToAction("Index", "Workout", new { id = workoutId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult UpdateExercise(int workoutExerciseId, int exerciseId, string name, string description,
            int? toolId,
            string toolName, string videoUrl, int reps, int sets, string resistance, string notes)
        {
            var tool = toolId != null ? _unitOfWork.ToolRepository.GetById(toolId) : new Tool { Name = toolName };
            var exercise = _unitOfWork.ExerciseRepository.GetById(exerciseId);
            var workoutExercise = _unitOfWork.WorkoutExerciseRepository
                .Get(x => x.Id == workoutExerciseId, null, "Workout")
                .First();

            exercise.Description = description;
            exercise.Name = name;
            exercise.Tool = tool;
            exercise.VideoUrl = videoUrl;

            workoutExercise.Notes = notes;
            workoutExercise.Resistance = resistance;
            workoutExercise.Sets = sets;
            workoutExercise.Reps = reps;

            _unitOfWork.ExerciseRepository.Update(exercise);
            _unitOfWork.WorkoutExerciseRepository.Update(workoutExercise);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Workout", new { id = workoutExercise.Workout.Id });
        }

        [Authorize]
        [HttpPost]
        public IActionResult DeleteExercise(int workoutExerciseId, int workoutId)
        {
            _unitOfWork.WorkoutExerciseRepository.Delete(workoutExerciseId);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Workout", new { id = workoutId });
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddExistingExerciseToWorkout(int workoutId, int exerciseId, string notes, int reps,
            int sets, string resistance)
        {
            var workout = _unitOfWork.WorkoutRepository.Get(x => x.Id == workoutId, null, "WorkoutExercises").First();
            var exercise = _unitOfWork.ExerciseRepository.GetById(exerciseId);

            workout.WorkoutExercises.Add(new WorkoutExercise
            {
                Exercise = exercise,
                Workout = workout,
                Notes = notes,
                Resistance = resistance,
                Reps = reps,
                Sets = sets
            });

            _unitOfWork.WorkoutRepository.Update(workout);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Workout", new { id = workoutId });
        }

        [Authorize]
        public IActionResult ExerciseInfo(int exerciseId)
        {
            if (exerciseId != 0)
            {
                var exercise = _unitOfWork.ExerciseRepository.Get(x => x.Id == exerciseId, null, "Tool").First();
                var exerciseVM = new ExerciseViewModel
                {
                    Name = exercise.Name,
                    Tool = exercise.Tool.Name,
                    Description = exercise.Description
                };

                return Json(exerciseVM);
            }
            return BadRequest();
        }
    }
}
