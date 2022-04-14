import { Component, Input, OnInit } from '@angular/core';
import { Attachment } from '../models/attachment';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
})
export class ProfileComponent implements OnInit {
  @Input() profileIdShowing = '';
  fullName;
  eMail;
  id;
  phone;
  profile;
  uniqueName = '';
  pAT = '';
  profileLoading = false;
  files: any;
  listAttachments: Attachment[] = [];
  listAttachmentId: string[] = [];
  constructor(
    private userService: UserService,
    private createMessage: CreateMessageService,
    public shareService: ShareServiceService
  ) {}

  ngOnInit(): void {
    (this.profileIdShowing != '' && this.profileIdShowing != this.shareService.getUserData.id)
      ? this.getProfileById(this.profileIdShowing)
      : this.getProfile();
  }

  updateProfile() {
    this.profileLoading = true;
    this.userService.updateProfile({ name: this.fullName, email: this.eMail }).subscribe(
      data => {
        this.createMessage.createMessage('success', 'successfully');
        localStorage.setItem('userData', JSON.stringify(data));
        this.updateUserTfs();
      },
      err => {
        this.profileLoading = false;
        this.createMessage.createMessage('error', err.error.message);
      }
    );
  }
  updateUserTfs() {
    const data = {
      uniqueName: this.uniqueName,
      pat: this.pAT,
    };
    this.userService.CreateUpdateUserTfs(data).subscribe(
      data => {
        if (data) {
          this.getUserTfs(data.userId);
          this.profileLoading = false;
        }
      },
      err => {
        this.profileLoading = false;
        this.createMessage.createMessage('error', err.error.message);
      }
    );
  }
  getUserTfs(userId) {
    this.userService.GetUserInforTfs(userId).subscribe(data => {
      if (data) {
        this.uniqueName = data.uniqueName;
        this.pAT = data.pat;
      }
    });
  }
  getProfile() {
    this.userService.getProfile().subscribe(data => {
      if (data) {
        this.profile = data;
        this.fullName = data.name;
        this.eMail = data.email;
        this.phone = data.phoneNumber;
        this.id = data.id;
        this.getUserTfs(data.id);
      }
    });
  }
  getProfileById(id) {
    this.userService.GetUserById(id).subscribe(data => {
      if (data) {
        this.profile = data;
        this.fullName = data.name;
        this.eMail = data.email;
        this.phone = data.phoneNumber;
        this.id = data.id;
        this.getUserTfs(data.id);
      }
    });
  }
}
