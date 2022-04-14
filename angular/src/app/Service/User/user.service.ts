import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private shareService: ShareServiceService) {}
  public getListUser(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserByIdProject(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user-by-project-id?projectId=${idProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserByAdmin(input): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/user?input=${input}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserAll(input, skipCount, MaxResultCount): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/dashboard/user-all?Filter=${input}&SkipCount=${skipCount}&MaxResultCount=${MaxResultCount}`;
    return this.shareService.returnHttpClient(url);
  }
  public getProfile(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-profile`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserAddProject(ProjectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user-add-project?ProjectId=${ProjectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserAddAssign(ProjectId, issueID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user-add-assign-issue?projectId=${ProjectId}&issueID=${issueID}`;
    return this.shareService.returnHttpClient(url);
  }
  public updateProfile(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/update-profile?Name=${data.name}&Email=${data.email}`;
    return this.shareService.putHttpClient(url, data);
  }
  public getListUserbyIdProject(skipCount, pageSize, input, idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user-by-id-project-search?Filter=${input}&SkipCount=${skipCount}&MaxResultCount=${pageSize}&IdProject=${idProject}`;
    return this.shareService.returnHttpClient(url);
  }

  public getListCreaterIDByIdProject(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-createrIssue-by-project-id?projectId=${idProject}`;
    return this.shareService.returnHttpClient(url);
  }

  //searchat
  public searchAllUser(input): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/dashboard/user-all?Filter=${input}`;
    return this.shareService.returnHttpClient(url);
  }
  public DeleteUser(UserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/user-by-id/${UserId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public GetTMTProject(Host, Collection, Pat): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/t-mTProjects?Host=${Host}&Collection=${Collection}&PAT=${Pat}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetHost(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/host`;
    return this.shareService.returnHttpClient(url);
  }
  public updateHost(data, Id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/host/${Id}`;
    return this.shareService.putHttpClient(url, data);
  }
  public CreateHost(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/host`;
    return this.shareService.postHttpClient(url, data);
  }
  public GetProjectTFS(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/project`;
    return this.shareService.returnHttpClient(url);
  }
  public GetProjectId(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/${id}/project-id`;
    return this.shareService.returnHttpClient(url);
  }
  public CheckAdmin(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/check-admin`;
    return this.shareService.returnHttpClient(url);
  }
  public GetUserInforTfs(userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/user-infor-tfs?UserId=${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetUserById(userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-profile-by-Id?id=${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateUpdateUserTfs(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/user-infor-tfs`;
    return this.shareService.postHttpClient(url, data);
  }
}
