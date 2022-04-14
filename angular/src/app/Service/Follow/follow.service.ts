import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class FollowService {
  constructor(private shareService: ShareServiceService) {}

  public CreateFollow(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow`;
    return this.shareService.postHttpClient(url, data);
  }
  public CreateListFollow(data, idIssue): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/by-list?idIssue=${idIssue}`;
    return this.shareService.postHttpClient(url, data);
  }
  public DeleteFollow(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/${id}`;
    return this.shareService.deleteHttpClient(url);
  }
  public GetFollowById(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/${id}/by-id`;
    return this.shareService.returnHttpClient(url);
  }
  public GetListFollowByIssueId(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/by-issue-id/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }

  public GetListFollowByUserIdProjectId(userId, projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/by-user-id-project-id?userId=${userId}&projectId=${projectId}`;
    return this.shareService.returnHttpClient(url);
  }

  public getListFollowByIssueId(userId, issueId, projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender?idUser=${userId}&idIssue=${issueId}`;
    return this.shareService.returnHttpClient(url);
  }

  public CheckFollowUser(issueId, userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/by-user-issue-id?issueId=${issueId}&userId=${userId}`;
    return this.shareService.returnHttpClient(url);
  }

  public getListFollowByIssueIdWithDate(date, userId, issueId, projectId): Observable<any> {
    if (date) {
      if (issueId) {
        const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender?date=${date}&idUser=${userId}&idIssue=${issueId}`;
        return this.shareService.returnHttpClient(url);
      } else {
        const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender?date=${date}&idUser=${userId}`;
        return this.shareService.returnHttpClient(url);
      }
    } else {
      if (issueId) {
        const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender?idUser=${userId}&idIssue=${issueId}`;
        return this.shareService.returnHttpClient(url);
      } else {
        const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender?idUser=${userId}`;
        return this.shareService.returnHttpClient(url);
      }
    }
  }
  public getListFollow(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/${projectId}/calender`;
    return this.shareService.returnHttpClient(url);
  }

  public CheckFollow(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/check-follow/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetHistoryByIssue(idIssue): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/issue-updates/${idIssue}?index=0&pageSize=20&filterName=default&filterValue=default`;
    return this.shareService.returnHttpClient(url);
  }
  public getIssueDetail(idIssue): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/issue-detail/${idIssue}`;
    return this.shareService.returnHttpClient(url);
  }
  public issuefollowByUserId(UserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/by-user-id/${UserId}`;
    return this.shareService.returnHttpClient(url);
  }
}
