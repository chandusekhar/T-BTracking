import { ShareServiceService } from './../share-service.service';
import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  public connection;
  constructor(private shareService: ShareServiceService) {}

  SetConnection() {
    if (this.shareService.getToken()) {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.shareService.REST_API_SERVER + '/notify', {
          accessTokenFactory: () => this.shareService.getToken(),
        })
        .build();
      this.connection.start();
    }
  }
  //
}
