import {
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  Output,
  EventEmitter,
  HostListener,
  AfterViewInit,
} from '@angular/core';
import { Router } from '@angular/router';
import { MemberService } from '../Service/Member/member.service';
import { LoaderService } from './../Service/Loader/loader.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import { HistoryService } from '../Service/History/history.service';
import { Chart } from 'chart.js';
import { IssueService } from '../Service/Issue/issue.service';
import { MailService } from '../Service/Mail/mail.service';
import { ConversationService } from '../Service/Conversation/conversation.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class DashboardComponent implements OnInit, AfterViewInit {
  @ViewChild('radar') radar: ElementRef;
  @ViewChild('pie') pie: ElementRef;
  @ViewChild('line') line: ElementRef;
  @ViewChild('bar') bar: ElementRef;
  groupBy: any;
  projects: any;
  countUpdate = 0;
  isVisibleAddProject = false;
  isVisibleAddMember = false;
  isOkLoadingAddProject = false;
  isOkLoadingAddMember = false;
  isVisibleInvite = false;
  filter: string = '';
  filterShow: string = '';
  filterUser: string = '';
  selectNumber: number = 4;
  pageIndex1 = 1;
  totalCount1 = 0;
  pageSize1 = 13;
  selectedUserAssign = null;
  listUser: any;
  idProjectAdd;
  search: string;
  listProjectShow;
  email;
  admin: boolean = false;
  listUserAll;
  pageIndex = 1;
  pageIndexUser = 1;
  pageSize = 3;
  pageSizeUser = 5;
  totalCount = 0;
  isHaveProject: boolean = false;
  increaseNumber: number = 4;
  userLogin;
  listUpdate;
  isVisibleShowMess = false;
  projectStatistic: any;
  date = new Date();
  dd = String(this.date.getDate()).padStart(2, '0');
  mm = String(this.date.getMonth() + 1).padStart(2, '0');
  yyyy = this.date.getFullYear();
  radioChartValue = 'radar';
  today = `${this.yyyy}-${this.mm}-${this.dd}`;
  mainColor = this.shareService.mainColor;
  public load = 'Load More';
  limitPageSize = 0;
  valueModalInformation;
  modalLoading = false;
  //listOfMyIssue: any;
  data: any;
  dataRadarProject: any;
  dataLineProject: any;
  dataPieProject: any;
  public valueAddProject;
  otherMessage: number;
  public isAddLoad = false;
  percent = 0;
  checktext = '';
  nameProject: string = '';
  ///mess
  MemberInProject: any;
  listIdUser = [];
  updatesLoading = false;
  options: string[] = [];
  isVisibleInformationModal = false;
  usersProcessing;
  isVisibleProfile = false;
  idProfileShowing;
  conversationId;
  isShowMoreLoading = false;
  linkTfsProject = '';

  @Output() newItemEvent = new EventEmitter<number>();
  constructor(
    public historyService: HistoryService,
    private projectService: ProjectService,
    public shareService: ShareServiceService,
    private createMessage: CreateMessageService,
    private userService: UserService,
    private memberService: MemberService,
    private rout: Router,
    private issueService: IssueService,
    private signalRService: SignalRService,
    public loaderService: LoaderService,
    private mailService: MailService,
    private conversationService: ConversationService
  ) {}
  ngAfterViewInit(): void {
    // this.getUserProcessing();
  }

  ngOnInit(): void {
    if (localStorage.getItem('userData')) {
      this.checkUserAmin();
      this.signalRService.SetConnection();
      this.getListProject();
      //this.getListProjectShow();
      this.getProjectStatistic(this.filter);
      this.ReloadProject();
      localStorage.removeItem('ProjectId');
    } else {
      this.rout.navigateByUrl('/sign-in');
    }
    this.getUpdates();
    this.userLogin = this.shareService.getUserData.id;
  }

  getUserProcessing() {
    this.modalLoading = true;
    this.projectService.getUserProcessing().subscribe(
      data => {
        this.usersProcessing = data;
        this.modalLoading = false;
      },
      err => (this.modalLoading = false)
    );
  }

  getUserDetailsProcessing(userId) {
    this.modalLoading = true;
    var indexUser = this.usersProcessing.findIndex(user => user.id == userId);
    if (this.usersProcessing[indexUser].userProcessingDetailDtos == null) {
      this.projectService.getUserDetailsProcessing(userId).subscribe(
        data => {
          this.modalLoading = false;
          this.usersProcessing[indexUser].userProcessingDetailDtos = data;
        },
        err => (this.modalLoading = false)
      );
    } else {
      this.usersProcessing[indexUser].userProcessingDetailDtos = null;
      this.modalLoading = false;
    }
  }

  onInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.options = value ? [value + '@gmail.com'] : [];
  }

  // listenReloadDashboard() {
  //   // this.signalRService.connection.on('ReloadNotify', data => {
  //   //   if (data == 'Reload') {
  //   //     this.getListProject();
  //   //     this.getProjectStatistic(this.filter);
  //   //     this.getUpdates();
  //   //   }
  //   // });

  //   this.signalRService.connection.on('SignIn', () => {
  //     console.log('SignIn')
  //     location.reload();
  //   });
  // }

  handleCancelMess(): void {
    this.isVisibleShowMess = false;
  }
  showMesss(): void {
    this.isVisibleShowMess = true;
  }
  gridStyle = {
    width: '16.66%',
    textAlign: 'left',
  };
  Redirect(id) {
    localStorage.setItem('ProjectId', id);
    this.signalRService.connection.invoke('ReloadBugAssign', id, [
      this.shareService.getUserData.id,
    ]);
    this.rout.navigateByUrl('/project/' + id + '/home');
  }
  clickBoard(id) {
    localStorage.setItem('ProjectId', id);
    this.rout.navigateByUrl('/project/' + id + '/board');
  }
  clickAddIssue(id) {
    localStorage.setItem('ProjectId', id);
    this.rout.navigateByUrl('/project/' + id + '/add-issue');
  }
  getProjectStatistic(filter) {
    this.projectService
      .getProjectStatistic(filter, this.shareService.getUserData.id)
      .subscribe(data => {
        this.projectStatistic = data;
        this.dataLineProject = {
          labels: data['projects'],
          datasets: [
            {
              type: 'bar',
              label: 'Created',
              data: data['issuesCreated'],
              borderColor: 'rgba(16, 142, 233)',
              backgroundColor: 'rgba(255, 85, 0, 0.5)',
            },
            {
              type: 'line',
              label: 'Closed',
              lineTension: 0.05,
              data: data['issuesClosed'],
              fill: false,
              borderColor: 'rgb(135, 208, 104,0.8)',
            },
            {
              type: 'line',
              label: 'Over DueDate',
              lineTension: 0.05,
              data: data['issuesOverDueDate'],
              fill: false,
              borderColor: 'rgba(255, 0, 0, 0.7)',
            },
          ],
        };
        var maxIssue = Math.max.apply(Math, this.dataLineProject['datasets'][0]['data']);
        this.createLineChart(
          maxIssue / 10 > 0 ? Math.floor(maxIssue / 10) : 1,
          this.dataLineProject
        );
      });
  }
  createLineChart(stepSize, data) {
    new Chart(this.line.nativeElement, {
      type: 'bar',
      data: data,
      options: {
        title: {
          display: true,
          text: 'Tasks by project',
        },
        scales: {
          yAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
                // steps: 10,
                stepSize: stepSize,
              },
            },
          ],
        },
      },
    });
  }
  getListUser() {
    this.userService.getListUserAddProject(this.idProjectAdd).subscribe(data => {
      this.listUser = data.items;
    });
  }
  addMemberToProject() {
    // for (let i = 0; i < this.selectedUserAssign.length; i++) {
    //   const data = {
    //     ProjectID: this.idProjectAdd,
    //     UserID: this.selectedUserAssign[i],
    //   };
    //   this.memberService.CreateMember(data).subscribe(() => {
    //     this.connectionHub.invoke('ReloadProject', data.UserID);
    //   });
    // }
  }
  ReloadProject() {
    this.signalRService.connection.on('ReloadProject', userIds => {
      if (userIds.includes(this.shareService.getUserData.id)) {
        this.getListProject();
      }
    });
  }
  checkUserAmin() {
    this.issueService.CheckAdmin(this.shareService.getUserData.id).subscribe(data => {
      if (data == true) {
        this.admin = true;
      } else {
        this.admin = false;
      }
    });
  }
  GetCharAt(name) {
    return name
      .split(' ')
      .map(char => char.charAt(0))
      .join('');
  }
  filterChange(Filter) {
    if (Filter == '') {
      Filter = null;
    }
    this.filter = Filter;
    this.getListProject();
    this.getProjectStatistic(this.filter);
    this.getUpdates();
  }
  filterChangeShow(Filter) {
    if (Filter == '') {
      Filter = null;
    }
    this.filterShow = Filter;
  }

  getListProject() {
    this.modalLoading = true;
    this.projectService
      .getListProjectByUserId(JSON.parse(localStorage.getItem('userData')).id, this.filter)
      .subscribe(
        res => {
          this.modalLoading = false;
          if (res != null) {
            this.isHaveProject = true;
          }
          this.projects = res;
        },
        error => {
          this.shareService.errorHandling(error);
          this.modalLoading = false;
        }
      );
  }

  handleCancelAddProject(): void {
    this.isVisibleAddProject = false;
  }
  handleCancelAddMember(): void {
    this.isVisibleAddMember = false;
  }
  handleOkAddProject(): void {
    this.isAddLoad = true;
    this.projectService.CreateProject({ name: this.valueAddProject }).subscribe(
      data => {
        this.isVisibleAddProject = false;
        this.valueAddProject = null;
        this.getProjectStatistic(this.filter);
        if (this.linkTfsProject && this.linkTfsProject != '' && this.linkTfsProject != undefined) {
          this.projectService
            .SyncProjectFromTfs({ projectId: data.id, url: this.linkTfsProject })
            .subscribe(
              data => {
                this.getListProject();
                this.getUpdates();
                this.isAddLoad = false;
                this.createMessage.createMessage('success', 'Create project successfully!');
              },
              err => {
                this.isAddLoad = false;
                this.getListProject();
                this.getUpdates();
                this.createMessage.createMessage('success', 'Create project successfully!');
                this.createMessage.createMessage('error', err.error.message);
              }
            );
        } else {
          this.getListProject();
          this.getUpdates();
          this.isAddLoad = false;
          this.createMessage.createMessage('success', 'Create project successfully!');
        }
      },
      err => {
        this.createMessage.createMessage('error', err.error.message);
        this.isVisibleAddProject = true;
        this.isAddLoad = false;
      }
    );
  }
  CreateMember(): void {
    this.listIdUser = [];
    if (this.selectedUserAssign == null) {
      this.createMessage.createMessage('error', 'Input Null Value!');
      this.isVisibleAddMember = true;
      return;
    } else {
      const data = {
        ProjectID: this.idProjectAdd,
        userIDs: this.selectedUserAssign,
      };
      this.memberService.CreateByListMember(data).subscribe(() => {
        this.getListProject();
        //message
        this.conversationService.CheckConversation(this.idProjectAdd).subscribe(data => {
          if (data == false) {
            this.conversationService.getNameProject(this.idProjectAdd).subscribe(id => {
              this.userService.getListUserByIdProject(id.id).subscribe(name => {
                name.items.forEach(element => {
                  if (element.id != this.shareService.getUserData?.id) {
                    this.listIdUser.push(element.id);
                  }
                });
                this.createMessage.CreateGroupChat(id.name, this.listIdUser).subscribe(data => {
                  const res = {
                    idProject: this.idProjectAdd,
                    conversationId: data.conversationId,
                  };
                  this.CreateConversationDatabase(res);
                });
              });
            });
          } else {
            if (this.idProjectAdd != null) {
              this.conversationService.getConversationDetail(this.idProjectAdd).subscribe(data => {
                this.createMessage
                  .addMember(data.conversationId, this.selectedUserAssign)
                  .subscribe();
              });
            }
          }
        });

        /////
        this.getProjectStatistic(this.filter);
        this.signalRService.connection.invoke('ReloadProject', this.selectedUserAssign);
        this.createMessage.createMessage('success', 'Add member success!');
        this.isVisibleAddMember = false;
        // this.selectedUserAssign = null;
      });
    }
  }
  checkNameExist(name) {
    this.projectService.getCheckNameExist(name).subscribe(data => {
      this.checktext = data;
    });
  }
  CreateProject(input) {
    this.projectService.CreateProject(input).subscribe(
      () => {
        this.getListProject();
        this.getUpdates();
      },
      err => {
        this.createMessage.createMessage('error', err.error.message);
      }
    );
  }

  showModalAddProject(event): void {
    this.isVisibleAddProject = true;
    event.stopPropagation();
  }
  showModalAddMember(ProjectId): void {
    this.isVisibleAddMember = true;
    this.idProjectAdd = ProjectId;
    this.selectedUserAssign = null;
    this.getListUser();
  }

  getUpdates() {
    if (!this.updatesLoading && !this.listUpdate) {
      this.updatesLoading = true;
    }
    this.isShowMoreLoading = true;
    let skipCount = 0;
    const url = `${this.shareService.REST_API_SERVER}/api/app/dashboard/by-user-id/${this.shareService.getUserData.id}?skipCount=${skipCount}&pageSize=${this.pageSize1}&projectName=${this.filter}`;
    this.shareService.returnHttpClient(url).subscribe(
      data => {
        if (data.length == this.countUpdate) this.load = null;
        this.countUpdate = data.length;
        this.groupBy = data.map(e => {
          e.count = 3;
          e.showMore = e['data'].length < e.count ? false : true;
          return e;
        });

        this.updatesLoading = false;
        this.isShowMoreLoading = false;
        this.listUpdate = data;
      },
      err => {
        this.updatesLoading = false;
        this.isShowMoreLoading = false;
      }
    );
  }
  showUpdates(element, count) {
    return element.slice(0, count);
  }
  @HostListener('scroll', ['$event'])
  onScroll() {
    {
      this.pageSize1 += 10;
      this.getUpdates();
    }
  }
  showMore(element) {
    element['count'] += this.increaseNumber;
    if (element['count'] >= element['data'].length) {
      element['showMore'] = false;
    }
  }
  showModalInvite(projectId): void {
    this.idProjectAdd = projectId;
    this.isVisibleInvite = true;
  }

  handleOkInvite(): void {
    if (!this.email || this.email == '' || !this.emailIsValid(this.email)) {
      this.createMessage.createMessage('error', 'Check your input mail!');
      return;
    }
    this.mailService.SendMailInvite(this.email, this.idProjectAdd).subscribe(() => {
      this.createMessage.createMessage('success', 'Send Mail Invite Successfully!');
      this.isVisibleInvite = false;
    });
  }

  handleCancelInvite(): void {
    this.isVisibleInvite = false;
  }
  emailIsValid(email) {
    return /\S+@\S+\.\S+/.test(email);
  }

  ///messenger
  ////mess
  getlistMemberInProJect(idProject) {
    this.userService.getListUserByIdProject(idProject).subscribe(data => {
      this.MemberInProject = data.items;
      this.MemberInProject.forEach(element => {
        if (element.id != this.shareService.getUserData?.id) {
          this.listIdUser.push(element.id);
        }
      });
    });
  }
  //crate
  CreateConversationDatabase(Data) {
    this.conversationService.CreateConversationInDatabase(Data).subscribe((res: any) => {});
  }
  //getName
  getNameProject(idProject) {
    this.conversationService.getNameProject(idProject).subscribe(data => {
      this.nameProject = data.name;
    });
  }

  showModalInformation(value): void {
    if (value != 'Bugs' && value != 'Comments On Project') {
      if (value == 'Total User Processing') {
        this.getUserProcessing();
      }
      this.valueModalInformation = value;
      if (value == 'Projects') {
        this.getListProject();
      }
      this.isVisibleInformationModal = true;
    }
  }

  handleOkInformation(): void {
    this.isVisibleInformationModal = false;
  }

  handleCancelInformation(): void {
    this.isVisibleInformationModal = false;
  }
  showUserProfile(id) {
    this.idProfileShowing = id;
    this.isVisibleProfile = true;
  }
  handleCancelProfile() {
    this.isVisibleProfile = false;
  }
  //////////////messenger
  CreateConversition() {
    this.isVisibleProfile = false;
    this.createMessage.CheckConversation(this.idProfileShowing).subscribe(data => {
      if (data['hasConversation']) {
        this.conversationId = data['conversationId'];
        this.signalRService.connection.invoke(
          'DrawerChat',
          data['conversationId'],
          this.shareService.getUserData.id
        );
      } else {
        this.createMessage.CreateConversition(this.idProfileShowing).subscribe(data => {
          this.conversationId = data.conversationId;
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
