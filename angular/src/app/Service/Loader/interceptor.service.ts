import { LoaderService } from './loader.service';
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class InterceptorService implements HttpInterceptor {
  constructor(public loaderService: LoaderService) {}
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (
      !req.url.includes('tdesk.tpos.dev/api') &&
      !req.urlWithParams.includes('tdesk.tpos.dev/api')
    ) {
      this.loaderService.isLoading.next(true);
    }
    return next.handle(req).pipe(
      finalize(() => {
        this.loaderService.isLoading.next(false);
      })
    );
  }
}
