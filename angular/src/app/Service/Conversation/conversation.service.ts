import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class ConversationService {
  public countUnReadMessage: number;
  public listMemberUnread : any;
  public unreadMess : number
  public message : any
  public gotoMess: string = '';
  public showMiniProject : boolean = false;
  public conversationList = [];
  public positionMini = false;

  constructor(private shareService : ShareServiceService) {
    
   }
   public CreateConversationInDatabase(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/conversation`;
    return this.shareService.postHttpClient(url, data);
  }

  public CheckConversation(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/check-conversation?idproject=${id}`;
    return this.shareService.returnHttpClient(url);
  }
  public getUserHasConversation(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/user-chat-box`;
    return this.shareService.returnHttpClient(url);
  }
  public getAllUser(search): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/user-chat?input=${search}`;
    return this.shareService.returnHttpClient(url);
  }
  public UpdateLastMessage(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/last-message?IDConversation=${data.IDConversation}&lastMessage=${data.lastMessage}`;
    return this.shareService.putHttpClient(url,data);
  }
  public getAll(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/user-all`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserAll(input): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/dashboard/user-all?Filter=${input}`;
    return this.shareService.returnHttpClient(url);
  }

  public getName(userID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/${userID}/get-name`;
    return this.shareService.returnHttpClientGet(url);
  }
  public getConversationDetail(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/get-conver?idProject=${idProject}`;
    return this.shareService.returnHttpClientGet(url);
  }
  public getNameProject(idProject): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/get-name-project?idProject=${idProject}`;
    return this.shareService.returnHttpClientGet(url);
  }
  public getInfoUser(idUser): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/info-user?IdUser=${idUser}`;
    return this.shareService.returnHttpClientGet(url);
  }
  public getListProjectTag(projectId) {
    const url = `${this.shareService.REST_API_SERVER}/api/app/conversition/get-list-project-tag/${projectId}`;
    return this.shareService.returnHttpClient(url);
  }
}
