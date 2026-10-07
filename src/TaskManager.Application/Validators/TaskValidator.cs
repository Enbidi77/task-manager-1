using System;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Validators
{
    public class TaskValidator
    {
        public bool Validate(TaskItem task, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                errorMessage = "Title cannot be empty.";
                return false;
            }

            if (task.Title.Length > 500)
            {
                errorMessage = "Title cannot exceed 500 characters.";
                return false;
            }

            if (task.Description?.Length > 5000)
            {
                errorMessage = "Description cannot exceed 5000 characters.";
                return false;
            }

            if (task.DueDate.HasValue && task.DueDate.Value < new DateTime(1900, 1, 1))
            {
                errorMessage = "Invalid Due Date.";
                return false;
            }

            if (task.EstimatedDuration.HasValue && task.EstimatedDuration.Value.TotalSeconds <= 0)
            {
                errorMessage = "Estimated Duration must be positive.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
