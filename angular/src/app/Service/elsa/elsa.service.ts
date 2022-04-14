import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root',
})
export class ElsaService {
  constructor(private shareService: ShareServiceService) {}
  public getListDefinitions(): Observable<any> {
    const url = `${this.shareService.ELSA_REST_API_SERVER}/v1/workflow-definitions`;
    return this.shareService.returnHttpClientGet(url);
  }
  public getDefinition(id): Observable<any> {
    const url = `${this.shareService.ELSA_REST_API_SERVER}/v1/workflow-definitions/${id}//export`;
    return this.shareService.returnHttpClientGet(url);
  }
  public executeDefinition(id, data): Observable<any> {
    const url = `${this.shareService.ELSA_REST_API_SERVER}/v1/workflow-definitions/${id}/execute`;
    return this.shareService.postHttpClient(url, data);
  }
}
