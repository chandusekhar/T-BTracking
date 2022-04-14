import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { Component, OnInit } from '@angular/core';
import { NzModalRef } from 'ng-zorro-antd/modal';
import { CategoryService } from '../Service/Category/category.service';
import { DepartmentService } from '../Service/Department/department.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-my-team',
  templateUrl: './my-team.component.html',
  styleUrls: ['./my-team.component.scss']
})
export class MyTeamComponent implements OnInit {
  IdTeam:string='';
  listMemberTeam:any;
  isVisibleAddMember = false;
  isOkLoadingAddMember = false;
  IdMemberAdd:string=null;
  listUser : any;
  listCategory : any;
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
  idTeamIssue = null;
  listProject:any;
  listStatus:any;
  selected2 = 0;
  Assign:Boolean = false;
  public NameTeam;
  constructor(
    private departmentService:DepartmentService,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    private userService:UserService,
    public shareService: ShareServiceService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    private projectService : ProjectService,
  ) { }

  ngOnInit(): void {
    this.userLogin = this.shareService.getUserData.id;
    this.getIdTeamByIdLeader();
    this.getListUser();
    this.getListProject();
    this.getListCategory()
    this.getListStatus();
  }
  getListUser() {
    this.userService.getListUser().subscribe(data => {
      this.listUser = data;
    });
  }
  getListMember(){
    this.departmentService.getListMemberTeamById(this.IdTeam).subscribe(data=>{
      this.listMemberTeam=data;

    })
  }
  getIdTeamByIdLeader(){
    this.departmentService.getNameTeamByIdLeader(this.userLogin).subscribe(data=>{
      this.IdTeam = data;
      this.getListMember();
      this.getListIssueByTeam()
    })
  }
  handleCancelAddMember(): void {
    this.isVisibleAddMember = false;
  }
  showModalAddMember(event): void {
    this.isVisibleAddMember = true;
    event.stopPropagation();
    this.getListUserAddMember();
  }
  getListUserAddMember(){
    this.departmentService.getListUserAddMember(this.IdTeam).subscribe(data=>{
      this.listUserAddMember=data;
    })
  }
   handleOkAddMember(): void {
     this.CreateMemberTeam();
     this.isOkLoadingAddMember=false;
     this.getListMember();
     this.IdMemberAdd=null;
   }
  CreateMemberTeam() {
    for (let i = 0; i < this.IdMemberAdd.length; i++) {
    const dataMember= {
      idUser: this.IdMemberAdd[i].toString(),
      idTeam: this.IdTeam.toString()

    };
    this.departmentService.CreateMemberTeam(dataMember).subscribe(
      data => {
          this.isVisibleAddMember = false;
          this.getListMember();
      }
    );
    }
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
 getListIssueByTeam(){
  // this.departmentService.getListIssueAll(this.searchIssue,this.idProjectIssue,this.idStatus
  //   ,this.idCategory,this.idUserIssue,'',this.IdTeam,this.Assign).subscribe(data=>{
  //     this.listIssue = data;
  //   })
}
 // change search issue by name
IssueChange(input){
  this.searchIssue=input;
  this.getListIssueByTeam();
}
// Thay đổi tìm kiếm issue create hoặc assign form issue
changeSearchIssue(id){
  this.selected2 = id;
  if(id == 0) this.Assign = false;
  else this.Assign=true;
  this.getListIssueByTeam();
}
// Chọn  create hoặc assign form issue
isActive2(item){
  return this.selected2 === item;
}
// change user search issue
getUserNameIssue(idUser){
  this.idUserIssue =  idUser;
  this.getListIssueByTeam();
}
// change project search issue
getByProject(idProject){
  this.idProjectIssue = idProject;
  this.getListIssueByTeam();
}
// change status search issue
getByStatus(idStatus){
  this.idStatus = idStatus;
  this.getListIssueByTeam();
}
// change category search issue
getByCategory(idCategory){
  this.idCategory = idCategory;
  this.getListIssueByTeam();
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
}
