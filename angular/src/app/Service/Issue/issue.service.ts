import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class IssueService {
  public totalPageNumberOfIssue;
  constructor(private shareService: ShareServiceService) {}
  public getListIssueByNameStatus(NameStatus, IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-by-name-status?NameStatus=${NameStatus}&IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getCheckName(projectid, name): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/check-name/${projectid}?name=${name}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListIssue(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue`;
    return this.shareService.returnHttpClient(url);
  }
  public getListIssueCreatedByMe(filter, idStatus, idProject, Assign, take, skip): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-created-by-me?Filter=${filter}&IdStatus=${idStatus}&Idproject=${idProject}&IsAss=${Assign}&take=${take}&skip=${skip}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListEnumPriority(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/enum-priority-value`;
    return this.shareService.returnHttpClient(url);
  }
  public getListEnumLevel(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/enum-level-value`;
    return this.shareService.returnHttpClient(url);
  }
  public patchIssueAll(data, id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/all-by-list?IdIssue=${id}`;
    return this.shareService.putHttpClient(url, data);
  }
  public patchIssueDate(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/due-date?ID=${data.ID}&date=${data.dueDate}`;
    return this.shareService.putHttpClient(url, data);
  }
  public patchIssueCategory(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/category?ID=${data.id}&category=${data.category}`;
    return this.shareService.putHttpClient(url, data);
  }
  public patchIssueNullDueDate(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/due-date?ID=${data.ID}`;
    return this.shareService.putHttpClient(url, data);
  }
  public patchIssueReceivedDate(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/received-date`;
    return this.shareService.putHttpClient(url, data);
  }
  public getListIssueByIdProject(Filter, IdProject, Status): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-by-id-project?Filter=${Filter}&IdProject=${IdProject}&idStatus=${Status}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListIssueByIdCategory(IdCategory, IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-by-id-category?IdCategory=${IdCategory}&IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }

  public searchIssueByName(Text, IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-by-id-project?Filter=${Text}&IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }

  public PaginationIssue(number): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue?SkipCount=${number}&MaxResultCount=1`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateIssue(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue`;
    return this.shareService.postHttpClient(url, data);
  }

  public getListIssueResult(
    SkipCount,
    MaxResultCount,
    IdProject,
    Filter,
    Idcategory,
    StatusName,
    userName
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-by-id-project?Filter=${Filter}&SkipCount=${SkipCount}&MaxResultCount=${MaxResultCount}&IdProject=${IdProject}&IdCategory=${Idcategory}&idStatus=${StatusName}&createrId=${userName} `;
    return this.shareService.returnHttpClient(url);
  }
  public CreateIssueWithAttachment(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-attachment`;
    return this.shareService.postHttpClient(url, data);
  }
  public getIssueById(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/by-id/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public DeleteIssue(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/by-id/${issueId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public updateIssueWithAttachment(issueId, data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue?Id=${issueId}`;
    return this.shareService.putHttpClient(url, data);
  }
  public CheckAdmin(idUser): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/check-role-admin?idUser=${idUser}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetUserHistory(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/user-audit-logs/${idProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public updateIssueByStatus(statusId, issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issues-by-status-id?StatusId=${statusId}&Id=${issueId}`;
    return this.shareService.putHttpClient(url, '');
  }
  public updateAssigneeIssues(issueId, data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/assingee/${issueId}`;
    return this.shareService.putHttpClient(url, data);
  }
  public GetProjectId(IssueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/project-id-by-issue/${IssueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public GetListIssueAdmin(
    Input,
    IdProject,
    IdUser,
    idStatus,
    idCate,
    dueDate,
    idUserAssign
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/issue-all?input=${Input}&IdProject=${IdProject}&IdStatus=${idStatus}&IdCate=${idCate}&dueDate=${dueDate}&IdUser=${IdUser}&idUserAssign=${idUserAssign}`;
    return this.shareService.returnHttpClient(url);
  }
  public updateIssueByCategory(catId, issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/category?ID=${issueId}&category=${catId}`;
    return this.shareService.putHttpClient(url, '');
  }

  public GetIssuesNoParent(projectId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-no-parent/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
  public updateIssueByPriority(input, issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/priority?ID=${issueId}&Priority=${input}`;
    return this.shareService.putHttpClient(url, '');
  }
  public updateIssueByLevel(input, issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/issue-level?ID=${issueId}&IssueLevel=${input}`;
    return this.shareService.putHttpClient(url, '');
  }
  public getWITsByProjectTfs(
    projectId,
    take: number,
    skip: number,
    filter: string,
    state: string,
    type: string
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/w-iTs/${projectId}?take=${take}&skip=${skip}&filter=${filter}&state=${state}&type=${type}`;
    return this.shareService.returnHttpClient(url);
  }
  public getIssueByName(projectId, name): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/issue/id-by-name/${projectId}?name=${name}`;
    return this.shareService.returnHttpClient(url);
  }
}
