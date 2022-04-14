import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  public isLoggedIn = false;
  redirectUrl: string;
  currentUserId: any;
  constructor(private shareService: ShareServiceService) {}
  public login(data):Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/user/sign-in`;
    return this.shareService.postHttpClient(url, data);
  }

  logout(): void {
    localStorage.clear();
    this.currentUserId = null;
  }

  public get getCurrentUserId() {
    return (this.currentUserId = localStorage.getItem('currentUserId'));
  }
  public getCurrentUserTfs(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/azure/get-profile`;
    return this.shareService.returnHttpClient(url);
  }
}
