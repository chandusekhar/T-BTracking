import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LoaderService } from '../Service/Loader/loader.service';
import { MailService } from '../Service/Mail/mail.service';
import { MemberService } from '../Service/Member/member.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss'],
})
export class SignUpComponent implements OnInit {
  showSendOtpForm = true;
  showVerifyOtpForm = false;
  showSignUpForm = false;
  current = 0;
  sendOtpFG: FormGroup;
  VerifyOtpFG: FormGroup;
  SignUpFG: FormGroup;
  phoneNumber: any;
  otpCode: any;
  smsOtpToken: any;
  email: any;
  projectId;
  name: any;
  password: any;
  status = 'process';
  createAppUser: boolean = false;
  showCreateAppUserOption: boolean = false;
  Email;
  type = 'password';
  submitSendOtp(): void {
    for (const i in this.sendOtpFG.controls) {
      this.sendOtpFG.controls[i].markAsDirty();
      this.sendOtpFG.controls[i].updateValueAndValidity();
    }
    //this.sendOtpForm=!this.sendOtpForm;
  }
  submitVerifyOtp(): void {
    for (const i in this.VerifyOtpFG.controls) {
      this.VerifyOtpFG.controls[i].markAsDirty();
      this.VerifyOtpFG.controls[i].updateValueAndValidity();
    }
    //this.sendOtpForm=!this.sendOtpForm;
  }
  submitSignUp(): void {
    for (const i in this.SignUpFG.controls) {
      this.SignUpFG.controls[i].markAsDirty();
      this.SignUpFG.controls[i].updateValueAndValidity();
    }
    //this.sendOtpForm=!this.sendOtpForm;
  }

  constructor(
    public router: Router,
    private fb: FormBuilder,
    private shareService: ShareServiceService,
    private errorMessage: CreateMessageService,
    public loaderService: LoaderService,
    private rout: ActivatedRoute,
    private memberService: MemberService,
    private mailService: MailService
  ) {}

  ngOnInit(): void {
    this.projectId = this.rout.snapshot.params.projectId;
    this.Email = this.rout.snapshot.params.Email;
    (this.sendOtpFG = this.fb.group({
      phoneNumber: [null, Validators.required],
    })),
      (this.VerifyOtpFG = this.fb.group({
        phomeNumber: [null, Validators.required],
        otpCode: [null, Validators.required],
      })),
      (this.SignUpFG = this.fb.group({
        name: [null, Validators.required],
        email: [null, Validators.required],
        password: [null, Validators.required],
      }));
  }

  showPassword() {
    this.type == 'text' ? (this.type = 'password') : (this.type = 'text');
  }

  navigate() {
    this.router.navigate(['sign-in']);
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
  sendOtp() {
    const url = `${this.shareService.REST_API_SERVER}/api/user/sign-up/send-otp-sms`;
    this.phoneNumber = this.sendOtpFG.value.phoneNumber;
    var body = {
      phoneNumber: this.phoneNumber,
    };
    this.shareService.postHttpClient(url, body).subscribe(
      res => {
        if (this.projectId) {
          this.router.navigate(['/sign-up/' + this.projectId + '/' + this.Email + '/enter-otp']);
        } else {
          this.router.navigate(['/sign-up/enter-otp']);
        }
        this.current = 1;
        this.status = 'process';
      },
      error => {
        this.errorHandling(error);
        if (error.error.message == 'Phone has existed in this server') {
          this.showCreateAppUserOption = true;
        }
        this.status = 'error';
      }
    );
  }
  verifyOtp() {
    const url = `${this.shareService.REST_API_SERVER}/api/user/sign-up/verify-otp-sms`;
    this.otpCode = this.VerifyOtpFG.value.otpCode;
    var body = {
      phoneNumber: this.phoneNumber,
      otpCode: this.otpCode,
    };
    this.shareService.postHttpClient(url, body).subscribe(
      res => {
        this.otpCode = this.VerifyOtpFG.value.otpCode;
        this.smsOtpToken = res.smsOtpToken;
        this.showVerifyOtpForm = !this.showVerifyOtpForm;
        this.showSignUpForm = !this.showSignUpForm;
        if (this.projectId) {
          this.router.navigate(['/sign-up/' + this.projectId + '/' + this.Email + '/submit']);
        } else {
          this.router.navigate(['/sign-up/submit']);
        }
        this.current = 2;
        this.status = 'process';
      },
      error => {
        this.phoneNumber = body.phoneNumber;
        this.errorHandling(error);
        this.status = 'error';
      }
    );
  }
  signUp() {
    const url = `${this.shareService.REST_API_SERVER}/api/user/sign-up`;
    if (this.Email) {
      this.SignUpFG.value.email = this.Email;
      this.email = this.Email;
    }

    var body = {
      phoneNumber: this.phoneNumber,
      otpCode: this.otpCode,
      smsOtpToken: this.smsOtpToken,
      name: this.SignUpFG.value.name,
      password: this.SignUpFG.value.password,
      email: this.SignUpFG.value.email,
    };
    this.shareService.postHttpClient(url, body).subscribe(
      res => {
        if (this.projectId) {
          const data = {
            ProjectID: this.projectId,
            UserID: res.id,
          };
          this.memberService.CreateMemberAnonymous(data).subscribe();
          this.mailService.SendMailResponse(this.Email, res.name, this.projectId).subscribe();
        }
        this.errorMessage.createMessage('success', 'Registered successfully!');
        this.current = 3;
        this.status = 'process';
        this.router.navigate(['/']);
      },
      error => {
        this.smsOtpToken = body.smsOtpToken;
        this.errorHandling(error);
        this.status = 'error';
      }
    );
  }
}
