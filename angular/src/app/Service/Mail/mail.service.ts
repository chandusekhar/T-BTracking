import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class MailService {

  constructor(
    private shareService:ShareServiceService
  ) { }

    public SendMail(data,subject,message): Observable<any>{
      const url = `${this.shareService.REST_API_SERVER}/api/app/send-notify-mail/send-mail?subject=${subject}&message=${message}`;
      return this.shareService.postHttpClient(url,data);
    }
    public SendMailInvite(email,projectId): Observable<any>{
      const url = `${this.shareService.REST_API_SERVER}/api/app/send-notify-mail/send-mail-invite/${projectId}?Email=${email}`;
      return this.shareService.postHttpClient(url,"");
    }
    public SendMailResponse(email,name,projectId): Observable<any>{
      const url = `${this.shareService.REST_API_SERVER}/api/app/send-notify-mail/send-mail-response/${projectId}?Name=${name}&Email=${email}`;
      return this.shareService.postHttpClient(url,"");
    }
}
