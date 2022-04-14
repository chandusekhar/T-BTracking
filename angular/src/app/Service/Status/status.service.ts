import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class StatusService {
  constructor(private shareService: ShareServiceService) {}
  public getListStatus(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetList`;
    return this.shareService.returnHttpClient(url);
  }
  public getList(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/only`;
    return this.shareService.returnHttpClient(url);
  }
  public getListStatusWithIssue(IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListStatus?IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListStatusWithIssueByFilter(IdProject, Filter): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListStatus?IdProject=${IdProject}&idCategory=${Filter}`;
    return this.shareService.returnHttpClient(url);
  }
  public checkIssueInProject(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/CheckIssueInProject?idProject=${idProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getIdStatusByName(Name): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status?Name=${Name}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListStatusByCondition(
    IdProject,
    Filter,
    idUser,
    dueDate,
    idCategory,
    skip,
    take
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListStatus?IdProject=${IdProject}
    &Filter=${Filter}&idUser=${idUser}&dueDate=${dueDate}&idCategory=${idCategory}&skip=${skip}&take=${take}`;
    return this.shareService.returnHttpClient(url);
  }
  public getIssuesByStatus(
    IdProject,
    Filter,
    idUser,
    dueDate,
    idCategory,
    skip,
    take,
    statusId
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/issues-by-status/${statusId}?IdProject=${IdProject}
    &Filter=${Filter}&idUser=${idUser}&dueDate=${dueDate}&idCategory=${idCategory}&skip=${skip}&take=${take}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListCategoryBoardByCondition(
    IdProject,
    Filter,
    idUser,
    dueDate,
    idCategory
  ): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListCategoryBoard?IdProject=${IdProject}
    &Filter=${Filter}&idUser=${idUser}&dueDate=${dueDate}&idCategory=${idCategory}`;
    return this.shareService.returnHttpClient(url);
  }
  //api/status/GetListStatus?Filter=bug%204&IdProject=6750CFE0-30AD-21EA-F546-39FCF820EAB6&idUser=1362588917530252&dueDate=2021-06-07&level=string
  public patchStatusAll(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/all-by-list`;
    return this.shareService.putHttpClient(url, data);
  }
  public CreateStatus(input): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status?Name=${input.Name}&NzColor=${input.NzColor}`;
    return this.shareService.postHttpClient(url, input);
  }
  public deleteStatus(statusId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status?id=${statusId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public PutStatus(data, statusID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/${statusID}`;
    return this.shareService.putHttpClient(url, data);
  }
  public getListStatusWithIssueCat(IdProject, idCat): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListStatus?IdProject=${IdProject}&idCategory=${idCat}`;
    return this.shareService.returnHttpClient(url);
  }

  public getListStatusByIdProject(IdProject, take): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/status/GetListStatus?IdProject=${IdProject}&take=${take}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListStatusPie(IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/admin/status?IdProject=${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getStatusStatistics(IdProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/status-statistics/${IdProject}`;
    return this.shareService.returnHttpClient(url);
  }
  public getIssueDto(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/status/issue-dto/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
}
