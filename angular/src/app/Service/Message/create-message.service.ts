import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NzMessageService } from 'ng-zorro-antd/message';
import { Observable } from 'rxjs';
import { TDestAPI } from 'src/app/model/OutputDto/TDeskAPI';

@Injectable({
  providedIn: 'root',
})
export class CreateMessageService {
  get token() {
    return localStorage.getItem('accessToken');
  }

  constructor(
    private message: NzMessageService,

    private http: HttpClient
  ) {}
  createMessage(type: string, message: string): void {
    this.message.create(type, `${message}`);
  }
  CreateConversition(receiverId: string): Observable<any> {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    let dto = { receiverId: receiverId };
    return this.http.post(`${TDestAPI.tdeskApi}api/v1/conversation/create-userconversation`, dto, {
      headers: reqHeader,
    });
  }
  //create message
  CreateMessageAPI(
    content: string,
    type: number,
    conversationId: string,
    attachment: string[]
  ): Observable<any> {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    let dto = {
      content: content,
      type: type,
      conversationId: conversationId,
      attachmentIds: attachment,
    };
    return this.http.post(`${TDestAPI.tdeskApi}api/v1/messages`, dto, { headers: reqHeader });
  }
  //getMessage in conversation
  getMessage(conversationId: string, MaxResultCount, skipcount) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(
      `${TDestAPI.tdeskApi}api/v1/messages?ConversationId=${conversationId}&Sorting=creationTime%20desc&SkipCount=${skipcount}&MaxResultCount=${MaxResultCount}`,
      { headers: reqHeader }
    );
  }
  CheckConversation(receivedId: string) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(
      `${TDestAPI.tdeskApi}api/v1/conversation/check-conversation?id=${receivedId}&conversationType=0`,
      { headers: reqHeader }
    );
  }
  ////check
  CheckUserInConversation(conversationId: string) {
    var reqHeader = new HttpHeaders({
      // 'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(
      `${TDestAPI.tdeskApi}api/v1/userconversation/check-user-in-conversation?conversationId=${conversationId}`,
      { headers: reqHeader }
    );
  }
  //tạo cuộc hội thoại nhóm
  CreateGroupChat(nameConversation: String, listMemberId: string[]): Observable<any> {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    let dto = { nameConversation: nameConversation, listMemberId: listMemberId };
    return this.http.post(`${TDestAPI.tdeskApi}api/v1/conversation/create-groupconversation`, dto, {
      headers: reqHeader,
    });
  }
    //tạo cuộc hội thoại nhóm tu 2 user co san
    CreateGroupChatFrom2User(conversaitonId:string, nameConversation: String, members: []): Observable<any> {
      var reqHeader = new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.token}`,
      });
      let dto = {conversaitonId:conversaitonId, nameConversation: nameConversation, members: members };
      return this.http.post(`${TDestAPI.tdeskApi}api/v1/conversation/create-group-with-conversationUser`, dto, {
        headers: reqHeader,
      });
    }

  //getListMember
  getListMember(skipCount,maxResultCount) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/conversation?SkipCount=${skipCount}&MaxResultCount=${maxResultCount}`, {
      headers: reqHeader,
    });
  }
  //tạo  Attachment
  createAttachment(file: File): Observable<any> {
    let formData = new FormData();
    formData.append('attachmentDto.Attachment', file, file.name);
    var reqHeader = new HttpHeaders({
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.post<any>(`${TDestAPI.tdeskApi}api/v1/attachment`, formData, {
      reportProgress: true,
      observe: 'events',
      headers: reqHeader,
    });
  }
  //xóa attachment
  deleteAttachment(idAttachment) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.delete(`${TDestAPI.tdeskApi}api/v1/attachment/${idAttachment}`, {
      headers: reqHeader,
    });
  }
  //getconversation
  getConversaion(idConversation) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/conversation/${idConversation}`, {
      headers: reqHeader,
    });
  }
  //delete conversaiton
  delete(idConversation) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.delete(`${TDestAPI.tdeskApi}api/v1/conversation?id=${idConversation}`, {
      headers: reqHeader,
    });
  }
  //changeNamegroup
  changeNamegroup(conversationId: string, newName: string): Observable<any> {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.put(
      `${TDestAPI.tdeskApi}api/v1/conversation/change-name-group?conversationId=${conversationId}&newName=${newName}`,
      null,
      { headers: reqHeader }
    );
  }

  //addmember
  addMember(conversationId: String, listMemberId: String[]): Observable<any> {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    let dto = { conversationId: conversationId, listMemberId: listMemberId };
    return this.http.post(`${TDestAPI.tdeskApi}api/v1/userconversation/add-member`, dto, {
      headers: reqHeader,
    });
  }
  //deleteMember
  deleteMember(idConversation, userId) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.delete(
      `${TDestAPI.tdeskApi}api/v1/userconversation/delete-member?conversationId=${idConversation}&memberId=${userId}`,
      { headers: reqHeader }
    );
  }
  //thông tin message
  messageDetail(idMessage) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/messages/${idMessage}`, {
      headers: reqHeader,
    });
  }

  //listAttachment
  listAttachmentConversation(conversationId : string, MaxResultCount) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/attachment/conversation-paging?ConversationId=${conversationId}&AttachmentType=0&SkipCount=0&MaxResultCount=${MaxResultCount}`, {
      headers: reqHeader,
    });
  }
  //
  listAttachmentNotImage(conversationId : string, MaxResultCount) {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/attachment/conversation-paging?ConversationId=${conversationId}&AttachmentType=1&SkipCount=0&MaxResultCount=${MaxResultCount}`, {
      headers: reqHeader,
    });
  }
//getInfoMessage
getInfoMessage(messageId : string) {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  return this.http.get(`${TDestAPI.tdeskApi}api/v1/messages/${messageId}`, {
    headers: reqHeader,
  });
}

//tìm kiếm message
SearchMessage(conversationId: String, Text: string, skipCount, MaxResultCount): Observable<any> {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  Text = encodeURIComponent(Text)
  return this.http.post(`${TDestAPI.tdeskApi}api/v1/common/search-user?conversationId=${conversationId}&Text=${Text}&SkipCount=${skipCount}&MaxResultCount=${MaxResultCount}`,
  null, {
    headers: reqHeader,
  });
}
SearchMessageForTotalCount(conversationId: String, Text: string): Observable<any> {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  Text = encodeURIComponent(Text)
  return this.http.post(`${TDestAPI.tdeskApi}api/v1/common/search-user?conversationId=${conversationId}&Text=${Text}`,
  null, {
    headers: reqHeader,
  });
}
//cuộn scroll đến tin nhắn
scrollToMessage(messageId : string, oldCount, newCount) {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  return this.http.get(`${TDestAPI.tdeskApi}api/v1/messages/list-message-by-request?MessageId=${messageId}&oldCount= ${oldCount}&newCount=${newCount}`, {
    headers: reqHeader,
  });
}
///login
login(phoneNumber: String, password: string): Observable<any> {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  let dto = { phoneNumber: phoneNumber, password: password };
  return this.http.post(`${TDestAPI.tdeskApi}api/v1/users/sign-in/password`, dto, {
    headers: reqHeader,
  });
}
//delete IMG
deleteMessage(idMessage,idConversation) {
  var reqHeader = new HttpHeaders({
    'Content-Type': 'application/json',
    Authorization: `Bearer ${this.token}`,
  });
  return this.http.delete(`${TDestAPI.tdeskApi}api/v1/messages/${idMessage}?conversationId=${idConversation}`, {
    headers: reqHeader,
  });
}

///////////////////////avartar
UploadAvartar(file: File): Observable<any> {
  let formData = new FormData();
  formData.append('AvatarImage', file, file.name);
  var reqHeader = new HttpHeaders({
    Authorization: `Bearer ${this.token}`,
  });
  return this.http.post<any>(`${TDestAPI.tdeskApi}api/v1/users/upload-avatar`, formData, {
    reportProgress: true,
    observe: 'events',
    headers: reqHeader,
  });
  }
  getInfoToGetAvartar() {
    var reqHeader = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.get(`${TDestAPI.tdeskApi}api/v1/users/get-profile-user`, {
      headers: reqHeader,
    });
  }
  UploadAvartarGroup(file: File, conversationId): Observable<any> {
    let formData = new FormData();
    formData.append('AvatarImage', file, file.name);
    var reqHeader = new HttpHeaders({
      Authorization: `Bearer ${this.token}`,
    });
    return this.http.post<any>(`${TDestAPI.tdeskApi}api/v1/conversation/upload-avatar-group?conversationId=${conversationId}`, formData, {
      reportProgress: true,
      observe: 'events',
      headers: reqHeader,
    });
    }

    //////////////getUnreadMessage
    updateUnread(conversationId: string): Observable<any> {
      var reqHeader = new HttpHeaders({
        'Content-Type': 'application/json',
        Authorization: `Bearer ${this.token}`,
      });
      return this.http.put(
        `${TDestAPI.tdeskApi}api/v1/messages/unread-message?conversationId=${conversationId}`,
        null,
        { headers: reqHeader }
      );
    }
}
