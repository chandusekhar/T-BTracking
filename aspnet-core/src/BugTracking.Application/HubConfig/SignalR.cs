using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;


namespace BugTracking.Hub
{
    [Authorize("BugTracking.Users")]
    public class SignalR : Microsoft.AspNetCore.SignalR.Hub
    {
        public void calendar(string UserId)
        {
            Clients.All.SendAsync("calendar", UserId);
        }
        public void DrawerChat(string conversationId, string UserId)
        {
            Clients.All.SendAsync("DrawerChat", conversationId, UserId);
        }
        public void Typing(string conversationId, string conten, string UserId)
        {
            Clients.All.SendAsync("Typing", conversationId, conten, UserId);

        }
        public void SignIn()
        {
            Clients.Caller.SendAsync("SignIn");
        }
        public void UnreadMini(string conversationId, string UserID)
        {
            Clients.All.SendAsync("UnreadMini", conversationId, UserID);
        }
        public void MiniChat(string conversationId, string UserID)
        {
            Clients.All.SendAsync("MiniChat", conversationId, UserID);
        }
        public void CountUnread(int count)
        {
            Clients.All.SendAsync("CountUnread", count);
        }
        public void UpdateUnread(string conversationId, string UserID)
        {
            Clients.All.SendAsync("UpdateUnread", conversationId, UserID);
        }
        public void SendMessage(string conversationId, string UserID)
        {
            Clients.All.SendAsync("SendMessage", conversationId, UserID);
        }
        public void addMemberGroup(string conversationId, string UserID)
        {
            Clients.All.SendAsync("addMemberGroup", conversationId, UserID);
        }
        public void removeMemberGroup(string conversationId, string UserID)
        {
            Clients.All.SendAsync("removeMemberGroup", conversationId, UserID);
        }
        public void seenMessage(string conversationId, string UserID)
        {
            Clients.All.SendAsync("seenMessage", conversationId, UserID);
        }
        public void DisposeMessageModal(bool value, string UserID)
        {
            Clients.All.SendAsync("DisposeMessageModal", value, UserID);
        }
        public void OnlyReloadDB(string connectionId)
        {
            Clients.Client(connectionId).SendAsync("OnlyReloadDB");
        }
        public void ReloadDB()
        {
            Clients.All.SendAsync("ReloadDB");
        }
        public void ReloadBugAssign(string idProject, string[] idUser)
        {
            Clients.All.SendAsync("ReloadBugAssign", idProject, idUser);
        }
        public void ReloadCloseAssignee(string idProject)
        {
            Clients.All.SendAsync("ReloadCloseAssignee", idProject);
        }
        public void ReloadHistory(string idProject)
        {
            Clients.All.SendAsync("ReloadHistory", idProject);
        }
        public void ReloadProject(string[] userId)
        {
            Clients.All.SendAsync("ReloadProject", userId);
        }
        public void ReloadProjectName()
        {
            Clients.All.SendAsync("ReloadProjectName");
        }
        public void ReloadBoardByIdProject(string idProject)
        {
            Clients.All.SendAsync("ReloadBoardByIdProject", idProject);
        }
        public void ReloadBoardByIdProjectAndCaller(object issueDto)
        {
            Clients.All.SendAsync("ReloadBoardByIdProjectAndCaller", issueDto);
        }
        public void ReloadBoardByIdProjectAndCallerCategory(object issueDto)
        {
            Clients.All.SendAsync("ReloadBoardByIdProjectAndCallerCategory", issueDto);
        }
        public void ReloadBoardByIdProjectAndCallerCreate(object issueDto)
        {
            Clients.All.SendAsync("ReloadBoardByIdProjectAndCallerCreate", issueDto);
        }
        public void ReloadBoardByIdProjectAndCallerDelete(object issueDto)
        {
            Clients.All.SendAsync("ReloadBoardByIdProjectAndCallerDelete", issueDto);
        }
        public void ReloadBoardByIdProjectAddAssignee(object issueDto)
        {
            Clients.All.SendAsync("ReloadBoardByIdProjectAddAssignee", issueDto);
        }
        public void ReloadNotify(string IssueId)
        {
            Clients.All.SendAsync("ReloadNotify", IssueId);
        }
        public void ReloadComments(string IssueId)
        {
            Clients.All.SendAsync("ReloadComments", IssueId);
        }
        public void ChangesFromTfs()
        {
            Clients.All.SendAsync("ChangesFromTfs");
        }
        public override Task OnConnectedAsync()
        {
            var ConnectionID = Context.ConnectionId;
            Clients.Client(ConnectionID).SendAsync("GetIdConnection", ConnectionID);
            return base.OnConnectedAsync();
        }
    }
}
