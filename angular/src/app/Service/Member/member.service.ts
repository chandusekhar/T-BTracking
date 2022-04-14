import { Injectable } from '@angular/core';
import { ShareServiceService } from '../share-service.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MemberService {
  constructor(private shareService: ShareServiceService) {}
  public CreateMember(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members`;
    return this.shareService.postHttpClient(url, data);
  }
  public CreateByListMember(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/by-list`;
    return this.shareService.postHttpClient(url, data);
  }
  public getListMemberByIdProject(ProjectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/members-by-project-id/${ProjectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public DeleteMember(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/by-id/${id}`;
    return this.shareService.deleteHttpClient(url);
  }
  public checkUserInProject(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/check-member/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateMemberAnonymous(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/anonymous`;
    return this.shareService.postHttpClientAnonymous(url, data);
  }
  public getMemberStatistics(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/members/member-statistics/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
}
