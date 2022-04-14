import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CategoryService } from '../Service/Category/category.service';
import { DepartmentService } from '../Service/Department/department.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-issue-department',
  templateUrl: './issue-department.component.html',
  styleUrls: ['./issue-department.component.scss'],
})
export class IssueDepartmentComponent implements OnInit {
  button = [
    { id: 0, name: 'Created By Member', class: 'fas fa-plus', type: 'Created' },
    { id: 1, name: 'Assigned To Member', class: 'fas fa-tag', type: 'Assigned' },
  ];
  listIssue: any;
  isShowFilter = false;
  isShowSearch = false;
  searchIssue: string = '';
  searchTeam=' ';
  idStatus = null;
  pageIndex = 1;
  totalCount = 0;
  pageSize = 8;
  idProjectIssue = null;
  idCategory = null;
  idUserIssue = null;
  idDepartmentIssue = '';
  idDepartmentTeam = null;
  selectedT: any;
  idTeamIssue = null;
  listProject: any;
  listStatus: any;
  listDepartment: any;
  listCategory: any;
  selectedD: any;
  listUser: any;
  selectedTeam: string = '';
  selected2 = 0;
  listTeamSearchIssue;
  userLogin;
  Assign: Boolean = false;
  constructor(
    private departmentService: DepartmentService,
    public shareService: ShareServiceService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    private projectService: ProjectService,
    private userService: UserService,
    public loaderService: LoaderService,
    public router: Router
  ) { }

  ngOnInit(): void {

    this.userLogin = this.shareService.getUserData.id;
    this.getListProject();
    this.getListStatus();
    this.getListCategory();
    this.getListUser();
    if(this.shareService.admin){
      this.getListDepartment();
      this.changeDepartmentSearchIssue('','');
    }
   if(this.shareService.manager){
    this.listDepartment = null;
    this.departmentService.getNameDepartmentByIdManager(this.userLogin).subscribe(data=>{
      this.idDepartmentIssue = '';
      this.getListTeam();
      // this.departmentService.getListTeam(this.searchTeam, data)
      // .subscribe(data => {
      //   this.listTeamSearchIssue = data;
      //   this.getListIssueByDepartment();
      // });
    })
   }
   if(this.shareService.leader){
     this.listDepartment = null;
     this.listTeamSearchIssue = null;
     this.departmentService.getNameTeamByIdLeader(this.userLogin).subscribe(data=>{
       this.idTeamIssue = data[0];
       this.getListIssueByDepartment();
     })
   }
  }
  // lấy danh sách issue
  getListIssueByDepartment() {
    let skipCount = (this.pageIndex-1) * (this.pageSize);
    this.departmentService
      .getListIssueAll(
        this.searchIssue,
        skipCount,
        this.pageSize,
        this.idProjectIssue,
        this.idStatus,
        this.idCategory,
        this.idUserIssue,
        this.idDepartmentIssue,
        this.idTeamIssue,
        this.Assign
      )
      .subscribe(data => {
        this.listIssue = data;
        this.totalCount = data.totalCount;
      });
  }
  getListTeam(){
    var url = `${this.shareService.REST_API_SERVER}/odata/DepartmentOData/ODataService.GetListTeam()?$filter=contains(Name, '')`;
    if(this.idDepartmentTeam != null && this.idDepartmentTeam != '') {
      url += ` and NameDepartment eq '${this.idDepartmentTeam}'`;
    }
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listTeamSearchIssue = data?.value;
      this.getListIssueByDepartment();
    })
  }
  // change search issue by name
  IssueChange(input) {
    this.searchIssue = input;
    this.getListIssueByDepartment();
  }
  // thay đổi team tìm kiếm issue
  changeSearchTeamIssue(idTeam) {
    this.selectedT = idTeam;
    this.idTeamIssue = idTeam;
    this.getListIssueByDepartment();
  }
  routerBug(projectId, issueId) {
    localStorage.setItem('ProjectId', projectId);
    this.router.navigate(['project/' + projectId + '/issues/view/' + issueId]);
  }
  // thay đổi department để tìm kiếm issue form issue
  changeDepartmentSearchIssue(idDepartment,nameDepartment) {
    this.idDepartmentTeam = nameDepartment;
    this.idDepartmentIssue = idDepartment;
    this.selectedD = idDepartment;
    this.idTeamIssue = null;
    this.getListTeam();
  }
  // Thay đổi tìm kiếm issue create hoặc assign form issue
  changeSearchIssue(id) {
    this.selected2 = id;
    if (id == 0) this.Assign = false;
    else this.Assign = true;
    this.getListIssueByDepartment();
  }
  // Chọn  create hoặc assign form issue
  isActive2(item) {
    return this.selected2 === item;
  }
  // Chọn department form issue
  isActiveDepartment(item) {
    return this.selectedD === item;
  }
  // Chọn team form issue
  isActiveTeam(item) {
    return this.selectedT === item;
  }
  // change user search issue
  getUserNameIssue(idUser) {
    this.idUserIssue = idUser;
    this.getListIssueByDepartment();
  }
  // change project search issue
  getByProject(idProject) {
    this.idProjectIssue = idProject;
    this.getListIssueByDepartment();
  }
  // change status search issue
  getByStatus(idStatus) {
    this.idStatus = idStatus;
    this.getListIssueByDepartment();
  }
  // change category search issue
  getByCategory(idCategory) {
    this.idCategory = idCategory;
    this.getListIssueByDepartment();
  }
  // get list project for search project
  getListProject() {
    this.projectService.getListProject().subscribe(data => {
      this.listProject = data.items;
    });
  }
  // get list category for search issue form issue
  getListCategory() {
    this.categoryService.getListCategory().subscribe(data => {
      this.listCategory = data.items;
    });
  }
  // get list status for search issue form issue
  getListStatus() {
    this.statusService.getListStatus().subscribe(data => {
      this.listStatus = data;
    });
  }
  getListUser() {
    this.userService.getListUser().subscribe(data => {
      this.listUser = data;
    });
  }
  // get list department
  getListDepartment() {
    this.departmentService.getListDepartment('').subscribe(data => {
      this.listDepartment = data;
    });
  }
  showFilterInMobile():void{
    this.isShowFilter = !this.isShowFilter;
  }
  showSearchInMobile():void{
    this.isShowSearch = !this.isShowSearch;
  }
}

