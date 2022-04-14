import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class HistoryService {

  constructor(private shareService: ShareServiceService) { }

  public GetHistory(IdProject, Startdate,EndDate, IdUser, Action,skipcount, resultcount, issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/result?SkipCount=${skipcount}&MaxResultCount=${resultcount}&projectId=${IdProject}&startDate=${Startdate}&endDate=${EndDate}&idUser=${IdUser}&acc=${Action}&issueId=${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetNameStatus(IdStatus): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/idby-name/${IdStatus}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetHistoryByIssuesId(issuesId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/${issuesId}/history-by-issue`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserByIdProject(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/get-list-user-by-project-id?projectId=${idProject}`;
    return this.shareService.returnHttpClient(url);
  }
}
