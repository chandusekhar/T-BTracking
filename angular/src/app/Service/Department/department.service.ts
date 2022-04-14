import { Injectable } from '@angular/core';
import { ShareServiceService } from '../share-service.service';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  constructor( private shareService: ShareServiceService) { }
  public getList(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/get-list`;
    return this.shareService.returnHttpClient(url);
  }
  public getListDepartment(input): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/get-list-department?filter=${input}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateDepartment(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/create`;
    return this.shareService.postHttpClient(url, data);
  }
  public DeleteDepartment(departmentId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/delete?id=${departmentId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public updateDepartment(departmentId,data): Observable<any>{
    const url = `${this.shareService.REST_API_SERVER}/api/department/edit?id=${departmentId}`;
    return this.shareService.putHttpClient(url,data);
  }
  public getListTeam(input,departmentId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/team/team/${departmentId}?input=${input}`;
    return this.shareService.returnHttpClient(url);
  }input
  public getListMemberTeamById(idTeam): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/member-team/by-id-team?IdTeam=${idTeam}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListMemberByIdDepartment(idDepartment): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/member-team/by-id-department?IdDepartment=${idDepartment}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateTeam(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/team`;
    return this.shareService.postHttpClient(url, data);
  }
  public CreateMemberTeam(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/member-team`;
    return this.shareService.postHttpClient(url, data);
  }
  public updateTeam(teamId,data): Observable<any>{
    const url = `${this.shareService.REST_API_SERVER}/api/app/team/${teamId}`;
    return this.shareService.putHttpClient(url,data);
  }
  public DeleteTeam(teamId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/team/${teamId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public getListUserAddMember(idTeam): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/member-team/get-list-user-add-member-team?idTeam=${idTeam}`;
    return this.shareService.returnHttpClient(url);
  }
  public DeleteMemberTeam(memberTeamId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/member-team/by-id/${memberTeamId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public checkManager(UserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/check-manager?UserId=${UserId}`;
    return this.shareService.returnHttpClient(url);
  }
  public checkLeader(UserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/team/check-leader/${UserId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListIssueAll(input,SkipCount,MaxResultCount,idProject,idStatus,idCate,idUser,idDepartment,idTeam,isAss): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/get-list-issue-all?Filter=${input}&SkipCount=${SkipCount}&MaxResultCount=${MaxResultCount}&IdProject=${idProject}&IdStatus=${idStatus}&IdCate=${idCate}&IdUser=${idUser}&IdDepartment=${idDepartment}&IdTeam=${idTeam}&IsAss=${isAss}`;
    return this.shareService.returnHttpClient(url);
  }
  public getNameDepartmentByIdManager(idUser): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/department/get-NameDepartment-by-idManager?IdUser=${idUser}`;
    return this.shareService.returnHttpClient(url);
  }
  public getNameTeamByIdLeader(idUser): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/team/get-name-team-by-leader?IdUser=${idUser}`;
    return this.shareService.returnHttpClient(url);
  }
}
