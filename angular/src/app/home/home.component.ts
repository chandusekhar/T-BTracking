import { AuthService } from '@abp/ng.core';
import { AfterViewInit, Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { CategoryService } from '../Service/Category/category.service';
import { HistoryService } from '../Service/History/history.service';
import { IssueService } from '../Service/Issue/issue.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';
import { MemberService } from '../Service/Member/member.service';
import { ProjectService } from '../Service/Project/project.service';
@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit, AfterViewInit {
  projectLogin;
  //Status
  listStatus: any;
  count: number = 0;
  categoryid: any = [];
  listCountStatus: any = [];
  listCate: any;
  listCategory: any = [];
  last = 0;
  listCountCategory: any = [];
  statusName: any;
  //cat
  listIssue: any;
  totalCountToday;
  OpenPro = 0;
  listHistory: any = [];
  object: any[];
  objectName: any[];
  today = new Date();
  date: any = null;
  startDate: any = null;
  endDate: any = null;
  ExportList: any;

  listUser: any;
  listUserAssign: any;
  listIssueHistory: any;
  listAction: any;
  issueList: any;
  IdUser = null;
  issueId = null;
  color: any;
  action = null;
  pageSize = 20;
  pageIndex = 1;
  idCat = '';
  countCat = 0;
  listStatusofcat: any = [];
  listTest: any = [];
  listTest1: any = [];
  limitPageSize = 0;
  listIdUser = [];
  lastDate: number = 1;
  project1Statistic;
  colSpan = 24;
  //mess
  MemberInProject: any;
  public load = 'Load More';
  isShowMoreLoading = false;
  get hasLoggedIn(): boolean {
    return this.oAuthService.hasValidAccessToken();
  }
  loading = false;
  historyLoading = false;
  allCategoryNull = false;

  constructor(
    public router: Router,
    private rout: ActivatedRoute,
    private memberService: MemberService,
    private oAuthService: OAuthService,
    private authService: AuthService,
    private issueService: IssueService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    public loaderService: LoaderService,
    public historyService: HistoryService,
    private shareService: ShareServiceService,
    public projectService: ProjectService,
  ) {}
  ngAfterViewInit(): void {
    this.getListUserByIdProject();
    this.getListIssue();
    this.getListCatByIdProject();
    this.getStatusStatistics();
    this.getProject1Statistic(this.lastDate);
  }
  ngOnInit(): void {
    this.projectLogin = this.rout.snapshot.params.idProject;
    this.checkUserInProject(this.projectLogin);
    this.getHistory();
    this.totalCountToday = this.getCountDate(this.today);
    this.shareService.reloadRouter();
  }

  setRowSpan(length) {
    return Math.floor(24 / length);
  }

  getProject1Statistic(lastDate) {
    this.projectService.getProject1Statistic(this.projectLogin, lastDate).subscribe(data => {
      this.project1Statistic = data;
    });
  }
  GetCharAt(name) {
    var rs = '';
    name.split(' ').forEach(char => (rs += char.charAt(0)));
    return rs;
  }

  changeLasDate() {
    this.getProject1Statistic(this.lastDate);
  }

  getStatusStatistics() {
    this.statusService.getStatusStatistics(this.projectLogin).subscribe(data => {
      this.listStatus = data;
    });
  }

  getCat(cat) {
    this.idCat = cat;
    this.getStatusStatistics();
  }
  login() {
    this.authService.navigateToLogin();
  }
  // getCatBy Project
  getListCatByIdProject() {
    this.categoryService.getListCategoryByProject(this.projectLogin).subscribe(data => {
      this.listCate = data;
      this.allCategoryNull = data.filter(e => e.count > 0).length > 0;
    });
  }

  GetIdUser(id) {
    this.IdUser = id;
    this.getHistory();
  }
  //action
  getAction(Action) {
    this.action = Action;
    this.getHistory();
  }
  //issue Name
  getIssue(issue) {
    this.issueId = issue;
    this.getHistory();
  }
  //scroll
  onScroll() {
    this.pageSize += 10;
    this.getHistory();
  }
  //getHistory
  getHistory() {
    let skipCount = (this.pageIndex - 1) * this.pageSize;
    if (!this.historyLoading && this.limitPageSize == 0) {
      this.historyLoading = true;
    }
    this.isShowMoreLoading = true;
    var newDate;
    var newMonth;
    var newDate1;
    var newMonth1;
    var StartDate = '';
    var EndDate = '';
    if (this.startDate == null) {
      this.startDate = null;
    } else {
      var resultDate = this.startDate.getDate();
      if (resultDate < 10) {
        newDate = '0' + resultDate.toString();
      } else newDate = resultDate.toString();
      var resultMonth = this.startDate.getMonth() + 1;
      if (resultMonth < 10) {
        newMonth = '0' + resultMonth.toString();
      } else newMonth = resultMonth.toString();
      StartDate = this.startDate.getFullYear() + '-' + newMonth + '-' + newDate;
    }
    //
    if (this.endDate == null) {
      this.endDate = null;
    } else {
      var resultDate1 = this.endDate.getDate() + 1;
      if (resultDate1 < 10) {
        newDate1 = '0' + resultDate1.toString();
      } else newDate1 = resultDate1.toString();
      var resultMonth1 = this.endDate.getMonth() + 1;
      if (resultMonth1 < 10) {
        newMonth1 = '0' + resultMonth1.toString();
      } else newMonth1 = resultMonth1.toString();
      EndDate = this.endDate.getFullYear() + '-' + newMonth1 + '-' + newDate1;
    }
    this.historyService
      .GetHistory(
        this.projectLogin,
        StartDate,
        EndDate,
        this.IdUser,
        this.action,
        skipCount,
        this.pageSize,
        this.issueId
      )
      .subscribe(
        data => {
          this.limitPageSize = data.count;
          this.listHistory = Object.keys(data.result).map(key => [key, data.result[key]]);
          if (this.pageSize >= this.limitPageSize + 10) {
            this.load = null;
          }
          this.historyLoading = false;
          this.isShowMoreLoading= false;
        },
        err => {
          this.historyLoading = false;
          this.isShowMoreLoading = false;
        }
      );
  }
  ///startdate
  onChangeDueDate(result: Date): void {
    this.startDate = result;
    this.getHistory();
  }
  //endDate
  EndDate(result: Date): void {
    this.endDate = result;
    this.getHistory();
  }

  convertToDate(string) {
    return new Date(string);
  }
  getCountDate(day) {
    return (
      Number.parseInt((day.getFullYear() * 12 * 30).toString()) +
      Number.parseInt(((day.getMonth() + 1) * 12).toString()) +
      Number.parseInt(day.getDate())
    );
  }

  //Get list user
  getListUserByIdProject() {
    this.historyService.getListUserByIdProject(localStorage.getItem('ProjectId')).subscribe(data => {
      if (data) {
        this.listUser = data;
        // console.log(this.listUser);
      }
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

  //get list Issue
  getListIssue() {
    this.issueService
      .getListIssueByIdProject('', localStorage.getItem('ProjectId'), '')
      .subscribe(data => {
        this.issueList = data;
      });
  }
}
