import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ShareServiceService } from '../Service/share-service.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { AuthService } from '../Service/Auth/auth.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { MemberService } from '../Service/Member/member.service';
import { MailService } from '../Service/Mail/mail.service';
import { DepartmentService } from '../Service/Department/department.service';
@Component({
  selector: 'app-sign-in',
  templateUrl: './sign-in.component.html',
  styleUrls: ['./sign-in.component.scss'],
})
export class SignInComponent implements OnInit {
  tempUsername;
  tempPassword;
  validateForm!: FormGroup;
  public connectionHub;
  admin: boolean = false;
  projectId;
  email;
  type = 'password';
  submitForm(): void {
    //a khai bao admin nay ben share service r qua app component goi qua a
    for (const i in this.validateForm.controls) {
      this.validateForm.controls[i].markAsDirty();
      this.validateForm.controls[i].updateValueAndValidity();
    }
  }

  constructor(
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    private shareService: ShareServiceService,
    public loaderService: LoaderService,
    private message: CreateMessageService,
    private signalRService: SignalRService,
    private rout: ActivatedRoute,
    private memberService: MemberService,
    private mailService: MailService,
    private departmentService: DepartmentService
  ) {}

  ngOnInit(): void {
    if (this.shareService.getUserData?.id) {
      this.router.navigate(['/']);
    }
    this.projectId = this.rout.snapshot.params.projectId;
    this.email = this.rout.snapshot.params.Email;
    this.validateForm = this.fb.group({
      phoneNumber: [null, [Validators.required]],
      password: [null, [Validators.required]],
      // remember: [true],
    });
    this.signalRService.SetConnection();
    this.connectionHub = this.signalRService.connection;
    this.shareService.manager = false;
    this.shareService.leader = false;
    this.shareService.reloadRouter();
  }

  showPassword() {
    this.type == 'text' ? (this.type = 'password') : (this.type = 'text');
  }

  navigate() {
    if (this.projectId && this.email) {
      this.router.navigate(['sign-up/' + this.projectId + '/' + this.email]);
    } else {
      this.router.navigate(['sign-up']);
    }
  }
  login() {
    this.authService.login(this.validateForm.value).subscribe(
      res => {
        const promiseLogin = new Promise((resolve, reject) => {
          resolve(
            this.shareService.writeLocalData(
              res.accessToken,
              res.refreshToken,
              JSON.stringify(res.data)
            )
          );
          this.shareService.admin = res.data.isAdmin;
          if (!this.shareService.admin) {
            this.checkUserManager();
          }
          if (this.projectId && this.email) {
            const data = {
              ProjectID: this.projectId,
              UserID: res.data.id,
            };
            this.memberService.CreateMemberAnonymous(data).subscribe();
            this.mailService
              .SendMailResponse(this.email, res.data.name, this.projectId)
              .subscribe();
          }
          //  this.connectionHub.invoke('ReloadNotify', '');//Reload
          // this.signalRService.connection.invoke('SignIn');

          this.connectionHub.invoke('ReloadBugAssign', this.shareService.getIdProject(), [
            this.shareService.getUserData.id,
          ]);
          this.connectionHub.invoke('SendMessage', '', this.shareService.getUserData.id);
        });
        promiseLogin
          .then(() => {
            location.reload();
            this.router.navigateByUrl('/departmentManage');
            //   this.connectionHub.invoke('ReloadNotify', '');//Reload
          })
          .then(() => {
            this.message.createMessage('success', 'Login successfully!');
          });
      },
      error => {
        this.shareService.errorHandling(error);
      }
    );
    this.message
      .login(
        this.validateForm.controls.phoneNumber.value,
        this.validateForm.controls.password.value
      )
      .subscribe(
        data => {},
        error => {
          this.shareService.errorHandling(error);
        }
      );
  }
  checkUserManager() {
    this.departmentService.checkManager(this.shareService.getUserData.id).subscribe(data => {
      this.shareService.manager = data;
    });
    if (!this.shareService.manager) {
      this.departmentService.checkLeader(this.shareService.getUserData.id).subscribe(data => {
        this.shareService.leader = data;
      });
    }
  }
}
