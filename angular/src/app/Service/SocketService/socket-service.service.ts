import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import * as io from 'socket.io-client';
import { TDestAPI } from 'src/app/model/OutputDto/TDeskAPI';
@Injectable({
  providedIn: 'root',
})
export class SocketServiceService {
  socketUrl = TDestAPI.socketUrl;
  socketCallUrl = TDestAPI.socketCallUrl;
  socket: any;
  socketCall: any;
  // token: Token;
  get token() {
    return localStorage.getItem('accessToken');
  }

  constructor() {
    if (!this.token) {
        this.disconnectSocket();
    } else {
          this.initSocket();
        }
    }
  initSocket(): void {
    this.socket = io(`${this.socketUrl}?token=${this.token}`);
    this.socketCall = io(`${this.socketCallUrl}?token=${this.token}`);
  }

  disconnectSocket(): void {
    this.socket.close();
    this.socketCall.close();
  }
  /**
   * @description listen data from event
   * @param eventName name of event
   * @returns Observable of data
   */
  listen(eventName: string): Observable<any> {
    return new Observable((subscriber) => {
      this.socket.on(eventName, (data: any) => {
        subscriber.next(data);
      });
    });
  }
  listenCall(eventName: string): Observable<any> {
    return new Observable((subscriber) => {
      this.socketCall.on(eventName, (data: any) => {
        subscriber.next(data);
      });
    });
  }
  public videoCallRejected(toId: string, type: string): void {
    this.socketCall.emit('video-call-reject', {
      toId,
      type,
    });
  }
  public videoCallGroupRejected(toId: string, type: string): void {
    this.socketCall.emit('video-call-group-reject', {
      toId,
      type,
    });
  }
  emit(eventName: string, data: any): void {
    this.socket.emit(eventName, data);
  }
}

