import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class AttachmentService {
  constructor(private shareService: ShareServiceService) {}
  public CreateAttachment(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment`;
    return this.shareService.postHttpClient(url, data);
  }
  public CreateListAttachment(data,ID): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment/by-list-attachment?ID=${ID}`;
    return this.shareService.postHttpClient(url, data);
  }
  public getAttachmentByIdTable(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment/${id}`;
    return this.shareService.returnHttpClient(url);
  }
  public CreateDetailAttachment(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/detail-attachment`;
    return this.shareService.postHttpClient(url, data);
  }
  public UpdateAttachment(data,id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment?ID=${id}`;
    return this.shareService.putHttpClient(url, data);
  }
  public deleteAttachment(Id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment/${Id}`;
    return this.shareService.deleteHttpClient(url);
  }
    public RemoveAttachment(fileName): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/attachment/list-file?fileName=${fileName}`;
    return this.shareService.deleteHttpClient(url);
  }
}
