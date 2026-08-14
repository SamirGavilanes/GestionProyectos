using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Burndown.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Project.Burndown;

public class ProjectBurndownEngine : IProjectBurndownEngine
{
    private readonly DataDbContext dbContext;

    public ProjectBurndownEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<ProjectBurndownResponse> GetBurndown(long projectId)
    {
        try
        {
            var project = dbContext.Project
                .FirstOrDefault(p => p.Id == projectId && p.RowStatus == (short)RowStatus.Active);

            if (project == null)
                return OperationResult<ProjectBurndownResponse>.CreateFailureResult("El proyecto no existe.");

            var requirementIds = dbContext.Requirement
                .Where(r => r.ProjectId == projectId && r.RowStatus == (short)RowStatus.Active)
                .Select(r => r.Id)
                .ToList();

            var tasks = dbContext.Task
                .Where(t => requirementIds.Contains(t.RequirementId) && t.RowStatus == (short)RowStatus.Active)
                .ToList();

            var totalEstimated = tasks.Sum(t => t.TimeEstimationHours);
            var taskIds = tasks.Select(t => t.Id).ToList();

            var hoursByDay = dbContext.TimeLog
                .Where(t => taskIds.Contains(t.TaskId) && t.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(t => t.ExecutionDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.UsedHours));

            var start = project.StartDate.Date;
            var plannedEnd = project.EndDate?.Date ?? DateTime.UtcNow.Date;
            var end = plannedEnd;
            var today = DateTime.UtcNow.Date;
            if (today > end)
                end = today;
            if (end < start)
                end = start;

            var totalDays = (end - start).Days;
            if (totalDays < 1)
                totalDays = 1;

            var points = new List<ProjectBurndownPoint>();
            decimal cumulativeLogged = 0;

            for (var day = start; day <= end; day = day.AddDays(1))
            {
                var dayIndex = (day - start).Days;
                var progress = (decimal)dayIndex / totalDays;
                var remainingFactor = 1m - progress;

                cumulativeLogged += hoursByDay.GetValueOrDefault(day, 0);

                points.Add(new ProjectBurndownPoint
                {
                    Label = day.ToString("dd/MM"),
                    Estimated = Math.Round(totalEstimated * remainingFactor, 2),
                    Optimal = Math.Round(totalEstimated * 0.9m * remainingFactor, 2),
                    ProblemLimit = Math.Round(totalEstimated * 1.15m * remainingFactor, 2),
                    Actual = Math.Max(0, Math.Round(totalEstimated - cumulativeLogged, 2))
                });
            }

            return OperationResult<ProjectBurndownResponse>.CreateSuccessResult(new ProjectBurndownResponse
            {
                ProjectName = project.Description,
                TotalEstimatedHours = totalEstimated,
                Points = points
            });
        }
        catch (Exception ex)
        {
            return OperationResult<ProjectBurndownResponse>.CreateFailureResult(ex);
        }
    }
}
