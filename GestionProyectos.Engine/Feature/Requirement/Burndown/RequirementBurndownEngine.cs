using GestionProyectos.Data;
using GestionProyectos.Engine.Feature.Project.Burndown.Response;
using GestionProyectos.Shared.Enums;
using GestionProyectos.Shared.Message;

namespace GestionProyectos.Engine.Feature.Requirement.Burndown;

public class RequirementBurndownEngine : IRequirementBurndownEngine
{
    private readonly DataDbContext dbContext;

    public RequirementBurndownEngine(DataDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public OperationResult<ProjectBurndownResponse> GetBurndown(long requirementId)
    {
        try
        {
            var requirement = dbContext.Requirement
                .FirstOrDefault(r => r.Id == requirementId && r.RowStatus == (short)RowStatus.Active);

            if (requirement == null)
                return OperationResult<ProjectBurndownResponse>.CreateFailureResult("El requerimiento no existe.");

            var tasks = dbContext.Task
                .Where(t => t.RequirementId == requirementId && t.RowStatus == (short)RowStatus.Active)
                .ToList();

            var totalEstimated = tasks.Sum(t => t.TimeEstimationHours);
            var taskIds = tasks.Select(t => t.Id).ToList();

            var hoursByDay = dbContext.TimeLog
                .Where(t => taskIds.Contains(t.TaskId) && t.RowStatus == (short)RowStatus.Active)
                .AsEnumerable()
                .GroupBy(t => t.ExecutionDate.Date)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.UsedHours));

            var start = requirement.StartDate.Date;
            var plannedEnd = requirement.EndDate?.Date ?? DateTime.UtcNow.Date;
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
                ProjectName = requirement.Description,
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
