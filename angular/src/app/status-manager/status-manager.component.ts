import { Component, OnInit } from '@angular/core';
import { CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';
import { NzButtonSize } from 'ng-zorro-antd/button';
import { StatusService } from '../Service/Status/status.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { ShareServiceService } from '../Service/share-service.service';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { LoaderService } from '../Service/Loader/loader.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MemberService } from '../Service/Member/member.service';

@Component({
  selector: 'app-status-manager',
  templateUrl: './status-manager.component.html',
  styleUrls: ['./status-manager.component.scss'],
})
export class StatusManagerComponent implements OnInit {
  projectLogin;
  size: NzButtonSize = 'small';
  listStatus: any;
  isVisibleAddStatus = false;
  isVisible = false;
  selectedStatus = null;
  selectedStatusEdit = null;
  isOkLoadingAddStatus = false;
  isOkLoadingEditStatus = false;
  confirmModal?: NzModalRef;
  ColorString;
  colorFinish: string="#708090";
  event;
  idStatusEdit;
  constructor(
    public router: Router,
    private rout: ActivatedRoute,
    private memberService: MemberService,
    private statusService: StatusService,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    private shareService: ShareServiceService,
    public loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.projectLogin = this.rout.snapshot.params.idProject;
    this.checkUserInProject( this.projectLogin);
    this.getListStatus();
  }
  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    if (event.previousIndex != event.currentIndex) {
      this.statusService.patchStatusAll(event.container.data).subscribe();
    }
  }
  getListStatus() {
    this.statusService.getListStatus().subscribe(data => {
      this.listStatus = data;
    });
  }
  handleCancelAddStatus() {
    this.isVisibleAddStatus = false;
  }
  handleCancelEditStatus() {
    this.isVisible = false;
  }
  CreateStatus(): void {
    if (this.selectedStatus == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      return;
    } else {
      this.addStatus();
      this.isVisibleAddStatus = false;
      this.selectedStatus = null;
    }
  }
  EditStatus(): void {
    if (this.selectedStatusEdit == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      this.isVisible = true;
      return;
    } else {
      this.editStatus();
      this.isVisible = false;
      this.selectedStatusEdit = null;
    }
  }
  showModalAddStatus(): void {
    this.isVisibleAddStatus = true;
  }
  showModalEditStatus(id, name, color): void {
    this.selectedStatusEdit =name;
    this.idStatusEdit = id;
    this.colorFinish = color;
    this.isVisible = true;

  }
  editStatus() {
    const dataEdit = {
      Name: this.selectedStatusEdit,
      NzColor: this.colorFinish
    };
    this.statusService.PutStatus(dataEdit, this.idStatusEdit).subscribe(
      () => {
        this.getListStatus();
        this.createMessage.createMessage('success', 'Edit status success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  getColor(color) {
    this.colorFinish = color;
  }
  addStatus() {
    const dataAddStatus = {
      Name: this.selectedStatus,
      NzColor: this.colorFinish.substring(1),
    };
    this.statusService.CreateStatus(dataAddStatus).subscribe(
      () => {
        this.getListStatus();
        this.createMessage.createMessage('success', 'Add status success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  RemoveStatus(StatusID) {
    this.confirmation.warn('::Bạn chắc chắn xóa?', '::Chắc chắn xóa').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.statusService.deleteStatus(StatusID).subscribe(
          () => {
            this.createMessage.createMessage('success', 'Delete status success!');
            this.getListStatus();
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      }
    });
  }
  checkUserInProject(projectId){
    if(projectId){
      this.memberService.checkUserInProject(projectId).subscribe(data=>{
        if(!data){
          this.shareService.deleteLocalData();
          this.router.navigate(['/sign-in']);
        }
      });
    }
  }
}
