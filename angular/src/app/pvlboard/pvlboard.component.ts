import { LoaderService } from './../Service/Loader/loader.service';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from './../Service/User/user.service';
import { AssigneeService } from './../Service/Assignee/assignee.service';
import { ShareServiceService } from './../Service/share-service.service';
import { IssueService } from './../Service/Issue/issue.service';
import { StatusService } from './../Service/Status/status.service';
import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { CategoryService } from '../Service/Category/category.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { MemberService } from '../Service/Member/member.service';
import { ProjectService } from '../Service/Project/project.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
@Component({
  selector: 'app-pvlboard',
  templateUrl: './pvlboard.component.html',
  styleUrls: ['./pvlboard.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class PVLBoardComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('containerStatusPanel') containerStatusPanel!: ElementRef;
  nzRowNumberSpan: number;
  selectedUser?: any;
  listCategory: any;
  listStatus: any = null;
  listCategoryBoard: any;
  public issueID;
  isVisible = false;
  public isStatus = true;
  currentDate;
  listUser;
  isShowFilter = false;
  isVisibleAddIssue: boolean = false;
  Filter: any = null;
  idUser: any = null;
  dueDate: any = null;
  level: any = null;
  idStatus: any = null;
  loading: boolean = false;
  idCategory: any = null;
  today = new Date();
  totalCountToday: number;
  idProject = localStorage.getItem('ProjectId');
  private currentRout: boolean = true;
  queryParams;
  listAddAssign;
  listRemoveAssign = [];
  projectCurrentId;
  categoryText = 'Category';
  interval;
  statusId;
  listShowTask = [];
  currentIssueParentId: string;
  projectTfsDto;
  styleIcon: string = '';
  isVisibleProfile = false;
  idProfileShowing;
  take = 10;
  skip = 0;
  issuesCreateTemp = [];

  constructor(
    private categoryService: CategoryService,
    private signalRService: SignalRService,
    private statusService: StatusService,
    private issueService: IssueService,
    public shareService: ShareServiceService,
    private assignService: AssigneeService,
    private userService: UserService,
    private router: Router,
    private route: ActivatedRoute,
    public loaderService: LoaderService,
    private memberService: MemberService,
    private projectService: ProjectService,
    private createMessage: CreateMessageService
  ) {}
  ngAfterViewInit(): void {
    this.listenChangeTfs();
  }
  ngOnDestroy(): void {
    this.currentRout = false;
    clearInterval(this.interval);
  }

  ngOnInit(): void {
    if (this.idProject == undefined || this.idProject != this.route.snapshot.params.idProject) {
      this.router.navigateByUrl('/');
    } else {
      this.signalRService.SetConnection();
      this.projectCurrentId = this.route.snapshot.params.idProject;
      this.getProjectTfsDto();
      this.checkUserInProject(this.projectCurrentId);
      this.getListStatusByCondition();
      this.GetListUser();
      this.shareService.getCurrentDate();
      this.currentDate = this.shareService.currentDate;
      this.ReloadDB();
      this.ReloadDBAddIssue();
      this.ReloadBoardByIdProjectAndCallerDelete();
      this.ReloadDBAddAssignee();
      this.totalCountToday = this.getCountDate(this.today);
      this.getListCategory();
      this.ReloadBoardByIdProject();
      this.ReloadBoardByIdProjectAndCallerCategory();
    }
  }
  getProjectTfsDto() {
    this.projectService.getProjectTfs(localStorage.getItem('ProjectId')).subscribe(data => {
      this.projectTfsDto = data;
      this.styleIcon =
        'font-size: 13px; margin-right: 5px; color: ' +
        this.projectTfsDto.nzColor +
        '; padding-left: 5px';
    });
  }
  listenChangeTfs() {
    this.signalRService.connection.on('ChangesFromTfs', data => {
      if (data.includes(this.idProject) && this.currentRout) {
        this.getListStatusByCondition();
      }
    });
  }
  ReloadDBAddIssue() {
    this.signalRService.connection.on('ReloadBoardByIdProjectAndCallerCreate', issueDto => {
      if (issueDto.projectId == this.idProject && this.currentRout) {
        if (issueDto.parentId == null) {
          if (issueDto.caller.includes(this.shareService.getUserData.id)) {
            this.statusService.getIssueDto(issueDto.issueId).subscribe(data => {
              this.issuesCreateTemp.push(data.id);
              var statusContainer = this.listStatus.find(x => x.name == 'Open');
              statusContainer.issueList.push(data);
            });
          }
        } else {
          let issueParent;
          for (let i = 0; i < this.listStatus.length; i++) {
            let issueParentFind = this.listStatus[i].issueList.find(x => x.id == issueDto.parentId);
            if (issueParentFind) issueParent = issueParentFind;
          }
          if (issueParent) {
            this.statusService.getIssueDto(issueDto.issueId).subscribe(data => {
              issueParent.issuesChildDto.items.push(data);
            });
          }
        }
      }
    });
  }

  ReloadDB() {
    this.signalRService.connection.on('ReloadBoardByIdProjectAndCaller', issueDto => {
      if (
        issueDto.projectID == this.idProject &&
        this.currentRout &&
        (this.shareService.getUserData.id != issueDto.caller || issueDto.caller == 'update')
      ) {
        var statusContainer = this.listStatus.find(x => x.id == issueDto.currentStatus);
        var statusPreviousContainer = this.listStatus.find(x => x.id == issueDto.previousStatus);
        if (statusPreviousContainer.issueList.findIndex(x => x.id == issueDto.id) != -1) {
          var index = statusPreviousContainer.issueList.findIndex(x => x.id == issueDto.id);
          statusPreviousContainer.issueList.splice(index, 1);
          statusContainer.issueList.push(issueDto);
        }
      }
    });
  }
  ReloadBoardByIdProjectAndCallerCategory() {
    this.signalRService.connection.on('ReloadBoardByIdProjectAndCallerCategory', issueDto => {
      if (
        issueDto.projectID == this.idProject &&
        this.currentRout &&
        issueDto.caller != this.shareService.getUserData.id
      ) {
        var categoryPrevious = this.listStatus.find(x => x.id == issueDto.previousCategory);
        var index = categoryPrevious.issueList.findIndex(x => x.id == issueDto.id);
        if (index != -1) {
          categoryPrevious.issueList.splice(index, 1);
          var category = this.listStatus.find(x => x.id == issueDto.currentCategory);
          category.issueList.push(issueDto);
        }
      }
    });
  }
  ReloadDBAddAssignee() {
    this.signalRService.connection.on('ReloadBoardByIdProjectAddAssignee', issueDto => {
      if (issueDto.projectID == this.idProject && this.currentRout) {
        if (!issueDto.isHaveParent) {
          var statusContainer = this.listStatus.find(x => x.id == issueDto.statusID);
          let issue = statusContainer.issueList.find(x => x.id == issueDto.id);
          if (issue) {
            issue.assigneesList = issueDto.assigneesList;
          } else if (issueDto['caller'].includes(this.shareService.getUserData.id)) {
            statusContainer.issueList.push(issueDto);
          }
        } else {
          for (let i = 0; i < this.listStatus.length; i++) {
            let issueParentFind = this.listStatus[i].issueList.find(x => x.id == issueDto.idParent);
            if (issueParentFind) {
              let issueChild = issueParentFind.issuesChildDto.items.find(x => x.id == issueDto.id);
              issueChild.assigneesList = issueDto.assigneesList;
            }
          }
        }
      }
    });
  }
  ReloadBoardByIdProjectAndCallerDelete() {
    this.signalRService.connection.on('ReloadBoardByIdProjectAndCallerDelete', issueDto => {
      if (issueDto.projectID == this.idProject && this.currentRout) {
        if (issueDto.isHaveParent) {
          let issueParent;
          for (let i = 0; i < this.listStatus.length; i++) {
            let issueParentFind = this.listStatus[i].issueList.find(x => x.id == issueDto.idParent);
            if (issueParentFind) issueParent = issueParentFind;
          }
          if (issueParent) {
            let issueDeletedFind = issueParent.issuesChildDto.items.findIndex(
              x => x.id == issueDto.id
            );
            issueParent.issueList = issueParent.issuesChildDto.items.splice(issueDeletedFind, 1);
          }
        } else {
          for (let i = 0; i < this.listStatus.length; i++) {
            let issueDeletedFind = this.listStatus[i].issueList.findIndex(x => x.id == issueDto.id);
            if (issueDeletedFind != -1) {
              this.listStatus[i].issueList.splice(issueDeletedFind, 1);
            }
          }
        }
      }
    });
  }
  getCountDate(day) {
    return (
      Number.parseInt((day.getFullYear() * 12 * 30).toString()) +
      Number.parseInt(((day.getMonth() + 1) * 12).toString()) +
      Number.parseInt(day.getDate())
    );
  }
  handleCancelAddIssue(): void {
    this.isVisibleAddIssue = false;
  }
  showModalAddIssue(issueId): void {
    this.currentIssueParentId = issueId;
    this.isVisibleAddIssue = true;
  }
  showTask(issueId) {
    if (this.listShowTask.includes(issueId)) {
      let index = this.listShowTask.lastIndexOf(issueId);
      this.listShowTask.splice(index, 1);
    } else {
      this.listShowTask.push(issueId);
    }
  }
  AssignMyself() {
    this.idUser = this.shareService.getUserData.id;
    this.reloadQueryParams();
  }
  GetCharAt(name) {
    var rs = '';
    name.split(' ').forEach(char => (rs += char.charAt(0)));
    return rs;
  }
  convertToDate(string) {
    return new Date(string);
  }
  getCurrentSnapshotParams() {
    this.queryParams = this.route.snapshot.queryParams;
  }
  GetListUser() {
    this.userService.getListUserByIdProject(this.idProject).subscribe(data => {
      this.listUser = data;
    });
  }
  viewCategoryBoard() {
    this.isStatus = !this.isStatus;
    if (this.isStatus) {
      this.categoryText = 'Category';
    } else {
      this.categoryText = 'Status';
    }
    this.getListCategory();
    this.getListStatusByCondition();
  }
  // confirm(id, idUser,idIssue): void {
  //   new Promise((resolve, reject) => {
  //     resolve(
  //       this.assignService.DeleteAssignee(id, idIssue).subscribe(() => {
  //         this.signalRService.connection.invoke('ReloadBugAssign', this.idProject, idUser);
  //         this.signalRService.connection.invoke('ReloadBoardByIdProject', this.idProject);
  //         this.signalRService.connection.invoke('ReloadNotify', idIssue);
  //       })
  //     );
  //   });
  // }
  onDueDateChange(id, date: Date) {
    if (date != null) {
      var dateChange = date.getFullYear() + '-' + (date.getMonth() + 1) + '-' + date.getDate();
      this.issueService.patchIssueDate({ ID: id, dueDate: dateChange }).subscribe(() => {
        this.getListStatusByCondition();
        this.signalRService.connection.invoke('ReloadNotify', id);
      });
    } else {
      this.issueService.patchIssueNullDueDate({ ID: id }).subscribe(() => {
        this.getListStatusByCondition();
        this.signalRService.connection.invoke('ReloadNotify', id);
      });
    }
  }

  onChangeDueDate(result: Date): void {
    var newDate;
    var newMonth;
    if (result == null) {
      this.dueDate = null;
    } else {
      var resultDate = result.getDate();
      if (resultDate < 10) {
        newDate = '0' + resultDate.toString();
      } else newDate = resultDate.toString();
      var resultMonth = result.getMonth() + 1;
      if (resultMonth < 10) {
        newMonth = '0' + resultMonth.toString();
      } else newMonth = resultMonth.toString();
      this.dueDate = result.getFullYear() + '-' + newMonth + '-' + newDate;
    }
    this.reloadQueryParams();
  }
  onChangeAssign(user) {
    if (user == null) {
      this.idUser = null;
    }
    this.idUser = user;
    this.reloadQueryParams();
  }
  CategoryChange(result) {
    this.idCategory = result;
    this.reloadQueryParams();
  }

  getIconColor(color) {
    return 'color: ' + color;
  }

  getListCategory() {
    this.idCategory = null;
    if (!this.isStatus) {
      this.listCategory = this.listStatus;
    } else {
      this.categoryService.getList().subscribe(data => {
        if (data) {
          this.listCategory = data;
        }
      });
    }
  }
  ReloadBoardByIdProject() {
    this.signalRService.connection.on('ReloadBoardByIdProject', idProject => {
      if (idProject == this.idProject && this.currentRout) {
        this.getListStatusByCondition();
      }
    });
  }
  reloadQueryParams() {
    // this.router.navigate(['/pvl'],
    // { queryParams:
    //   { Filter: this.Filter,idUser:this.idUser,dueDate:this.dueDate,idCategory:this.idCategory } })
    this.getListStatusByCondition();
  }
  updateStatus(statusId, issue, listUserAssign) {
    this.issueService.updateIssueByStatus(statusId, issue.id).subscribe(
      () => {
        this.signalRService.connection.invoke('ReloadNotify', issue.id);
        this.signalRService.connection.invoke('ReloadBoardByIdProject', this.idProject);
        if (listUserAssign.length > 0) {
          let list = [];
          listUserAssign.forEach(element => {
            list.push(element.userID);
          });
          this.signalRService.connection.invoke('ReloadBugAssign', this.idProject, list);
        }
      },
      error => {
        console.log(error);
      }
    );
  }
  onScroll(event, statusId, issuesLength) {
    if (event.target.scrollHeight - event.target.scrollTop == event.target.offsetHeight) {
      this.statusService
        .getIssuesByStatus(
          this.idProject,
          this.Filter,
          this.idUser,
          this.dueDate,
          this.idCategory,
          issuesLength,
          this.take,
          statusId
        )
        .subscribe(data => {
          let status = this.listStatus.find(x => x.id == statusId);
          data = data.filter(x => {
            return !this.issuesCreateTemp.includes(x.id);
          });
          status.issueList = status.issueList.concat(data);
        });
        
    }
  }
  getListStatusByCondition() {
    if (this.isStatus) {
      this.statusService
        .getListStatusByCondition(
          this.idProject,
          this.Filter,
          this.idUser,
          this.dueDate,
          this.idCategory,
          this.skip,
          this.take
        )
        .subscribe(data => {
          this.listStatus = data;
        });
    } else {
      this.statusService
        .getListCategoryBoardByCondition(
          this.idProject,
          this.Filter,
          this.idUser,
          this.dueDate,
          this.idCategory
        )
        .subscribe(data => {
          this.listStatus = data;
        });
    }
  }
  setRowSpan(length) {
    var rs = Math.floor(24 / length);
    if (rs <= 4) rs = 4;
    return rs;
  }
  getBorder(color) {
    return 'border: 1px solid ' + color;
  }
  filterChange(Filter) {
    if (Filter == '') {
      Filter = null;
    }
    this.Filter = Filter;
    this.reloadQueryParams();
  }
  showModal(issueID): void {
    this.isVisible = true;
    this.selectedUser = null;
    this.issueID = issueID;
    this.getUserInIssue(issueID);
  }

  getUserInIssue(issueID) {
    this.assignService.CheckIsHaveParent(issueID).subscribe(data => {
      if (data.isHaveParent) {
        this.assignService.GetUsersAssigneeIssueParent(data.parentId, issueID).subscribe(data => {
          this.listAddAssign = data;
        });
      } else {
        this.userService.getListUserAddAssign(this.idProject, issueID).subscribe(data => {
          if (data) {
            this.listAddAssign = data;
          }
        });
      }
    });
  }
  getUserInIssueAssign(issueID) {
    this.assignService.GetListAssigneeByIssue(issueID).subscribe(data => {
      if (data) {
        this.listRemoveAssign = data;
      }
    });
  }
  confirmDeleteIssue(issue) {
    this.getUserInIssueAssign(issue.id);
    this.issueService.DeleteIssue(issue.id).subscribe(() => {
      this.signalRService.connection.invoke('ReloadNotify', issue.id);
      this.signalRService.connection.invoke(
        'ReloadBugAssign',
        this.idProject,
        this.listRemoveAssign['items'].map(e => e.userID)
      );
      this.signalRService.connection.invoke('ReloadBoardByIdProjectAndCallerDelete', issue);
    });
  }
  CreateAssignee(): void {
    this.assignService.CreateAssignee(this.selectedUser, this.issueID, false).subscribe(() => {
      this.statusService.getIssueDto(this.issueID).subscribe(data => {
        data['caller'] = this.selectedUser;
        this.signalRService.connection.invoke('ReloadBoardByIdProjectAddAssignee', data);
      });
      this.signalRService.connection.invoke('ReloadBugAssign', this.idProject, this.selectedUser);
      this.signalRService.connection.invoke('ReloadNotify', this.issueID);
    });

    this.isVisible = false;
  }
  handleCancel(): void {
    this.isVisible = false;
  }

  //drag and drop
  drop(event: CdkDragDrop<string[]>) {
    if (this.isStatus) {
      if (event.previousContainer === event.container) {
        if (event.currentIndex != event.previousIndex) {
          moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
          let previousContainer;
          let previousData = [];
          let min =
            event.currentIndex < event.previousIndex ? event.currentIndex : event.previousIndex;
          let max =
            event.currentIndex > event.previousIndex ? event.currentIndex : event.previousIndex;
          for (let i = min; i <= max; i++) {
            previousData.push(event.previousContainer.data[i]['id']);
          }
          previousContainer = {
            issuesId: previousData,
          };
          let data = {
            container: null,
            index: event.currentIndex,
            previousContainer: previousContainer,
            previousIndex: min,
          };
          const issue = event.previousContainer.data.find(
            (item, index) => index == event.previousIndex
          );
          this.issueService.patchIssueAll(data, issue['id']).subscribe(() => {
            // this.signalRService.connection.invoke('ReloadBoardByIdProject', this.idProject);
          });
        }
      } else {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex
        );

        let containerData = [];
        for (let i = event.currentIndex; i < event.container.data.length; i++) {
          containerData.push(event.container.data[i]['id']);
        }
        let container = {
          statusId: event.container.connectedTo,
          issuesId: containerData,
        };
        let previousContainer;
        if (event.previousIndex == event.previousContainer.data.length) {
          previousContainer = null;
        } else {
          let previousData = [];
          for (let i = event.previousIndex; i < event.previousContainer.data.length; i++) {
            previousData.push(event.previousContainer.data[i]['id']);
          }
          previousContainer = {
            issuesId: previousData,
          };
        }
        let data = {
          container: container,
          index: event.currentIndex,
          previousContainer: previousContainer,
          previousIndex: event.previousIndex,
        };
        const issue = event.container.data.find((item, index) => index == event.currentIndex);
        issue['index'] = event.currentIndex;
        issue['previousStatus'] = event.previousContainer.connectedTo;
        issue['currentStatus'] = container.statusId;
        issue['caller'] = this.shareService.getUserData.id;
        this.issueService.patchIssueAll(data, issue['id']).subscribe(
          () => {
            this.signalRService.connection.invoke('ReloadBoardByIdProjectAndCaller', issue);
            this.signalRService.connection.invoke('ReloadNotify', issue['id']);
            this.signalRService.connection.invoke('ReloadCloseAssignee', this.idProject);
          },
          err =>
            err.error.message
              ? this.createMessage.createMessage('error', err.error.message)
              : this.shareService.errorHandling(err)
        );
      }
    } else {
      if (event.previousContainer === event.container) {
        moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
      } else {
        transferArrayItem(
          event.previousContainer.data,
          event.container.data,
          event.previousIndex,
          event.currentIndex
        );
        const issue = event.container.data.find((item, index) => index == event.currentIndex);
        this.issueService
          .patchIssueCategory({ id: issue['id'], category: event.container.connectedTo })
          .subscribe(() => {
            // this.getListStatusByCondition();
            this.signalRService.connection.invoke('ReloadNotify', issue['id']);
            issue['currentCategory'] = event.container.connectedTo;
            issue['previousCategory'] = event.previousContainer.connectedTo;
            issue['caller'] = this.shareService.getUserData.id;
            this.signalRService.connection.invoke('ReloadBoardByIdProjectAndCallerCategory', issue);
          });
      }
    }
  }
  checkUserInProject(projectId) {
    if (projectId) {
      this.memberService.checkUserInProject(projectId).subscribe(data => {
        if (!data) {
          this.shareService.deleteLocalData();
          this.router.navigate(['/sign-in']);
        }
      });
    }
  }
  //hien thi filter o mobile
  showFilterInMobile(): void {
    this.isShowFilter = !this.isShowFilter;
  }
  styeCustomBox() {
    return 'border-left:solid 1px ' + this.projectTfsDto.nzColor;
  }
  showUserProfile(id) {
    this.idProfileShowing = id;
    this.isVisibleProfile = true;
  }
  handleCancelProfile() {
    this.isVisibleProfile = false;
  }

  //messenger
  CreateConversition() {
    this.isVisibleProfile = false
    this.createMessage.CheckConversation(this.idProfileShowing).subscribe(data => {
      if (data['hasConversation']) {
        this.signalRService.connection.invoke(
          'DrawerChat',
          data['conversationId'],
          this.shareService.getUserData.id
        );
      } else {
        this.createMessage.CreateConversition(this.idProfileShowing).subscribe(data => {
          this.signalRService.connection.invoke(
            'DrawerChat',
            data.conversationId,
            this.shareService.getUserData.id
          );
        });
      }
      this.isVisibleProfile = false;
    });
  }
}
