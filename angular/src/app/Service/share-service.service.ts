import { HttpClient, HttpErrorResponse, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

import { CreateMessageService } from './Message/create-message.service';

@Injectable({
  providedIn: 'root',
})
export class ShareServiceService {
  public currentDate;
  public admin: boolean = false;
  public manager: boolean = false;
  public leader: boolean = false;
  public modalAddIssue :boolean = false;
  public REST_API_SERVER = environment.apis.default.url;
  public ELSA_REST_API_SERVER = 'https://localhost:44336';
  public isLocalStorageProject = localStorage.getItem('ProjectId');
  public witType = ['Bug', 'Epic', 'Feature', 'Issue', 'Task', 'Test Case', 'User Story'];
  public mainColor = '#f50';
  // public httpOptions:any;
  constructor(
    private httpClient: HttpClient,
    private router: Router,
    private errorMessage: CreateMessageService
  ) {}
  public httpOptions = {
    headers: new HttpHeaders({
      // 'Content-Type': 'application/json',
      Authorization: `Bearer ${this.getToken()}`,
    }),
  };
  reloadRouter() {
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }
  redirectTo(uri: string) {
    this.router
      .navigateByUrl('/', { skipLocationChange: true })
      .then(() => this.router.navigate([uri]));
  }
  public returnHttpClient(url) {
    return this.httpClient
      .get<any>(url, this.httpOptions)
      .pipe(catchError(err => this.handleError(err)));
  }
  public returnHttpClientGet(url) {
    return this.httpClient.get<any>(url).pipe(catchError(err => this.handleError(err)));
  }
  public deleteHttpClient(url) {
    return this.httpClient
      .delete<any>(url, this.httpOptions)
      .pipe(catchError(err => this.handleError(err)));
  }
  public postHttpClient(url, data) {
    return this.httpClient
      .post<any>(url, data, this.httpOptions)
      .pipe(catchError(err => this.handleError(err)));
  }
  public postHttpClientAnonymous(url, data) {
    return this.httpClient.post<any>(url, data).pipe(catchError(err => this.handleError(err)));
  }
  public putHttpClient(url, data) {
    return this.httpClient
      .put(url, data, this.httpOptions)
      .pipe(catchError(err => this.handleError(err)));
  }

  public putHttpClient2(url) {
    return this.httpClient
      .put(url, this.httpOptions)
      .pipe(catchError(err => this.handleError(err)));
  }
  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.log('An error occurred:', error.error.message);
    } else {
      console.log('status: ', error.status, error.message, error);

      // if(error.status == 403) {
      //   localStorage.clear();
      //   this.router.navigate(['sign-in']);
      // }
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.log('error', error.error);
      console.error(`Backend returned code ${error.status}, ` + `body was: ${error.error}`);
    }
    // Return an observable with a user-facing error message.
    // this.spinnerService.requestSpinner();
    return throwError(error.error);
  }

  writeLocalData(accessToken, refreshToken, userData) {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('userData', userData);
    // //   {
    //   headers: new HttpHeaders({
    //     Authorization: `Bearer ${this.getToken()}`,
    //   })
    // } = {
    // //   headers: new HttpHeaders({
    // //     Authorization: `Bearer ${this.getToken()}`,
    // //   })
    // // };
  }

  deleteLocalData() {
    localStorage.clear();
  }
  getToken() {
    return localStorage.getItem('accessToken');
  }

  getIdProject() {
    return localStorage.getItem('ProjectId');
  }

  getCurrentDate() {
    var today = new Date();
    this.currentDate = today.getFullYear() + '-' + (today.getMonth() + 1) + '-' + today.getDate();
  }
  get getUserData() {
    return JSON.parse(localStorage.getItem('userData'));
  }

  errorHandling(error) {
    if (error.error.code) {
      this.errorMessage.createMessage('error', error.error.message);
    } else {
      error.error.validationErrors.forEach(error => {
        this.errorMessage.createMessage('error', error.message);
      });
    }
  }
}
