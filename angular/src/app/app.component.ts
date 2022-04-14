import { MemberService } from './Service/Member/member.service';
import { IssueService } from './Service/Issue/issue.service';
import { AssigneeService } from './Service/Assignee/assignee.service';
import { ShareServiceService } from './Service/share-service.service';
import { Component, Input, OnInit, AfterViewInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { SignalRService } from './Service/SignalR/signal-r.service';
import { LoaderService } from './Service/Loader/loader.service';
import { ProjectService } from './Service/Project/project.service';
import { CreateMessageService } from './Service/Message/create-message.service';
import { UserService } from './Service/User/user.service';
import { NavigationStart, NavigationEnd } from '@angular/router';
import { FollowService } from './Service/Follow/follow.service';
import { NotificationService } from './Service/Notification/notification.service';
import { NzNotificationService } from 'ng-zorro-antd/notification';
import { StatusService } from './Service/Status/status.service';
import { DepartmentService } from './Service/Department/department.service';
import { CdkDragStart } from '@angular/cdk/drag-drop';
import { ConversationService } from './Service/Conversation/conversation.service';
// import { SocketServiceService } from './Service/SocketService/socket-service.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class AppComponent implements OnInit, AfterViewInit {
  unread: number;
  @Input() CountIssueAssign: Number;
  isCollapsed = false;
  userData: any;
  public currentQueryUrl;
  public idProject;
  listAssign;
  isVisible = false;
  percentClosed;
  countIssue = 0;
  listProject;
  listNotify;
  isVisibleAddProject: boolean;
  valueAddProject: any;
  isOkLoadingAddProject = false;
  isVisibleAddMember: boolean;
  idProjectAdd: any;
  selectedUserAssign: any;
  listUser: any;
  admin: boolean = false;
  isOkLoadingAddMember = false;
  // isVisibleAddIssue: boolean = false;
  urlBeforeRedirect: string = '';
  maxCount = 20;
  url;
  colorProject;
  currentPath: string;
  messageNotiy;
  isVisibleShowMess = false;
  Filter: string = 'null';
  FilterAssign: string = null;
  expandSet = new Set<number>();
  projectId: string = null;
  projectName;
  listHistory;
  projectLogin;
  listStatus;
  checktext = '';
  currentRouter;
  idProjectCreate: string = '';
  idConversation: string = '';
  public dragging: boolean;
  showMessbyProject: boolean = false;
  connectionHub: any;
  test1 = 'false';
  visibledrawer = false;
  pageIndex = 1;
  pageSize = 10;
  totalIssueCount = 0;
  listIdUser = [];
  showNotifi = false;
  skip = 0;
  notificationLoading = false;
  positionMiniChat = [];
  dragTemp = false;
  projectTfsDto;
  constructor(
    public router: Router,
    public shareService: ShareServiceService,
    private signalRService: SignalRService,
    private assignService: AssigneeService,
    public loaderService: LoaderService,
    private issueService: IssueService,
    private projectService: ProjectService,
    private createMessage: CreateMessageService,
    private userService: UserService,
    private memberService: MemberService,
    private rout: ActivatedRoute,
    private followService: FollowService,
    private notifyService: NotificationService,
    private notification: NzNotificationService,
    private statusService: StatusService,
    private departmentService: DepartmentService,
    public conversationService: ConversationService // private socketService : SocketServiceService
  ) {}

  ngAfterViewInit(): void {
    if (localStorage.getItem('userData')) {
      this.getListProject();
    }
  }
  redirectProject(id) {
    this.router.navigate(['project/' + id + this.urlBeforeRedirect]);
    localStorage.setItem('ProjectId', id);
    this.projectId = id;
    this.shareService.getIdProject();
    this.shareService.reloadRouter();
    this.GetListAssignByUser();
    this.getProjectTfsDto();
    this.getProject(this.shareService.getIdProject());
  }
  ngOnInit(): void {
    // this.getConverdetail()
    this.signalRService.SetConnection();
    this.connectionHub = this.signalRService.connection;
    this.url = this.rout.snapshot.params;
    this.router.events.subscribe(e => {
      if (e instanceof NavigationEnd) {
        if (e.url == '/') {
          this.currentPath = null;
        } else {
          var stringArr = e.url.split('/');
          this.currentPath = `/${stringArr[1]}/${stringArr[2]}/`;
          this.currentRouter = e.url;
        }
      }
    });
    localStorage.setItem('abpSession', JSON.stringify({ language: 'vi' }));
    if (this.shareService.getUserData) {
      this.signalRService.SetConnection();
      this.getListNotify();
      this.checkUserAdmin();
      this.ReloadDB();
      this.ReloadNotify();
      this.listenChangeRouter();
      this.GetListAssignByUser();
      this.ReloadCloseAssignee();
      // this.ReLoadMessage();
      this.listenChangeTfs();
      this.reloadProjectName();
      this.getProject(this.shareService.getIdProject());
      // this.p_getChat()
    }
    this.getProjectTfsDto();
  }
  listenChangeTfs() {
    this.signalRService.connection.on('ChangesFromTfs', data => {
      if (data[0] == this.shareService.getUserData.id) {
        this.signalRService.connection.invoke('ReloadNotify', data[2]);
      }
      this.GetListAssignByUser();
    });
  }

  getProjectTfsDto() {
    if (this.shareService.getIdProject()) {
      this.projectService.getProjectTfs(this.shareService.getIdProject()).subscribe(data => {
        this.projectTfsDto = data;
      });
    }
  }

  inputPositionMiniChat($event) {
    this.positionMiniChat = $event;
  }
  receiveMessage($event) {
    this.unread = $event;
  }
  reciveFalseMess($event) {
    this.test1 = $event;
  }
  getIssueDetail(issueId) {
    this.followService.GetHistoryByIssue(issueId).subscribe(data => {
      this.listHistory = data;
    });
  }
  onExpandChange(id: number, checked: boolean): void {
    if (checked) {
      this.getIssueDetail(id);
      this.expandSet.clear();
      this.expandSet.add(id);
    } else {
      this.expandSet.delete(id);
    }
  }
  handleCancelMess(): void {
    this.isVisibleShowMess = false;
    this.signalRService.connection.invoke(
      'DisposeMessageModal',
      true,
      this.shareService.getUserData?.id
    );
  }
  showMesss(): void {
    this.isVisibleShowMess = true;
  }

  listenChangeRouter() {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart) {
        var component = event.url.split('/');
        if (component[4]) {
          component[3]
            ? (this.urlBeforeRedirect = '/' + component[3] + '/' + component[4])
            : (this.urlBeforeRedirect = '/home');
        } else {
          component[3]
            ? (this.urlBeforeRedirect = '/' + component[3])
            : (this.urlBeforeRedirect = '/home');
        }
      }
    });
  }
  checkUserAdmin() {
    this.userService.CheckAdmin().subscribe(data => {
      this.shareService.admin = data;
    });
    if (!this.shareService.admin) {
      this.departmentService.checkManager(this.shareService.getUserData.id).subscribe(data => {
        this.shareService.manager = data;
      });
    }
    if (!this.shareService.manager && !this.shareService.admin) {
      this.departmentService.checkLeader(this.shareService.getUserData.id).subscribe(data => {
        this.shareService.leader = data;
      });
    }
  }
  getListProject() {
    if (localStorage.getItem('userData')) {
      this.projectService
        .getListProjectByUserId(JSON.parse(localStorage.getItem('userData')).id, '')
        .subscribe(
          res => {
            this.listProject = res;
          },
          error => {
            this.shareService.errorHandling(error);
          }
        );
    }
  }
  updateUnread() {
    this.notifyService.patchListNotify().subscribe(() => {
      this.listNotify.unRead = 0;
      this.getListNotify();
      this.showNotifi = !this.showNotifi;
    });
  }
  ReloadDB() {
    this.signalRService.connection.on('ReloadBugAssign', (idProject, idUser) => {
      if (
        (idProject == this.shareService.getIdProject() &&
          idUser.includes(this.shareService.getUserData.id)) ||
        (idProject == 'Reload' && idUser.includes(this.shareService.getUserData.id))
      ) {
        this.GetListAssignByUser();
        this.getListProject();
        this.getProjectTfsDto();
        this.getProject(this.shareService.getIdProject());
      }
      this.userData = JSON.parse(localStorage.getItem('userData'));
    });
  }
  EnterNotify(issueId, id) {
    forkJoin([
      this.notifyService.updateSingleNotify(id),
      this.issueService.GetProjectId(issueId),
    ]).subscribe(data => {
      this.getListNotify();
      if (data[1] != '00000000-0000-0000-0000-000000000000') {
        localStorage.setItem('ProjectId', data[1]);
        this.GetListAssignByUser();
        this.router.navigate(['project/' + data[1] + '/issues/view/' + issueId]);
      } else {
        this.router.navigate(['***']);
      }
    });
  }
  updateSingleNotify(id) {
    this.notifyService.updateSingleNotify(id).subscribe(() => {
      this.getListNotify();
    });
  }
  getNewestNotify(issueId) {
    this.notifyService.getNewestNotify().subscribe(data => {
      if (data) {
        this.notification.blank('New Notification', data.message, {
          nzStyle: {
            width: '500px',
          },
          nzClass: 'test-class',
        });
      }
    });
  }
  // .onClick.subscribe(() => {
  //   this.issueService.GetProjectId(issueId).subscribe(data => {
  //     if (data) {
  //       this.router.navigate(['project/' + data + '/issues/view/' + issueId]);
  //     }
  //   });
  // });
  ReloadNotify() {
    this.signalRService.connection.on('ReloadNotify', issueId => {
      if (issueId == this.shareService.getUserData.id) {
        this.getListNotify();
        this.getNewestNotify(issueId);
      } else {
        if (issueId == 'Reload') {
          this.getListNotify();
          this.GetListAssignByUser();
        } else {
          this.followService.CheckFollow(issueId).subscribe(data => {
            if (data) {
              this.getListNotify();
              this.getNewestNotify(issueId);
            }
          });
        }
      }
    });
  }
  ReloadCloseAssignee() {
    this.signalRService.connection.on('ReloadCloseAssignee', projectId => {
      if (projectId == localStorage.getItem('ProjectId')) {
        this.assignService.GetCountAssignee(projectId).subscribe(data => {
          this.countIssue = data;
        });
      }
    });
  }
  getListNotify() {
    this.notificationLoading = true;
    this.notifyService
      .getListNotify(this.skip, this.maxCount, this.shareService.getUserData.id)
      .subscribe(
        data => {
          if (data) {
            this.notificationLoading = false;
            this.listNotify = data;
          }
        },
        err => (this.notificationLoading = false)
      );
  }
  getMoreListNotify() {
    this.maxCount += 20;
    this.getListNotify();
  }
  routerBug(projectId, issueId) {
    localStorage.setItem('ProjectId', projectId);
    this.router.navigate(['project/' + projectId + '/issues/view/' + issueId]);
    this.isVisible = false;
  }
  signOut() {
    this.shareService.deleteLocalData();
    this.shareService.admin = false;
    this.shareService.manager = false;
    this.shareService.leader = false;
    this.router.navigate(['/sign-in']);
  }
  GetListAssignByUser() {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    this.assignService
      .GetListAssigneeByUser(
        localStorage.getItem('ProjectId') ? localStorage.getItem('ProjectId') : this.projectId,
        this.Filter,
        this.shareService.getUserData.id,
        skipCount,
        this.pageSize
      )
      .subscribe(data => {
        if (data) {
          this.listAssign = data;
          this.countIssue = data.countIssue;
          this.totalIssueCount = data.totalCount;
        }
      });
  }
  PageIndexAssigneeChange(event) {
    this.pageIndex = event;
    this.GetListAssignByUser();
  }
  PageSizeAssigneeChange(event) {
    this.pageSize = event;
    this.GetListAssignByUser();
  }
  navigate(NameModule) {
    this.idProject = this.shareService.getIdProject();
    if (NameModule == 'add-issue') {
      this.router.navigate(['project/' + this.idProject + '/add-issue']);
    }
    if (NameModule == 'home') {
      this.router.navigate(['project/' + this.idProject]);
    }
    if (NameModule == 'issue') {
      this.router.navigate(['project/' + this.idProject + '/issues']);
    }
    if (NameModule == 'board') {
      this.router.navigate(['project/' + this.idProject + '/board']);
    }
    if (NameModule == 'my-profile') {
      this.router.navigate(['/my-profile']);
    }
    if (NameModule == 'dashboard') {
      this.router.navigate(['/departmentManage']);
      this.signalRService.connection.invoke('ReloadBugAssign', 'Reload', [
        this.shareService.getUserData.id,
      ]);
    }
    if (NameModule == 'my-follow') {
      this.router.navigate(['/my-follow']);
    }
    if (NameModule == 'admin') {
      this.router.navigate(['/admin']);
    }
    if (NameModule == 'my-profile') {
      this.router.navigate(['/profile']);
    }
  }
  GetListStatus() {
    this.statusService.getList().subscribe(data => {
      this.listStatus = data;
    });
  }
  onChangeProject(event) {
    this.projectId = event;
    this.GetListAssignByUser();
  }
  onChangeStatus(event) {
    this.Filter = event;
    this.GetListAssignByUser();
  }
  showModal(): void {
    this.getListProject();
    this.GetListStatus();
    this.GetListAssignByUser();
    // this.updateReceivedDate(this.listAssign.items);

    this.isVisible = true;
  }

  handleOk(): void {
    this.isVisible = false;
  }
  showModalAddProject(): void {
    this.isVisibleAddProject = true;
  }
  showModalAddIssue(): void {
    if (this.shareService.getIdProject()) {
      this.shareService.modalAddIssue = true;
    }
  }
  handleCancel(): void {
    this.isVisible = false;
  }
  handleCancelAddProject(): void {
    this.isVisibleAddProject = false;
  }
  handleCancelAddIssue(): void {
    this.shareService.modalAddIssue = false;
  }
 
  checkNameExist(name) {
    this.projectService.getCheckNameExist(name).subscribe(data => {
      this.checktext = data;
    });
  }
  handleOkAddProject(): void {
    if (this.valueAddProject == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      this.isVisibleAddProject = true;
      return;
    } else {
      this.CreateProject({ name: this.valueAddProject });
      this.isVisibleAddProject = false;
      this.valueAddProject = null;
    }
  }
  CreateProject(input) {
    this.projectService.CreateProject(input).subscribe(data => {
      this.idProjectCreate = data.id;
      this.getListProject();
    });
  }
  handleCancelAddMember(): void {
    this.isVisibleAddMember = false;
  }
  getListUser() {
    if (this.shareService.getIdProject()) {
      this.userService.getListUserAddProject(this.shareService.getIdProject()).subscribe(data => {
        this.listUser = data.items;
      });
    }
  }
  showModalAddMember(): void {
    if (this.shareService.getIdProject()) {
      this.isVisibleAddMember = true;
      this.getListUser();
    }
  }
  CreateMember(): void {
    this.listIdUser = [];
    if (this.selectedUserAssign == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      this.isVisibleAddMember = true;
      return;
    } else {
      const data = {
        ProjectID: this.shareService.getIdProject(),
        userIDs: this.selectedUserAssign,
      };
      this.memberService.CreateByListMember(data).subscribe(() => {
        this.conversationService
          .CheckConversation(this.shareService.getIdProject())
          .subscribe(data => {
            if (data == false) {
              this.conversationService
                .getNameProject(this.shareService.getIdProject())
                .subscribe(id => {
                  this.userService.getListUserByIdProject(id.id).subscribe(name => {
                    name.items.forEach(element => {
                      if (element.id != this.shareService.getUserData?.id) {
                        this.listIdUser.push(element.id);
                      }
                    });
                    this.createMessage.CreateGroupChat(id.name, this.listIdUser).subscribe(data => {
                      const res = {
                        idProject: this.shareService.getIdProject(),
                        conversationId: data.conversationId,
                      };
                      this.CreateConversationDatabase(res);
                    });
                  });
                });
            } else {
              if (this.shareService.getIdProject() != null) {
                this.conversationService
                  .getConversationDetail(this.shareService.getIdProject())
                  .subscribe(data => {
                    this.createMessage
                      .addMember(data.conversationId, this.selectedUserAssign)
                      .subscribe();
                  });
              }
            }
          });
        this.signalRService.connection.invoke('ReloadProject', this.selectedUserAssign);
        this.isVisibleAddMember = false;
        this.selectedUserAssign = null;
        this.createMessage.createMessage('success', 'Add member success!');
      });
    }
  }
  CreateConversationDatabase(Data) {
    this.conversationService.CreateConversationInDatabase(Data).subscribe((res: any) => {});
  }

  public handleDragStart(event: CdkDragStart): void {
    this.dragging = true;
    this.dragTemp = true;
  }
  // public handleClick(event: MouseEvent): void {
  //   if (this.dragging) {
  //     this.dragging = false;
  //     return;
  //   }
  //   this.router.navigate(['messenger']);
  // }
  // showMessProject() {
  //   console.log(this.test1);
  //   if (this.test1 == 'true') {
  //     this.test1 = 'false';
  //   } else {
  //     this.test1 = 'true';
  //   }
  // }
  ////////////////
  receiveDrawer($event) {
    this.visibledrawer = $event;
  }
  open(event: MouseEvent): void {
    if (this.dragging) {
      this.dragging = false;
      return;
    }
    this.visibledrawer = true;
  }

  close(): void {
    this.visibledrawer = false;
  }
  public getProject(idProject) {
    this.conversationService.getNameProject(idProject).subscribe(res => {
      this.projectName = res.name;
      this.GetCharAt(this.projectName);
    });
  }
  GetCharAt(name) {
    let temp = name.split(' ')
    .map(char => char.charAt(0))
    .join('');
    return temp.charAt(0).concat(temp.charAt(1)).toUpperCase();
  }
  reloadProjectName(){
    this.signalRService.connection.on('ReloadProjectName', () => {
      this.getProject(this.shareService.getIdProject());
    });
  }
}
