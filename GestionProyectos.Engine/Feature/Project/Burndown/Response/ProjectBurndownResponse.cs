namespace GestionProyectos.Engine.Feature.Project.Burndown.Response;

public class ProjectBurndownResponse
{
    public string ProjectName { get; set; } = string.Empty;
    public decimal TotalEstimatedHours { get; set; }
    public List<ProjectBurndownPoint> Points { get; set; } = new();
}

public class ProjectBurndownPoint
{
    public string Label { get; set; } = string.Empty;
    public decimal Estimated { get; set; }
    public decimal Optimal { get; set; }
    public decimal ProblemLimit { get; set; }
    public decimal Actual { get; set; }
}
