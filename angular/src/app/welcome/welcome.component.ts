import { SignalRService } from './../Service/SignalR/signal-r.service';
import { Component, OnInit } from '@angular/core';
@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.scss']
})
export class WelcomeComponent implements OnInit {
message:string=null;
public connectionHub;
constructor(
  private signalRService:SignalRService
) { }

  ngOnInit() {
    this.signalRService.SetConnection();
    this.connectionHub=this.signalRService.connection
  }

  showMessage(): void {
    this.connectionHub.invoke("PushNotifyToUser",'2E57B023-773F-423A-D992-39FCB88D0391',this.message);
    this.connectionHub.invoke("PushNotifyAll",this.message)
  }


}
