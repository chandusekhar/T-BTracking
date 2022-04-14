import { ProjectService } from './../Service/Project/project.service';
import { AfterViewInit, Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ShareServiceService } from '../Service/share-service.service';
import { MemberService } from '../Service/Member/member.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { Confirmation, ConfirmationService } from '@abp/ng.theme.shared';
import { LoaderService } from '../Service/Loader/loader.service';
import { IssueService } from '../Service/Issue/issue.service';
import { StatusService } from '../Service/Status/status.service';
import { ConversationService } from '../Service/Conversation/conversation.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
import { SignalRService } from '../Service/SignalR/signal-r.service';

@Component({
  selector: 'app-setting',
  templateUrl: './setting.component.html',
  styleUrls: ['./setting.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class SettingComponent implements OnInit, AfterViewInit {
  @Input() projectNameChild: string = '';
  project;
  userLogin;
  projectName = '';
  currentProject;
  projectColor = '#363636';
  projectColorChange;
  isSync = false;
  projectTfs;
  witType;
  isVisible = false;
  isKeepCurrentBugs = false;
  issues;
  checked = false;
  listIssueSync = [];
  Filter = '';
  Status = 'null';
  listStatus;
  error;
  pageIndex = 1;
  pageSize = 999;
  witTypes;
  icon = 'fas fa-bug';
  state = 'null';
  description;
  wITs;
  states = [];
  isFirstSate = false;
  type = 'null';
  types = [];
  constructor(
    public router: Router,
    private projectService: ProjectService,
    private shareService: ShareServiceService,
    private memberService: MemberService,
    private rout: ActivatedRoute,
    private createMessage: CreateMessageService,
    private confirmation: ConfirmationService,
    public loaderService: LoaderService,
    private issueService: IssueService,
    private statusService: StatusService,
    private signalRService: SignalRService,
    private conversationService: ConversationService
  ) {}
  ngAfterViewInit(): void {
    this.GetListStatus();
    this.witTypes = this.shareService.witType;
  }

  ngOnInit(): void {
    this.currentProject = this.rout.snapshot.params.idProject;
    this.checkUserInProject(this.currentProject);
    this.getProject();
    this.userLogin = this.shareService.getUserData.id;
    this.getWITsByProjectTfs();
  }

  getWITsByProjectTfs() {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    this.issueService
      .getWITsByProjectTfs(this.currentProject, this.pageSize, skipCount, this.Filter, this.state, this.type)
      .subscribe(data => {
        this.wITs = data;
        console.log(data)
        if(data.state && data.type){
          if (data.state.length > 1) {
            this.states = data.state;
          }
          if(data.type.length > 1){
            this.types = data.type;
          }
        }
      });
  }

  typeChange(event){
    this.type = event;
    this.getWITsByProjectTfs();
  }

  routerIssue(name) {
    this.issueService.getIssueByName(this.currentProject, name).subscribe(data => {
      this.router.navigate(['project/' + this.currentProject + '/issues/view/' + data]);
    });
  }

  filterType(event) {
    this.getWITsByProjectTfs();
  }

  listenProjectName(event) {
    this.projectName = event;
  }
  getColor(color) {
    this.projectColor = color;
  }
  submit() {
    this.editProject();
  }
  editProject() {
    const dataProject = {
      name: this.projectName,
      nzColor: this.projectColor,
      witType: this.witType,
      Description: this.description,
    };
    this.projectService.PutProject(dataProject, localStorage.getItem('ProjectId')).subscribe(
      () => {
        this.createMessage.createMessage('success', 'successfully');
        this.getProject();
        this.signalRService.connection.invoke('ReloadProjectName');
        console.log(this.userLogin);
        if (this.projectColorChange != this.projectColor) {
          window.location.reload();
        }
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
  }
  RemoveProject() {
    this.confirmation
      .warn(
        '::If you have synced with TFS, this will delete the project on TFS, you are sure ?',
        ':: Delete project'
      )
      .subscribe(status => {
        if (status === Confirmation.Status.confirm) {
          this.conversationService
            .CheckConversation(localStorage?.getItem('ProjectId'))
            .subscribe(data => {
              if (data) {
                this.conversationService
                  .getConversationDetail(localStorage.getItem('ProjectId'))
                  .subscribe(res => {
                    this.createMessage.delete(res.conversationId).subscribe();
                  });
              }
            });
          this.projectService.deleteProject(localStorage.getItem('ProjectId')).subscribe(
            () => {
              this.createMessage.createMessage('success', 'Delete successfully!');
              this.router.navigate(['']);
            },
            err =>
              err.error.message
                ? this.createMessage.createMessage('error', err.error.message)
                : this.shareService.errorHandling(err)
          );
        }
      });
  }
  getTfsProject(id) {
    this.projectService.getTfsProject(id, this.shareService.getUserData.id).subscribe(
      data => {
        if (data) {
          this.projectTfs = data;
          this.description = data.description;
        }
      },
      err => (this.error = err.error.message)
    );
  }
  getProject() {
    this.projectService.getProjectByID(localStorage.getItem('ProjectId')).subscribe(data => {
      if (data.projectIdTFS != '00000000-0000-0000-0000-000000000000') {
        this.isSync = true;
        this.getTfsProject(data.projectIdTFS);
      } else {
        this.isSync = false;
      }
      this.witType = data.witType;
      this.projectName = data.name;
      this.project = data;
      this.projectColor = data.nzColor;
      this.projectColorChange = data.nzColor;
    });
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
  syncToTFS() {
    if (!this.isSync) {
      const data = {
        name: this.projectName,
        witType: this.witType,
        listIssueSync: this.listIssueSync,
        description: this.description,
      };
      this.projectService.getSyncToTFS(data).subscribe(
        () => {
          this.getProject();
          this.getIssues(this.currentProject);
          this.createMessage.createMessage('success', 'Synced to tfs successfully');
          this.isVisible = false;
          this.getWITsByProjectTfs();
        },
        err =>
          err.error.message
            ? this.createMessage.createMessage('error', err.error.message)
            : this.shareService.errorHandling(err)
      );
    } else {
      this.createMessage.createMessage('error', 'Project synced to tfs!');
    }
  }
  onChangeWitType(value) {
    this.witType = value;
  }
  onChangeKeepCurrentBugs(value) {
    if (!value) {
      this.listIssueSync = [];
      this.checked = false;
    }
    this.isKeepCurrentBugs = value;
  }
  getDeleteSync(projectId) {
    this.projectService.getDeleteSyncTfs(projectId, this.shareService.getUserData.id).subscribe(
      () => {
        this.getProject();
        this.projectTfs = null;
        this.getIssues(this.currentProject);
        this.description = '';
        this.createMessage.createMessage('success', 'Delete synced to tfs successfully!');
        this.getWITsByProjectTfs();
      },
      err =>
        err.error.message
          ? this.createMessage.createMessage('error', err.error.message)
          : this.shareService.errorHandling(err)
    );
  }
  showModal(): void {
    if (!this.isSync) {
      this.getIssues(this.currentProject);
      this.isVisible = true;
    } else {
      this.getDeleteSync(this.currentProject);
    }
  }

  handleOk(): void {
    this.syncToTFS();
  }

  handleCancel(): void {
    this.isVisible = false;
  }
  getIssues(projectId) {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    this.issueService
      .getListIssueResult(skipCount, this.pageSize, projectId, this.Filter, '', this.Status, '')
      .subscribe(data => {
        if (data) {
          this.issues = data;
        }
      });
  }
  nzPageIndexChange(event) {
    this.pageIndex = event;
    this.getWITsByProjectTfs();
  }
  onChangeChecked(issue) {
    if (this.listIssueSync.includes(issue)) {
      let index = this.listIssueSync.lastIndexOf(issue);
      this.listIssueSync.splice(index, 1);
    } else {
      this.listIssueSync.push(issue);
    }
    if (this.listIssueSync.length == this.issues.items.length) {
      this.checked = true;
    } else {
      this.checked = false;
    }
  }
  onChangeCheckedAll() {
    if (this.checked) {
      this.checked = false;
      this.listIssueSync = [];
    } else {
      this.checked = true;
      this.listIssueSync = [];
      this.issues.items.forEach(element => {
        this.listIssueSync.push(element);
      });
    }
  }
  GetListStatus() {
    this.statusService.getList().subscribe(data => {
      this.listStatus = data;
    });
  }
  onChangeStatus(event) {
    this.state = event;
    this.getWITsByProjectTfs();
  }
  onChangeFilter(value) {
    this.Filter = value;
    this.getWITsByProjectTfs();
  }
}
