using BugTracking.Issues;
using System;
using System.Collections.Generic;
using System.Text;

namespace BugTracking.Azures
{
    public class DataAzureTFS
    {
        public class WITCreatedTemp
        {
            public int witId { get; set; }
            public Guid issueId { get; set; }
        }

        public class WIT
        {
            public int id { get; set; }
            public int rev { get; set; }
            public Dictionary<string, dynamic> fields { get; set; }
        }

        public class WITs
        {
            public int count { get; set; }
            public List<WIT> value { get; set; }
        }
        public class WITsDto
        {
            public int count { get; set; }
            public List<WITDto> value { get; set; }
            public List<string> state { get; set; }
            public List<string> type { get; set; }
        }
        public class WITDto
        {
            public string name { get; set; }
            public string state { get; set; }
            public string type { get; set; }
            public string reason { get; set; }
            public string CreatedDate { get; set; }
            public string AreaPath { get; set; }
            public string TeamProject { get; set; }
            public string IterationPath { get; set; }

        }
        public class WiqlQuery
        {
            public string query { get; set; }

        }
        public class workItemId
        {
            public int id { get; set; }
            public string url { get; set; }
        }
        public class workItemsQuery
        {
            public List<workItemId> workItems { get; set; }
        }
        public class WebHookJson
        {
            public string publisherId { get; set; } = "tfs";
            public string eventType { get; set; }
            public string resourceVersion { get; set; } = "1.0-preview.1";
            public string consumerId { get; set; } = "webHooks";
            public string consumerActionId { get; set; } = "httpRequest";
            public publisherInputs publisherInputs { get; set; }
            public consumerInputs consumerInputs { get; set; }

        }
        public class publisherInputs
        {
            public string areaPath { get; set; } = "";
            public string projectId { get; set; }
            public string workItemType { get; set; } = "";
        }
        public class consumerInputs
        {
            public string url { get; set; }
        }
        public class DataProjectTfs
        {
            public string name { get; set; }
            public string description { get; set; }
            public capabilities capabilities { get; set; }

        }
        public class capabilities
        {
            public versioncontrol versioncontrol { get; set; }
            public processTemplate processTemplate { get; set; }
        }
        public class versioncontrol
        {
            public string sourceControlType { get; set; }
        }
        public class processTemplate
        {
            public string templateTypeId { get; set; }
        }
        public class dataSyncProject
        {
            public string name { get; set; }
            public string witType { get; set; }
            public List<IssuesDto> listIssueSync { get; set; }
            public string description { get; set; }
        }
        public class ProjectsTFS
        {
            public int count { get; set; }
            public List<ProjectTfs> value { get; set; }
        }
        public class ProjectTfs
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class CommentWit
        {
            public int workItemId { get; set; }
            public int id { get; set; }
            public int version { get; set; }
            public string text { get; set; }
        }
        public class UpdateChangesFromTfs
        {
            public resource resource { get; set; }
        }
        public class revision
        {
            public Dictionary<string, dynamic> fields { get; set; }
            public commentVersionRef commentVersionRef { get; set; }
        }
        public class commentVersionRef
        {
            public int commentId { get; set; }
            public int version { get; set; }
        }
        public class resource
        {
            public int id { get; set; }
            public int workItemId { get; set; }
            public int rev { get; set; }
            public revisedBy revisedBy { get; set; }
            public revision revision { get; set; }
            public Dictionary<string, object> fields { get; set; }
            public commentVersionRef commentVersionRef { get; set; }
        }
        public class valueObjectChange
        {
            public string newValue { get; set; }
            public string oldValue { get; set; }
        }
        public class revisedBy
        {
            public string id { get; set; }
            public string displayName { get; set; }
            public string uniqueName { get; set; }
        }
        public class commentsTfs
        {
            public List<commentTfs> comments { get; set; }
        }
        public class commentTfs
        {
            public int workItemId { get; set; }
            public int id { get; set; }
            public string text { get; set; }
            public Dictionary<string,dynamic> createdBy { get; set; }
            public string uniqueName { get; set; }
        }
        public class WitState
        {
            public int count { get; set; }
            public List<state> value { get; set; }
        }
        public class state
        {
            public string name { get; set; }
            public string color { get; set; }
            public string category { get; set; }
        }
        public class commentContent
        {
            public string text { get; set; }
        }
        public class commentCreated
        {
            public int id { get; set; }
        }
        public class value
        {
            public string rel { get; set; } = "System.LinkTypes.Hierarchy-Reverse";
            public string url { get; set; }
            public attributes attributes { get; set; }
        }
        public class attributes
        {
            public bool isLocked { get; set; } = false;
            public string name { get; set; } = "Parent";
        }
    }
}
