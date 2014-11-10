using System.Collections.Generic;
using System.Linq;
using Codestellation.Galaxy.Domain;
using Nejdb.Bson;

namespace Codestellation.Galaxy.WebEnd.Models
{
    public class DeploymentListItemModel
    {
        public DeploymentListItemModel(Deployment deployment, IEnumerable<KeyValuePair<ObjectId, string>> allFeeds)
        {
            Id = deployment.Id;
            Group = deployment.Group;
            Status = deployment.Status;
            DisplayName = deployment.GetDisplayName();
            FeedName = allFeeds.SingleOrDefault(x =>  x.Key == deployment.FeedId).Value;
        }

        public ObjectId Id { get; set; }

        public string Group { get; set; }
        public string DisplayName { get; set; }
        public string FeedName { get; set; }
        public string Status { get; set; }
    }
}