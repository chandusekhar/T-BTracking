import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(private shareService:ShareServiceService) { }
  public getListNotificationByUser(idUser): Observable<any>{
  const url = `${this.shareService.REST_API_SERVER}/api/app/notifications/notification-by-user-id/${idUser}`;
  return this.shareService.returnHttpClient(url);
  }
  public getListNotify(skip,take,currentUserId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/notifications/notification/${currentUserId}?skip=${skip}&take=${take}`;
    return this.shareService.returnHttpClient(url);
  }
  public getNewestNotify(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/notifications/newest-notify`;
    return this.shareService.returnHttpClient(url);
  }
  public patchListNotify(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/notifications/update`;
    return this.shareService.returnHttpClient(url);
  }
  public updateSingleNotify(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/notifications/update-single/${id}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateNotification(data): Observable<any>
  {
  const url = `${this.shareService.REST_API_SERVER}/api/app/notifications`;
  return this.shareService.postHttpClient(url,data);
  }
}
