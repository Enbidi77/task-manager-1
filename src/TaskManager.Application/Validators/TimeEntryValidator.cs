using System;
using TaskManager.Core.Entities;

namespace TaskManager.Application.Validators
{
    public class TimeEntryValidator
    {
        public bool Validate(TimeEntry entry, out string errorMessage)
        {
            if (entry.StartTime == default)
            {
                errorMessage = "StartTime is required.";
                return false;
            }

            if (entry.EndTime.HasValue && entry.EndTime.Value <= entry.StartTime)
            {
                errorMessage = "EndTime must be after StartTime.";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
