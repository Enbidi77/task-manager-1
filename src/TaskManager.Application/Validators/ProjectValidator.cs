using TaskManager.Core.Entities;

namespace TaskManager.Application.Validators
{
    public class ProjectValidator
    {
        public bool Validate(Project project, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(project.Name))
            {
                errorMessage = "Project Name cannot be empty.";
                return false;
            }

            if (project.Name.Length > 200)
            {
                errorMessage = "Project Name cannot exceed 200 characters.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(project.Color) && !IsHexColor(project.Color))
            {
                errorMessage = "Color must be a valid hex string.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private bool IsHexColor(string color)
        {
            if (color.StartsWith("#"))
            {
                return (color.Length == 7 || color.Length == 9) && 
                       System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9a-fA-F]+$");
            }
            return false;
        }
    }
}
