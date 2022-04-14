import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {
  private events = [];

  /*
  private events = [
    {id: 1, title: 'All Day Event', start: '2017-02-01'},
    {id: 2, title: 'Long Event', start: '2017-02-07', end: '2017-02-10'},
    {id: 3, title: 'Repeating Event', start: '2017-02-09T16:00:00'},
    {id: 3, title: 'test', start: '2017-02-20T07:00:00'},
  ];
  */

  public IssueIdCalendar : string;
  public ProjectIdCalendar : string;
  private people = [
    {id: 1, name: "PERSONA 1"},
    {id: 2, name: "PERSONA 2"},
    {id: 3, name: "PERSONA 3"},
    {id: 4, name: "PERSONA 4"},
    {id: 5, name: "PERSONA 5"},
  ]

  public eventData = new BehaviorSubject(this.events);

  constructor(
    private shareService:ShareServiceService) {}

  getEvents(): Observable<any[]> {
    return this.eventData.asObservable();
  }


  addEvent(event) {
    const newEvent = {id: 5, title: event.event.title, start: event.event.start, end: event.event.end};
    this.events.push(newEvent);
    this.eventData.next([...this.events]);
  }

  removeEventById(id:number): Observable<any[]>  {

    return this.eventData.asObservable();
  }

  getPeople(): Promise<any[]> {
    return Promise.all(this.people)
        .then(res => {
          return res;
        })
  }

  getCalendarFollow(issueName, idProject,status,action){
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/history-follow?isssueName=${issueName}&idProject=${idProject}&status=${status}&actionFollow=${action}`;
      return this.shareService.returnHttpClient(url);
  }
  getStatus(){
    const url = `${this.shareService.REST_API_SERVER}/api/app/status`;
      return this.shareService.returnHttpClient(url);
  }
  getTeamByUserId(){
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/team-by-user-id`;
      return this.shareService.returnHttpClient(url);
  }
   public GetHistoryByIssue(idIssue, pageSize): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/issue-updates/${idIssue}?index=0&pageSize=${pageSize}&filterName=default&filterValue=default`;
    return this.shareService.returnHttpClient(url);
  }
  getIssueFollow(idProject){
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/issue-follow?idProject=${idProject}`;
      return this.shareService.returnHttpClient(url);
  }
  getIssueDetail(issueId){
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/${issueId}/issue-detail`;
      return this.shareService.returnHttpClient(url);
  }
}
