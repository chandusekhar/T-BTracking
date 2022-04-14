import { ShareServiceService } from './../share-service.service';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class CommentService {
  constructor(private shareService: ShareServiceService) {}

  public getListComments(): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments`;
    return this.shareService.returnHttpClient(url);
  }
  public getCommentById(commentId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/${commentId}/by-id`;
    return this.shareService.returnHttpClient(url);
  }
  public createComment(data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments`;
    return this.shareService.postHttpClient(url, data);
  }
  public deleteComment(commentId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/by-id/${commentId}`;
    return this.shareService.deleteHttpClient(url);
  }
  public updateComment(commentId, data): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments?Id=${commentId}`;
    return this.shareService.putHttpClient(url, data);
  }
  public getListCommentsByIssueId(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/comments-by-issue-id/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListCommentsByUserId(userId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/comments-by-user-id/${userId}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListCommentResult(SkipCount, MaxResultCount, IssueId, Filter): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/${IssueId}/result-by-issue-id?Filter=${Filter}&SkipCount=${SkipCount}&MaxResultCount=${MaxResultCount}`;
    return this.shareService.returnHttpClient(url);
  }
  public getListUserCommentsByIssueId(issueId): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/comments/user-cmt-issued/${issueId}`;
    return this.shareService.returnHttpClient(url);
  }
}
