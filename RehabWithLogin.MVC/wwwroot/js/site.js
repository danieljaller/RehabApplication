function reloadWindow() {
    location.reload(true);
};

$("#addWPForm").submit(function () {
    $.ajax({
        method: "POST",
        url: "/WorkoutPlan/Create",
        data: $("#addWPForm").serialize()
    }).success(function (msg) {
        $("#addWPForm").hide();
        $("#formMessage").html(msg).show();
        $("#addWPForm").each(function () {
            this.reset();
        });
    });
    return false;
});

$("#closeWPModal").click(function () {
    $("#addWPForm").removeData();
    $("#addWPForm").show();
    $("#formMessage").hide();
});

$(".input-group.date").datepicker({
    startDate: "datetime.Today",
    todayBtn: "linked",
    clearBtn: true,
    multidate: true,
    calendarWeeks: true,
    datesDisabled: ["05/06/2017", "05/21/2017"]
});

function getYoutubeId(url) {
    const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
    const match = url.match(regExp);

    if (match && match[2].length === 11) {
        return match[2];
    } else {
        return "error";
    }
};

function videoModal(url) {
    const videoId = getYoutubeId(url);
    $("#exerciseModal-body").hide();
    $("#exerciseModal").find(".modal-title").html("Video");
    $("#videoHtml")
        .html(
        `<iframe width="560" height="315" src="//www.youtube.com/embed/${videoId
        }" frameborder="0" allowfullscreen></iframe>`);
    $("#videoHtml").show();
    $("#exerciseModal").modal("show");
};

function showExerciseInfo(exerciseInfo, title) {
    $("#exerciseModal").find(".modal-title").html(title);
    $("#videoHtml").hide();
    $("#exerciseModal-body").html(exerciseInfo);
    $("#exerciseModal-body").show();
    $("#exerciseModal").modal("show");
};

function updateNotes(workoutExerciseId, previousNotes) {
    $("#updateNoteModal").modal("show");
    handleNotesForm(workoutExerciseId, previousNotes);
};

function handleNotesForm(workoutExerciseId, previousNotes) {
    $("#updateNotesInput").html(previousNotes);
    $("#updateNotesForm").submit(function () {
        $.ajax({
            method: "POST",
            url: "/Exercise/UpdateExerciseNotes",
            data: { 'id': workoutExerciseId, 'notes': $("#updateNotesInput").val() }
        }).success(function () {
            $("#updateNotesForm").each(function () {
                this.reset();
            });
        });
    });
};

function deleteWorkoutPlan(workoutPlanId, workoutPlanName) {
    $("#deleteModal").modal("show");
    openDeleteModal(workoutPlanId, workoutPlanName);
};

function openDeleteModal(workoutPlanId, workoutPlanName) {
    $("#deleteModal").find(".modal-body").html(`Are you sure you want to delete ${workoutPlanName}?`);
    $("#confirmDeleteBtn").click(function () {
        $.ajax({
            method: "POST",
            url: "/WorkoutPlan/DeleteWorkoutPlan",
            data: { 'workoutPlanId': workoutPlanId }
        }).success(function () {
            $("#deleteModal").modal("toggle");
            reloadWindow();
        });
    });
};


function deleteWorkoutPlanWorkout(workoutPlanWorkoutId, workoutName) {
    $("#deleteModal").modal("show");
    $("#deleteModal").find(".modal-body").html(`Are you sure you want to delete ${workoutName}?`);
    $("#confirmDeleteBtn").click(function () {
        $.ajax({
            method: "POST",
            url: "/Workout/DeleteWorkoutPlanWorkout",
            data: { 'workoutPlanWorkoutId': workoutPlanWorkoutId }
        }).success(function () {
            $("#deleteModal").modal("toggle");
            reloadWindow();
        });
    });
};

function deleteExerciseFromWorkout(workoutExerciseId, exerciseName, workoutId) {
    $("#deleteExerciseModal").modal("show");
    $("#deleteExerciseModal").find(".modal-body")
        .html(`Are you sure you want to delete ${exerciseName} from the workout?`);
    $("#confirmExerciseDeleteBtn").click(function () {
        $.ajax({
            method: "POST",
            url: "/Exercise/DeleteExercise",
            data: { 'workoutExerciseId': workoutExerciseId, 'workoutId': workoutId }
        }).success(function () {
            reloadWindow();
        });
    });
};

$("#addExistingExercise").submit(function () {
    getExerciseInfo($("#addExistingExerciseSelect").val());
    return false;
});

function getExerciseInfo(exerciseId) {
    $.ajax({
        method: "GET",
        url: "/Exercise/ExerciseInfo",
        data: { 'exerciseId': exerciseId }
    }).success(function (exercise) {
        $("#existingExerciseName").html(exercise.name);
        $("#existingExerciseTool").html(exercise.tool);
        $("#existingExerciseDescription").html(exercise.description);
    });
};

$("#addExistingExerciseForm").submit(function () {
    $.ajax({
        method: "POST",
        url: "/Exercise/AddExistingExerciseToWorkout",
        data: {
            'exerciseId': $("#addExistingExerciseSelect").val(),
            'workoutId': $("#workoutId").val(),
            'notes': $("#existingExerciseNotes").val(),
            'reps': $("#existingExerciseReps").val(),
            'sets': $("#existingExerciseSets").val(),
            'resistance': $("#existingExerciseResistance").val()
        }
    }).done(function () {
        reloadWindow();
    });
});

function exportCalendar(wpId, wpName) {
    $("#exportConfirmationModal-label").html("Loading...");
    $("#exportConfirmationModal").find(".modal-body")
        .html('<div class="loader">Loading...</div>');
    $("#exportConfirmationModal").modal("show");
    $.ajax({
        method: "POST",
        url: "/Calendar/ExportToGoogleCalendar",
        data: { 'workoutPlanId': wpId }
    }).success(function () {
        $("#exportConfirmationModal-label").html("Success!");
        $("#exportConfirmationModal").find(".modal-body")
            .html(wpName + " was successfully exported to Google Calendar");
    }).fail(function () {
        $("#exportConfirmationModal-label").html("Fail!");
        $("#exportConfirmationModal").find(".modal-body")
            .html("The workout plan doesn't contain any workouts!");
    });
};

$(".done").on("click",
    function () {
        const id = $(this).prev().val();
        const parent = $(this).parent("p");
        if (parent.hasClass("line-through")) {
            $.ajax({
                method: "PUT",
                url: "/Workout/ToggleIsDone",
                data: { 'workoutPlanWorkoutId': id, 'isDone': false }
            }).done(function () {
                parent.removeClass("line-through");
            });

        } else {
            $.ajax({
                method: "PUT",
                url: "/Workout/ToggleIsDone",
                data: { 'workoutPlanWorkoutId': id, 'isDone': true }
            }).done(function () {
                parent.addClass("line-through");
            });
        };
    });