import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class AssigneeService {
  constructor(private shareService: ShareServiceService) {}

  public GetListAssignee(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee`;
    return this.shareService.returnHttpClient(url);
  }
  public GetListAssigneeByUser(idProject,filter, currentUserId, skip, maxCount): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/async-by-id-user/${currentUserId}?IdProject=${idProject}&Filter=${filter}&skip=${skip}&maxCount=${maxCount}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetListAssigneeByIssue(idIssue): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/async-by-issue-id/${idIssue}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateAssignee(data, idIssue,isDefault): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/by-list?idIssue=${idIssue}&IsDefault=${isDefault}`;
    return this.shareService.postHttpClient(url, data);
  }
  public CheckAssignUser(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/check-user-has-been-assign?IdUser=${data.IdUser}&IdProject=${data.IdProject}`;
    return this.shareService.postHttpClient(url, data);
  }
  public DeleteAssignee(id, idIssue): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/${id}?idIssue=${idIssue}`;
    return this.shareService.deleteHttpClient(url);
  }
  public GetCountAssignee(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/count-assign/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetUsersAssigneeIssueParent(parentId,issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/assignees-by-issue-id?parentId=${parentId}&issueId=${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public CheckIsHaveParent(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/assignee/check-issue-have-parent/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
}
