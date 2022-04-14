import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ShareServiceService } from '../share-service.service';

@Injectable({
  providedIn: 'root'
})
export class ExportFileService {

  constructor(private shareService: ShareServiceService) { }
  public NewSheet(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/new-google-sheet`;
    return this.shareService.returnHttpClient(url);
  }
  public UpdateSheet(spreadSheetId,sheetName): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/history/update-google-sheet/${spreadSheetId}?sheet=${sheetName}`;
    return this.shareService.returnHttpClient(url);
  }
}
