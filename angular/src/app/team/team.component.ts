import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { DepartmentService } from '../Service/Department/department.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-team',
  templateUrl: './team.component.html',
  styleUrls: ['./team.component.scss'],
})
export class TeamComponent implements OnInit {
  isVisibleAddTeam = false;
  isShowFilter = false;
  IdTeamEdit: string = null;
  IdUserLeader: string = null;
  IdDeparmentAdd = '';
  selectedTeam: string = '';
  a = '';
  idDepartmentTeam = null;
  IdTeamAddMember: string = null;
  nameTeamGetMember: string = '';
  listMemberTeam: any;
  IdLeaderShow: string = null;
  isVisibleEditTeam = false;
  isVisibleAddMember = false;
  isVisibleGetListMember = false;
  isOkLoadingAddMember = false;
  isOkLoadingGetListMember = false;
  removeTeam = false;
  IdMemberAdd: string = null;
  nameDepartmentShow;
  isOkLoadingEditTeam = false;
  isOkLoadingAddTeam = false;
  nameLeaderEditTeam;
  idDepartmentEdit;
  listDepartment;
  listUser: any;
  public NameTeamEdit;
  IdUserLeaderEdit: string = null;
  listUserAddMember: any;
  confirmModal?: NzModalRef;
  listTeam: any;
  selected: any;
  selected1: any;
  userLogin;
  public NameTeam;
  constructor(
    private departmentService: DepartmentService,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    private modal: NzModalService,
    private userService: UserService,
    public shareService: ShareServiceService,
    public loaderService: LoaderService
  ) {}

  ngOnInit(): void {
    this.userLogin = this.shareService.getUserData.id;
    this.getListUser();
    if (this.shareService.admin) {
      this.getListDepartment();
    }
    if (this.shareService.manager) {
      this.listDepartment = null;
      this.departmentService.getNameDepartmentByIdManager(this.userLogin).subscribe(data => {
        this.idDepartmentTeam = null;
        this.nameDepartmentShow = data[1];
        this.idDepartmentEdit = data[0];
        this.IdDeparmentAdd = '';
       // this.idDepartmentTeam = null;
        this.getListTeam();
        this.getListDepartment();
      });
    }
    if (this.shareService.leader) {
      this.listTeam = null;
      this.departmentService.getNameTeamByIdLeader(this.userLogin).subscribe(data => {
        this.nameTeamGetMember = data[1];
        this.IdTeamAddMember = data[0];
        this.getListMember();
      });
    }
  }
  showModalAddTeam(idTeam): void {
    this.isVisibleAddTeam = true;
    this.IdTeamEdit = idTeam;
  }
  handleCancelAddTeam(): void {
    this.isVisibleAddTeam = false;
  }
  handleCancelGetListMember(): void {
    this.isVisibleGetListMember = false;
  }
  handleOkAddTeam(): void {
    this.createTeam();
  }
  handleOkGetListMember(): void {
    this.isVisibleGetListMember = false;
  }
  createTeam() {
    const dataTeam = {
      name: this.NameTeam,
      idLeader: this.IdUserLeader.toString(),
      idDepartment: this.IdDeparmentAdd.toString(),
    };
    this.departmentService.CreateTeam(dataTeam).subscribe(
      data => {
        if (data) {
          this.isVisibleAddTeam = false;
          this.getListTeam();
          this.NameTeam = null;
          this.IdUserLeader = null;
          this.createMessage.createMessage('success', 'Create Successfully');
        }
      },
      err => {
        this.isVisibleAddTeam = true;
        this.createMessage.createMessage('error', err.error.message);
      }
    );
  }
  getListTeam() {
    var url = `${this.shareService.REST_API_SERVER}/odata/DepartmentOData/ODataService.GetListTeam()?$filter=contains(Name, '${this.selectedTeam}')`;
    if (this.idDepartmentTeam != null) {
      url += ` and NameDepartment eq '${this.idDepartmentTeam}'`;
    }
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listTeam = data?.value;
      this.nameTeamGetMember = this.listTeam[0]?.Name;
      this.getMemberTeampc(
        this.listTeam[0]?.Id,
        this.listTeam[0]?.Name,
        this.listTeam[0]?.IdLeader
      );
    });
  }
  // Tìm team theo tên form team
  TeamChange(input) {
    if (input == null) input = '';
    this.selectedTeam = input;
    this.idDepartmentTeam = null;
    this.getListTeam();
    this.selected1 = null;
  }
  // Tìm kiếm team theo tên department form team
  changeSearchDepartment(idDepartment,nameDepartment) {
    if (idDepartment == '' ) {
      this.idDepartmentTeam = null;
      this.IdDeparmentAdd = '';
      this.getListTeam();
    } else {
      this.idDepartmentTeam = nameDepartment;
      this.getListTeam();
      this.listMemberTeam = null;
      this.selected1 = idDepartment;
      this.selected = null;
      this.IdDeparmentAdd = idDepartment;
    }
  }
  getListUser() {
    this.userService.getListUser().subscribe(data => {
      this.listUser = data;
    });
  }
  getListMember() {
    var url = `${this.shareService.REST_API_SERVER}/odata/DepartmentOData/ODataService.GetListMemberByTeam()?$filter=contains(NameTeam, '${this.nameTeamGetMember}')`;
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listMemberTeam = data?.value;
      if (this.nameTeamGetMember == null || this.nameTeamGetMember == '') {
        this.listMemberTeam = null;
      }
      // this.getListTeam();
    });
  }
  // lấy danh sách member theo team cho team
  getMemberTeam(idTeam, nameTeam, idLeader) {
    if (nameTeam == null) nameTeam = '';
    this.nameTeamGetMember = nameTeam;
    this.IdTeamAddMember = idTeam;
    this.IdLeaderShow = idLeader;
    if (this.isVisibleEditTeam || this.removeTeam) {
      this.isVisibleGetListMember = false;
    } else {
      this.isVisibleGetListMember = true;
    }
    this.getListMember();
    this.selected = idTeam;
    this.removeTeam = false;
  }
  // lấy danh sách member theo team cho pc
  getMemberTeampc(idTeam, nameTeam, idLeader) {
    if (nameTeam == null) nameTeam = '';
    this.nameTeamGetMember = nameTeam;
    this.IdTeamAddMember = idTeam;
    this.IdLeaderShow = idLeader;
    this.getListMember();
    this.selected = idTeam;
    this.removeTeam = false;
  }
  // Hiển thị màn edit team
  showModalEditTeam(idTeam, idDepartment, nameDepartment, idLeader, nameLeader, nameTeam): void {
    this.isVisibleGetListMember = false;
    this.isVisibleEditTeam = true;
    this.IdTeamEdit = idTeam;
    this.nameDepartmentShow = nameDepartment;
    this.IdUserLeaderEdit = idLeader;
    this.idDepartmentEdit = idDepartment;
    this.nameLeaderEditTeam = nameLeader;
    this.NameTeamEdit = nameTeam;
  }
  editTeam() {
    const dataEdit = {
      Name: this.NameTeamEdit,
      idLeader: this.IdUserLeaderEdit.toString(),
    };
    this.departmentService.updateTeam(this.IdTeamEdit, dataEdit).subscribe(
      data => {
        this.getListTeam();
        this.isVisibleEditTeam = false;
        this.NameTeamEdit = null;
        this.IdUserLeaderEdit = null;
        this.createMessage.createMessage('success', 'Edit Team success!');
      },
      err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  // get list department
  getListDepartment() {
    var url = `${this.shareService.REST_API_SERVER}/odata/DepartmentOData?$filter=contains(Name, '${this.a}')`;
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listDepartment = data?.value;
      this.getListTeam();
    });
  }
  handleCancelEditTeam(): void {
    this.isVisibleEditTeam = false;
  }
  handleCancelAddMember(): void {
    this.isVisibleAddMember = false;
  }
  showModalAddMember(event): void {
    this.isVisibleAddMember = true;
    event.stopPropagation();
    this.getListUserAddMember();
  }
  getListUserAddMember() {
    this.departmentService.getListUserAddMember(this.IdTeamAddMember).subscribe(data => {
      this.listUserAddMember = data;
    });
  }
  handleOkEditTeam(): void {
    this.editTeam();
  }
  handleOkAddMember(): void {
    this.CreateMemberTeam();
    this.isOkLoadingAddMember = false;
    this.getListMember();
    this.IdMemberAdd = null;
  }
  CreateMemberTeam() {
    for (let i = 0; i < this.IdMemberAdd.length; i++) {
      const dataMember = {
        idUser: this.IdMemberAdd[i].toString(),
        idTeam: this.IdTeamAddMember.toString(),
      };
      this.departmentService.CreateMemberTeam(dataMember).subscribe(data => {
        this.isVisibleAddMember = false;
        this.getListMember();
      });
    }
  }
  RemoveTeam(IdTeam): void {
    this.removeTeam = true;
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn xóa Team?',
      nzContent: 'Khi xóa Team thì sẽ xóa tất cả members trong team',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.departmentService.DeleteTeam(IdTeam).subscribe(
            () => {
              this.getListTeam();
              this.createMessage.createMessage('Thành công', 'Đã xóa Team!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }
  RemoveMember(IdMember) {
    this.confirmation.warn('::Bạn chắc chắn xóa member này?', '::Xóa Member').subscribe(status => {
      if (status === Confirmation.Status.confirm) {
        this.departmentService.DeleteMemberTeam(IdMember).subscribe(
          () => {
            this.createMessage.createMessage('success', 'Delete member success!');
            this.getListMember();
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      }
    });
  }
  // Chọn team
  isActive(item) {
    return this.selected === item;
  }
  // Chọn department
  isActive1(item) {
    return this.selected1 === item;
  }
  showFilterInMobile(): void {
    this.isShowFilter = !this.isShowFilter;
  }
}
