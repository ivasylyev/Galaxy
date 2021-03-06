using Codestellation.Galaxy.Infrastructure.Emisstar;
using Nejdb.Bson;

namespace Codestellation.Galaxy.ServiceManager.Events
{
    [Synchronized]
    public class DeploymentDeletedEvent 
    {
        public readonly ObjectId DeploymentId;

        public DeploymentDeletedEvent(ObjectId deploymentId)
        {
            DeploymentId = deploymentId;
        }
    }
}