using GestionProyectos.Engine.Feature.Requirement.Delete.Request;
using GestionProyectos.Engine.Feature.Requirement.Delete.Response;
using GestionProyectos.Shared.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestionProyectos.Engine.Feature.Requirement.Delete
{
    public interface IRequirementDeletionEngine
    {
        OperationResult<RequirementDeletionResponse> Execute(RequirementDeletionRequest request);

    }
}
