import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { DepartmentService } from '../Service/Department/department.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-department',
  templateUrl: './department.component.html',
  styleUrls: ['./department.component.scss'],
})
export class DepartmentComponent implements OnInit {
  selectedDepartment: string = '';
  IdUserManager: string = null;
  nameManagerEdit;
  IdUsermanagerEdit: string = null;
  searchDepartment: string = '';
  searchTeam: string = '';
  listUser: any;
  idDepartmentGetMember;
  listCategory: any;
  listMemberDepartment: any;
  listStatus: any;
  idDepartmentEdit;
  idDepartmentIssue;
  isVisibleAddDepartment = false;
  isVisibleEditDepartment = false;
  isVisibleGetListMember = false;
  isVisibleShowIssue = false;
  listDepartment;
  selectedT: any;
  isOkLoadingAddDepartment = false;
  isOkLoadingEditDepartment = false;
  isOkLoadingGetListMember = false;
  isOkLoadingShowIssue = false;
  confirmModal?: NzModalRef;
  public NameDepartment;
  public NameDepartmentEdit;
  constructor(
    private departmentService: DepartmentService,
    public shareService: ShareServiceService,
    private confirmation: ConfirmationService,
    private modal: NzModalService,
    private userService: UserService,
    private createMessage: CreateMessageService,
    public loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.getListDepartment();
    this.getListUser();
  }
  DeparmentChange(input) {
    this.searchDepartment = input;
    this.getListDepartment();
  }
  getListDepartment() {
    var url = `${this.shareService.REST_API_SERVER}/odata/DepartmentOData?$filter=contains(Name, '${this.searchDepartment}')`;
  this.shareService.returnHttpClient(url).subscribe(data => {
    this.listDepartment = data?.value;
  })
}
  showModalAddDepartment(event): void {
    this.isVisibleAddDepartment = true;
    event.stopPropagation();
  }
  handleCancelAddDepartment(): void {
    this.isVisibleAddDepartment = false;
  }
  handleCancelGetListMember() {
    this.isVisibleGetListMember = false;
  }
  // Đóng modal show issue
  handleCancelShowIssue() {
    this.isVisibleShowIssue = false;
  }
  handleOkAddDepartment(): void {
    this.CreateDepartment();
  }
  handleOkGetListMember() {
    this.isVisibleGetListMember = false;
  }
  // Hiển thị màn hình sửa department
  showModalEditDepartment(idDepartment, idManager, nameManager, nameDepartment): void {
    this.isVisibleEditDepartment = true;
    this.idDepartmentEdit = idDepartment;
    this.IdUsermanagerEdit = idManager;
    this.nameManagerEdit = nameManager;
    this.NameDepartmentEdit = nameDepartment;
  }
  // Hiển thị danh sách issue
  showModalShowIssue(idDepartment) {
    this.isVisibleShowIssue = true;
  }
  // Hiển thị danh sách member của department
  showModalGetListMember(idDepartment) {
    var department = this.listDepartment.find(department => department.id === idDepartment);
    if (department.countMember > 0) {
      this.idDepartmentGetMember = idDepartment;
      this.getListMember();
      this.isVisibleGetListMember = true;
    }
  }
  // Danh sách member
  getListMember() {
    this.departmentService
      .getListMemberByIdDepartment(this.idDepartmentGetMember)
      .subscribe(data => {
        this.listMemberDepartment = data;
      });
  }
  handleCancelEditDepartment(): void {
    this.isVisibleEditDepartment = false;
  }
  handleOkEditDepartment(): void {
    this.editDepartment();
  }
  getListUser() {
    this.userService.getListUser().subscribe(data => {
      this.listUser = data;
    });
  }
  CreateDepartment() {
    const dataDepartment = {
      name: this.NameDepartment,
      idManager: this.IdUserManager.toString(),
    };
    this.departmentService.CreateDepartment(dataDepartment).subscribe(
      data => {
        if (data) {
          this.isVisibleAddDepartment = false;
          this.createMessage.createMessage('success', 'Create Successfully');
          this.getListDepartment();
          this.NameDepartment = null;
          this.IdUserManager = null;
        }
      },
      err => {
        this.isVisibleAddDepartment = true;
        this.createMessage.createMessage('error', err.error.message);
      }
    );
  }
  editDepartment() {
    const dataEdit = {
      Name: this.NameDepartmentEdit,
      idManager: this.IdUsermanagerEdit.toString(),
    };
    this.departmentService.updateDepartment(this.idDepartmentEdit, dataEdit).subscribe(
      () => {
        this.getListDepartment();
        this.isVisibleEditDepartment = false;
        this.NameDepartmentEdit = null;
        this.IdUsermanagerEdit = null;
        this.createMessage.createMessage('success', 'Edit Department success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  RemoveDepartment(IdDepartment): void {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Do you want to delete the Department?',
      nzContent: 'When deleting Department, it will delete all teams, members in the department',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.departmentService.DeleteDepartment(IdDepartment).subscribe(
            () => {
              this.getListDepartment();
              this.createMessage.createMessage('success', 'Delete Department successfully!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }
  RemoveMember(IdMember) {
    this.confirmation.warn('::Do you want to remove this member?', '::Remove Member').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.departmentService.DeleteMemberTeam(IdMember).subscribe(
          () => {
            this.createMessage.createMessage('success', 'Remove member success!');
            this.getListMember();
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      }
    });
  }
}
