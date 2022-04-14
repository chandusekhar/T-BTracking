import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { CategoryService } from '../Service/Category/category.service';
import { DepartmentService } from '../Service/Department/department.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-my-department',
  templateUrl: './my-department.component.html',
  styleUrls: ['./my-department.component.scss']
})
export class MyDepartmentComponent implements OnInit {
  isVisibleAddTeam = false;
  IdTeamEdit:string=null;
  IdUserLeader:string=null;
  IdDeparmentAdd:string=null;
  selectedTeam:string='';
  idDepartmentTeam:string='';
  IdTeamAddMember:string=null;
  listMemberTeam:any;
  IdLeaderShow:string=null;
  isVisibleEditTeam = false;
  isVisibleAddMember = false;
  isVisibleGetListMember =false;
  isOkLoadingAddMember = false;
  IdMemberAdd:string=null;
  nameDepartmentShow;
  isOkLoadingEditTeam = false;
  isOkLoadingAddTeam = false;
  isOkLoadingGetListMember = false;
  nameLeaderEditTeam;
  idDepartmentEdit;
  listDepartment;
  listUser : any;
  public NameTeamEdit;
  IdUserLeaderEdit:string=null;
  listUserAddMember: any;
  confirmModal?: NzModalRef;
  listTeam:any;
  selected:any;
  selected1:any;
  userLogin;
  button=[
    {id:0 , name:'Create'},
    {id:1 , name:'Assign'}
  ]
  listIssue : any;
  searchIssue:string='';
  idStatus = null;
  idProjectIssue = null;
  idCategory = null;
  idUserIssue = null;
  removeTeam = false;
  selectedT:any;
  idTeamIssue = null;
  listProject:any;
  listStatus:any;
  listCategory:any;
  selectedD:any;
  selected2 = 0;
  Assign:Boolean = false;
  public NameTeam;
  constructor( 
    private departmentService:DepartmentService,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    private modal: NzModalService,
    private userService:UserService,
    public shareService: ShareServiceService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    private projectService : ProjectService,
  ) { }

  ngOnInit(): void {
    this.userLogin = this.shareService.getUserData.id;
    if(this.shareService.manager){
      this.getIdDepartmentByIdManager();
    }
    this.getListTeam();
    this.getListUser();
    this.getListProject();
    this.getListCategory()
    this.getListStatus();
    this.getListIssueByDepartment()
  }
  showModalAddTeam(idTeam): void {
    this.isVisibleAddTeam = true;
    this.IdTeamEdit = idTeam;
  }
  handleCancelAddTeam(): void {
    this.isVisibleAddTeam = false;
  }
  handleOkAddTeam(): void {
    this.createTeam();
  }
  createTeam(){
    const dataTeam={
      name: this.NameTeam,
      idLeader: this.IdUserLeader.toString(),
      idDepartment: this.IdDeparmentAdd.toString()
    };
    this.departmentService.CreateTeam(dataTeam).subscribe(data=>{
      if(data){
 
        this.isVisibleAddTeam = false;
        this.createMessage.createMessage('success', 'Create Successfully');
        this.getListTeam();
        this.NameTeam=null;
        this.IdUserLeader=null;
      }
    },
      err => {
        this.isVisibleAddTeam = true;
        this.createMessage.createMessage('error', err.error.message);
    });
  }
  getIdDepartmentByIdManager(){
    this.departmentService.getNameDepartmentByIdManager(this.userLogin).subscribe(data=>{
      this.idDepartmentTeam = data;
    })
  }
  getListTeam(){
    this.departmentService.getListTeam(this.selectedTeam,this.idDepartmentTeam).subscribe(data=>{
      this.listTeam = data;
    })
  }
    // Tìm team theo tên form team
    TeamChange(input){
      this.selectedTeam=input;
     // this.idDepartmentTeam=null;
      this.getListTeam();
      this.selected1=null;
    }
  getListUser() {
    this.userService.getListUser().subscribe(data => {
      this.listUser = data;
    });
  }
  getListMember(){
    this.departmentService.getListMemberTeamById(this.IdTeamAddMember).subscribe(data=>{
      this.listMemberTeam=data;
    })
  }
   // lấy danh sách member theo team cho mobie
   getMemberTeam(idTeam,idLeader){
    this.IdTeamAddMember = idTeam;
    this.IdLeaderShow = idLeader;
    if(this.isVisibleEditTeam || this.removeTeam){
      this.isVisibleGetListMember = false;
    }else{
      this.isVisibleGetListMember = true;
    }
    this.selected=idTeam;
    this.getListMember();
    this.removeTeam =false;
  }
  // lấy danh sách member theo team cho pc
  getMemberTeampc(idTeam,idLeader){
    this.IdTeamAddMember = idTeam;
    this.IdLeaderShow = idLeader;
    this.getListMember();
    this.selected=idTeam;
    this.removeTeam =false;
  }
   // Hiển thị màn edit team
   showModalEditTeam(idTeam,idDepartment,nameDepartment,idLeader,nameLeader,nameTeam): void {
    this.isVisibleGetListMember = false;
    this.isVisibleEditTeam = true;
    this.IdTeamEdit = idTeam;
    this.nameDepartmentShow = nameDepartment;
    this.IdUserLeaderEdit = idLeader;
    this.idDepartmentEdit = idDepartment;
    this.nameLeaderEditTeam = nameLeader;
    this.NameTeamEdit = nameTeam;
  }
  handleCancelEditTeam(): void {
    this.isVisibleEditTeam = false;
  }
  handleCancelGetListMember(): void{
    
    this.isVisibleGetListMember = false;
  }
  handleCancelAddMember(): void {
    this.isVisibleAddMember = false;
  }
  handleOkGetListMember(): void{
    this.isVisibleGetListMember = false;
  }
  showModalAddMember(event): void {
    this.isVisibleAddMember = true;
    event.stopPropagation();
    this.getListUserAddMember();
  }
  getListUserAddMember(){
    this.departmentService.getListUserAddMember(this.IdTeamAddMember).subscribe(data=>{
      this.listUserAddMember=data;
    })
  }
  handleOkEditTeam(): void {
    this.editTeam();
   }
   handleOkAddMember(): void {
     this.CreateMemberTeam();
     this.isOkLoadingAddMember=false;
     this.getListMember();
     this.IdMemberAdd=null;
   }
   editTeam(){
    const dataEdit={
      Name: this.NameTeamEdit,
      idLeader: this.IdUserLeaderEdit.toString()
    };
    this.departmentService.updateTeam(this.IdTeamEdit,dataEdit).subscribe(data=>{
      this.getListTeam();
      this.isVisibleEditTeam=false;
      this.NameTeamEdit= null;
      this.IdUserLeaderEdit=null;
      this.createMessage.createMessage('success', 'Edit Team success!');
    },
    err => this.createMessage.createMessage('error', err.error.message)
    );
  }
  CreateMemberTeam() {
    for (let i = 0; i < this.IdMemberAdd.length; i++) {
    const dataMember= {
      idUser: this.IdMemberAdd[i].toString(),
      idTeam: this.IdTeamAddMember.toString()
      
    };
    this.departmentService.CreateMemberTeam(dataMember).subscribe(
      data => {
          this.isVisibleAddMember = false;
          this.getListMember();
      }
    );
    }
  }
  RemoveTeam(IdTeam): void {
    this.removeTeam =true;
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn xóa Team?',
      nzContent: 'Khi xóa Team thì sẽ xóa tất cả members trong team',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.departmentService.DeleteTeam(IdTeam).subscribe(() => {
            this.getListTeam();
            this.createMessage.createMessage('Thành công','Đã xóa Team!');
          },
          err => this.createMessage.createMessage('error', err.error.message)
          );
        })
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
  // lấy danh sách issue
 getListIssueByDepartment(){
  // this.departmentService.getListIssueAll(this.searchIssue,this.idProjectIssue,this.idStatus
  //   ,this.idCategory,this.idUserIssue,this.idDepartmentTeam,this.idTeamIssue,this.Assign).subscribe(data=>{
  //     this.listIssue = data;
  //   })
}
 // change search issue by name
IssueChange(input){
  this.searchIssue=input;
  this.getListIssueByDepartment();
}
 // thay đổi team tìm kiếm issue
 changeSearchTeamIssue(idTeam){
  this.selectedT = idTeam;
  this.idTeamIssue = idTeam;
  this.idDepartmentTeam == "";
  this.getListIssueByDepartment();
}
// Thay đổi tìm kiếm issue create hoặc assign form issue
changeSearchIssue(id){
  this.selected2 = id;
  if(id == 0) this.Assign = false;
  else this.Assign=true;
  this.getListIssueByDepartment();
}
// Chọn  create hoặc assign form issue
isActive2(item){
  return this.selected2 === item;
}
// Chọn department form issue
isActiveDepartment(item){
  return this.selectedD === item;
}
// Chọn team form issue
isActiveTeam(item){
  return this.selectedT === item;
} 
// change user search issue
getUserNameIssue(idUser){
  this.idUserIssue =  idUser;
  this.getListIssueByDepartment();
}
// change project search issue
getByProject(idProject){
  this.idProjectIssue = idProject;
  this.getListIssueByDepartment();
}
// change status search issue
getByStatus(idStatus){
  this.idStatus = idStatus;
  this.getListIssueByDepartment();
}
// change category search issue
getByCategory(idCategory){
  this.idCategory = idCategory;
  this.getListIssueByDepartment();
}
 // get list project for search project
 getListProject(){
  this.projectService.getListProject().subscribe(data=>{
  this.listProject=data.items;
  });
}
  // get list category for search issue form issue
  getListCategory(){
    this.categoryService.getListCategory().subscribe(data=>{
      this.listCategory=data.items;
    })
  }
  // get list status for search issue form issue
  getListStatus(){
    this.statusService.getListStatus().subscribe(data=>{
  this.listStatus=data;
    })
  }
   // get list department
   getListDepartment(){
    this.departmentService.getListDepartment('').subscribe(data=>{
      this.listDepartment=data;
    })
  }
}
