import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class ProjectService {
  constructor(private shareService: ShareServiceService) {}
  public getListProject(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-list`;
    return this.shareService.returnHttpClient(url);
  }
  public getCheckNameExist(name): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/CheckNameExist?name=${name}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListByUserId(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-list-by-user-id`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateProject(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/create`;
    return this.shareService.postHttpClient(url, data);
  }
  public SyncProjectFromTfs(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/sync-project-from-tfs/${data.projectId}?url=${data.url}`;
    return this.shareService.postHttpClient(url, data);
  }
  public getListProjectByUserId(userID, filter): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-list-project-by-user?userId=${userID}&filter=${filter}`;
    return this.shareService.returnHttpClient(url);
  }
  public getProjectByID(projectID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-by-id?id=${projectID}`;
    return this.shareService.returnHttpClient(url);
  }
  public PutProject(data, projectID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/edit?id=${projectID}`;
    return this.shareService.putHttpClient(url, data);
  }
  public deleteProject(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/delete?id=${projectId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public getItemProject(projectID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-list-Item-In-Project?idProject=${projectID}`;
    return this.shareService.returnHttpClient(url);
  }
  public getProjectStatistic(projectID, currentUserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/ProjectStatistic?name=${projectID}&currentUserId=${currentUserId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getIssueBydate(projectID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/get-isssue-chart?IdProject=${projectID}`;
    return this.shareService.returnHttpClient(url);
  }
  public ExportFile(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/exportFile`;
    return this.shareService.returnHttpClient(url);
  }
  public getListProjectAdmin(userId, input, dueDate): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/project?input=${input}&dueDate=${dueDate}&userid=${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getChartIssueAdmin(projectID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/chart-issue-of-project?IdProject=${projectID}`;
    return this.shareService.postHttpClient(url, projectID);
  }
  public getUpdateIdTFS(name): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/update-id-tFS?name=${name}`;
    return this.shareService.returnHttpClient(url);
  }
  public getSyncToTFS(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/sync-project`;
    return this.shareService.postHttpClient(url,data);
  }
  public getTfsProject(id, userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/${id}/project-tFS/${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getCategoryStatistic(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/CategoryStatistic?id=${id}`;
    return this.shareService.returnHttpClient(url);
  }
  public getChangeTfs(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/change-tfs/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getDeleteSyncTfs(projectId,userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/${projectId}/project/${userId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public getProject1Statistic(projectId,lastDate): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/project-1-statistic?Id=${projectId}&lastDay=${lastDate}`;
    return this.shareService.returnHttpClient(url);
  }
  public getProjectTfs(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/project-tfs-dto?Id=${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getUserProcessing(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/user-processing-dto`;
    return this.shareService.returnHttpClient(url);
  }
  public getUserDetailsProcessing(userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/project/user-processing-details-dto?userId=${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getTimeOnProject(projectId,time,IsMonth): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/time-on-project/time-on-project/${projectId}?timeSearch=${time}&Month=${IsMonth}`;
    return this.shareService.returnHttpClient(url);
  }
}
