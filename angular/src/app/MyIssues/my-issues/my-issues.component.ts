import { LoaderService } from './../../Service/Loader/loader.service';
import { IssueService } from './../../Service/Issue/issue.service';
import { Component, Input, OnInit } from '@angular/core';
import { StatusService } from 'src/app/Service/Status/status.service';
import { ProjectService } from 'src/app/Service/Project/project.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
@Component({
  selector: 'app-my-issues',
  templateUrl: './my-issues.component.html',
  styleUrls: ['./my-issues.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class MyIssuesComponent implements OnInit {
  @Input() isModalShowing = '';
  button = [
    { id: 0, name: 'Created By Me', class: 'fas fa-plus' },
    { id: 1, name: 'Assigned To Me', class: 'fas fa-tag' },
  ];
  listHistory;
  selected2 = 0;
  listIssues;
  fullName;
  eMail;
  phone;
  Assign: boolean = false;
  id;
  Filter: string = null;
  DueDate: string = null;
  TMTProject;
  listStatus;
  filterStatus = 'null';
  projectId = 'null';
  projectList;
  pageIndex = 1;
  pageSize = 13;
  constructor(
    private issueService: IssueService,
    public loaderService: LoaderService,
    private statusService: StatusService,
    private projectService: ProjectService
  ) {}

  ngOnInit(): void {
    this.GetListStatus();
    this.getListIssueByMe();
    this.GetListProject();
    this.changeSearchIssueModal(this.isModalShowing);
  }
  GetCharAt(name) {
    return name.charAt(0);
  }
  GetListStatus() {
    this.statusService.getList().subscribe(data => {
      this.listStatus = data;
    });
  }
  onChangeProject(event) {
    this.projectId = event;
    this.getListIssueByMe();
  }
  onChangeStatus(event) {
    this.filterStatus = event;
    this.getListIssueByMe();
  }
  GetListProject() {
    this.projectService.getListProject().subscribe(data => {
      this.projectList = data;
    });
  }
  RemoveIssue(id) {
    this.issueService.DeleteIssue(id).subscribe(() => {
      this.getListIssueByMe();
    });
  }
  getListIssueByMe() {
    this.isModalShowing != '' ? (this.pageSize = 10) : (this.pageSize = 13);
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    this.issueService
      .getListIssueCreatedByMe(
        this.Filter,
        this.filterStatus,
        this.projectId,
        this.Assign,
        this.pageSize,
        skipCount
      )
      .subscribe(data => {
        this.listIssues = data;
      });
  }
  nzPageIndexChange(event) {
    this.pageIndex = event;
    this.getListIssueByMe();
  }
  // Thay đổi tìm kiếm issue create hoặc assign form issue
  changeSearchIssue(id) {
    this.selected2 = id;
    if (id == 0) this.Assign = false;
    else this.Assign = true;
    this.getListIssueByMe();
  }
  // Chọn  create hoặc assign form issue
  isActive2(item) {
    return this.selected2 === item;
  }
  changeSearchIssueModal(input){
    if(this.button[1].name === input){
      this.Assign = true;
      this.getListIssueByMe();
      this.selected2=1;
    }
  }
}
