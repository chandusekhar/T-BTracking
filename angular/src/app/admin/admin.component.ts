import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { NzModalRef, NzModalService } from 'ng-zorro-antd/modal';
import { IssueService } from '../Service/Issue/issue.service';
import { CreateMessageService } from '../Service/Message/create-message.service';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';
import * as Chart from 'chart.js';
import { StatusService } from '../Service/Status/status.service';
import { CategoryService } from '../Service/Category/category.service';
import { ExportFileService } from '../Service/Export/export-file.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { Router } from '@angular/router';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})
export class AdminComponent implements OnInit, AfterViewInit {
  @ViewChild('lineCanvas') lineCanvas: ElementRef;
  @ViewChild('monthChart') monthChart: ElementRef;
  @ViewChild('pie') pie: ElementRef;
  @ViewChild('yearChart') yearChart: ElementRef;
  lineChart: any;
  monthchart: any;
  yearchart: any;
  Pie: any;
  filter: string = "";
  filterProject: string = "";
  listUser;
  search;
  dataDuedate = [
    { name: '3 day', value: 3 },
    { name: '5 day', value: 5 },
    { name: '7 day', value: 7 },
    { name: '1 month', value: 30 },
  ];
  inputIssue;
  listUserOData;
  dueDateIssue = 0;
  idUserIssue = null;
  idProjectIssue = null;
  idUserAssign = '';
  listIssue;
  idStatus = null;
  idCategory = null;
  filterIssue = '';
  stepMonth: number;
  stepYear: number;
  maxMonth: number;
  maxYear: number;
  userId = null;
  input;
  pageIndex = 1;
  totalCount = 0;
  pageSize = 9;
  countOpen: number = 0;
  countDuedate: number = 0;
  countFinish: number = 0;
  countAdmin: number = 0;
  countUser: number = 0;
  countAssgin: number = 0;
  countStatusPercent = [];
  lableMonth = [];
  createMonth = [];
  Color = [];
  finishMonth = [];
  StatusName = [];
  countStatus = [];
  lableYear = [];
  createYear = [];
  finishYear = [];
  listStatus;
  listCategory;
  DueDate = 0;
  listProject;
  listProjectOData;
  idProject = '';
  checkAdmin: boolean = false;
  confirmModal?: NzModalRef;
  userLogin;
  isVisibleExport = false;
  radioValue;
  spreadSheetId;
  sheetName;
  urlSheet;
  constructor(
    private userService: UserService,
    private modal: NzModalService,
    private createMessage: CreateMessageService,
    private shareService: ShareServiceService,
    private issueService: IssueService,
    private projectService: ProjectService,
    private statusService: StatusService,
    private categoryService: CategoryService,
    private exportService: ExportFileService,
    public loaderService: LoaderService,
    public router: Router
  ) { }

  ngOnInit(): void {
    this.getListUserOData();
    this.getListProjectOData();
    this.userLogin = this.shareService.getUserData.id;
    this.checkUserAmin();
    this.getCharIssueByProject();
    this.getListStatusByIdProject(this.idProject);
    this.getListIssue();
    this.getListStatus();
    this.getListCategory();
  }
  ngAfterViewInit(): void {
    this.chartMonth();
    this.chartYear();
    this.PieStatusMethod();
  }
  routerBug(projectId, issueId) {
    localStorage.setItem('ProjectId', projectId);
    this.router.navigate(['project/' + projectId + '/issues/view/' + issueId]);
  }
  filterChange(Filter) {
    if (Filter == null) {
      Filter = '';
    }
    this.filter = Filter;
    this.getListUserOData();
  }
  filterChangeShow(Filter) {
    if (Filter == null) {
      Filter = '';
    }
    this.filterProject = Filter;
    this.getListProjectOData();
  }
  filterChangeIssue(Filter) {
    if (Filter == null) {
      Filter = "";
    }
    this.filterIssue = Filter;
    this.getListIssue();
  }
  getListUserOData() {
    this.countAssgin = 0;
    var regex = /^[0-9]+$/;
    if (regex.test(this.filter)) {
      var url = `${this.shareService.REST_API_SERVER}/odata/AdminOData?$Filter=contains(PhoneNumber, '${this.filter}')`
    }
    else {
      var url = `${this.shareService.REST_API_SERVER}/odata/AdminOData?$filter=contains(Name, '${this.filter}')`
    }
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listUser = data?.value;
      this.listUser.forEach(t => {
        this.countAssgin = this.countAssgin + t.CountIssueAssign;
      });
    })
  }
  getListIssue() {
    let skipCount = (this.pageIndex-1) * (this.pageSize);
    this.countOpen = 0;
    this.countDuedate = 0;
    this.countFinish = 0;
    var url = `${this.shareService.REST_API_SERVER}/odata/AdminOData/ODataService.GetListIssue(UserId='${this.idUserAssign}')?$filter=contains(Name, '${this.filterIssue}')`;
    if (this.idProjectIssue != null) {
      url += ` and ProjectName eq '${this.idProjectIssue}'`;
    }
    if (this.idUserIssue != null) {
      url += ` and UserName eq '${this.idUserIssue}'`;
    }
    if (this.idStatus != null) {
      url += ` and StatusName eq '${this.idStatus}'`;
    }
    if (this.idCategory != null) {
      url += ` and CategoryName eq '${this.idCategory}'`;
    }
    url += `&$top=${this.pageSize}&$skip=${skipCount}&$count=true`;
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listIssue = data?.value;
      this.totalCount = data['@odata.count'];
      this.listIssue.forEach(t => {
        if (t.StatusName == 'Open') this.countOpen++;
        if (t.DueDate != null) this.countDuedate++;
        if (t.FinishDate != null) this.countFinish++;
      });
    })
  }
  getListProjectOData() {
      var url = `${this.shareService.REST_API_SERVER}/odata/AdminOData/ODataService.getListProject()?$filter=contains(Name, '${this.filterProject}')`;
    if(this.userId != null) {
      url += ` and userName eq '${this.userId}'`;
    }
    this.shareService.returnHttpClient(url).subscribe(data => {
      this.listProjectOData = data?.value;
    })
  }
  getByProject(idproject) {
    if (idproject == '') {
      idproject = null;
    }
    this.idProjectIssue = idproject;
    this.getListIssue();
  }
  getUserAssign(idAssign) {
    if (idAssign == '') {
      idAssign = null;
    }
    this.idUserAssign = idAssign;
    this.getListIssue();
  }
  getUserName(userId) {
    if (userId == "") userId = null;
    this.userId = userId;
    this.getListProjectOData();
  }
  getUserNameIssue(idUser) {
    if (idUser == '') {
      idUser = null;
    }
    if (this.dueDateIssue == null) this.dueDateIssue = 0;
    this.idUserIssue = idUser;
    this.getListIssue();
  }
  gridStyle = {
    width: '20%',
    textAlign: 'left',
  };
  DateChange(dueDate) {
    if (dueDate == null || dueDate == 'null' || this.DueDate == null) {
      dueDate = 0;
      this.DueDate = 0;
    } else {
      this.DueDate = dueDate;
    }
  }
  getByStatus(input) {
    if (input == '') {
      input = null;
    }
    this.idStatus = input;
    this.getListIssue();
  }
  getByCategory(input) {
    if (input == '') {
      input = null;
    }
    this.idCategory = input;
    this.getListIssue();
  }
  DateChangeIssue(date) {
    if (date == '') {
      date = 0;
    }
    this.dueDateIssue = date;
    this.getListIssue();
  }
  RemoveUser(UserId): void {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn xóa người dùng này?',
      nzContent:
        'Khi xóa người dùng thì sẽ xóa tất cả project,issue,category... mà người này tạo! và những thẻ được tag',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.userService.DeleteUser(UserId).subscribe(
            () => {
              this.getListUserOData();
              this.createMessage.createMessage('Thành công', 'Đã xóa User!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }
  RemoveProject(ProjectId) {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn xóa project này?',
      nzContent: 'Khi xóa project thì sẽ xóa tất cả issue,member... của project này',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.projectService.deleteProject(ProjectId).subscribe(
            () => {
              this.getListProjectOData();
              this.createMessage.createMessage('Thành công', 'Đã xóa Project!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }
  RemoveIssue(IssueId) {
    this.confirmModal = this.modal.confirm({
      nzTitle: 'Bạn muốn xóa issue này?',
      nzContent: 'Khi xóa issue thì sẽ xóa tất cả member được assgign',
      nzOnOk: () =>
        new Promise((resolve, reject) => {
          setTimeout(Math.random() > 0.5 ? reject : reject, 1000);
        }).catch(() => {
          this.issueService.DeleteIssue(IssueId).subscribe(
            () => {
              this.getListIssue();
              this.createMessage.createMessage('Thành công', 'Đã xóa issue!');
            },
            err => this.createMessage.createMessage('error', err.error.message)
          );
        }),
    });
  }
  getListStatus() {
    this.statusService.getListStatus().subscribe(data => {
      this.listStatus = data;
    });
  }
  getListCategory() {
    this.categoryService.getListCategory().subscribe(data => {
      this.listCategory = data.items;
    });
  }
  checkUserAmin() {
    this.issueService.CheckAdmin(this.shareService.getUserData.id).subscribe(data => {
      if (data == true) {
        this.checkAdmin = true;
      } else {
        this.checkAdmin = false;
      }
    });
  }
  getCharIssue(idProject) {
    this.idProject = idProject;
    this.getCharIssueByProject();
    this.getListStatusByIdProject(idProject);
  }

  getCharIssueByProject() {
    this.projectService.getChartIssueAdmin(this.idProject).subscribe(data => {
      this.lableYear = [];
      this.createYear = [];
      this.finishYear = [];
      this.lableMonth = [];
      this.createMonth = [];
      this.finishMonth = [];
      data.forEach(t => {
        this.lableYear.push(t.year);
        this.createYear.push(t.countCreate);
        this.finishYear.push(t.countFinish);
        t.data.forEach(element => {
          this.lableMonth.push(element.month);
          this.createMonth.push(element.countCreate);
          this.finishMonth.push(element.countFinish);
        });
      });
      if (
        Math.max.apply(Math, this.createYear) < 10 &&
        Math.max.apply(Math, this.finishYear) < 10
      ) {
        this.stepYear = 1;
        this.maxYear = 10;
      } else {
        if (Math.max.apply(Math, this.createYear) > Math.max.apply(Math, this.finishYear)) {
          this.maxYear = Math.max.apply(Math, this.createYear);
          this.stepYear = Math.round(this.maxYear / 10);
        } else {
          this.maxYear = Math.max.apply(Math, this.finishYear);
          this.stepYear = Math.round(this.maxYear / 10);
        }
      }
      if (
        Math.max.apply(Math, this.createMonth) < 10 &&
        Math.max.apply(Math, this.finishMonth) < 10
      ) {
        this.stepMonth = 1;
        this.maxMonth = 10;
      } else {
        if (Math.max.apply(Math, this.createMonth) > Math.max.apply(Math, this.finishMonth)) {
          this.maxMonth = Math.max.apply(Math, this.createMonth);
          this.stepMonth = Math.round(this.maxMonth / 10);
        } else {
          this.maxMonth = Math.max.apply(Math, this.finishMonth);
          this.stepMonth = Math.round(this.maxMonth / 10);
        }
      }

      this.monthchart.destroy();
      this.yearchart.destroy();
      this.chartMonth();
      this.chartYear();
    });
  }

  getListStatusByIdProject(projectId) {
    this.idProject = projectId;
    this.statusService.getListStatusPie(this.idProject).subscribe(data => {
      this.countStatus = [];
      this.Color = [];
      this.StatusName = [];
      data.forEach(element => {
        this.countStatus.push(element.countIssue);
        this.Color.push(element.nzColor);
        this.StatusName.push(element.name);
        this.countStatusPercent.push(element.countIssuePercent);
      });
      this.Pie.destroy();
      this.PieStatusMethod();
    });
  }
  handleOkExport() {
    if (this.radioValue) {
      if (this.radioValue == 'New') {
        this.exportService.NewSheet().subscribe(
          data => {
            if (data) {
              this.urlSheet = data.spreadsheetUrl;
              this.createMessage.createMessage('success', 'create new sheet successfully!!!');
            }
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      } else if (this.radioValue == 'Update') {
        if (!this.spreadSheetId || !this.sheetName) {
          this.createMessage.createMessage('error', 'check your input value!!!');
          return;
        }
        this.exportService.UpdateSheet(this.spreadSheetId, this.sheetName).subscribe(
          () => {
            this.createMessage.createMessage('success', 'your sheet has update successfully!!!');
          },
          err => this.createMessage.createMessage('error', err.error.message)
        );
      }
    }
  }

  handleCancelExport(): void {
    this.isVisibleExport = false;
  }
  exportModal() {
    this.isVisibleExport = true;
  }
  copyTo() {
    if (this.urlSheet) {
      navigator.clipboard.writeText(this.urlSheet);
      this.createMessage.createMessage('success', 'Copied the text: ' + this.urlSheet);
    }
  }
  PieStatusMethod() {
    this.Pie = new Chart(this.pie.nativeElement, {
      type: 'pie',

      data: {
        labels: this.StatusName,
        datasets: [
          {
            label: 'Dataset 1',
            data: this.countStatus,
            backgroundColor: this.Color,
          },
        ],
      },
      options: {
        title: {
          display: true,
          text: 'Status pie chart',
        },
        animation: {
          animateScale: true,
          animateRotate: true,
        },
        legend: {
          display: true,
          position: 'top',
        },
      },
    });
  }

  chartMonth() {
    this.monthchart = new Chart(this.monthChart.nativeElement, {
      type: 'line',
      data: {
        labels: this.lableMonth,
        datasets: [
          {
            label: 'Bugs Finished',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(75,192,192,0.4)',
            borderColor: 'rgba(75,192,192,1)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(75,192,192,1)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.finishMonth,
            spanGaps: false,
          },
          {
            label: 'Bugs Added',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'red',
            borderColor: 'red',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(75,192,192,1)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.createMonth,
            spanGaps: false,
          },
        ],
      },
      options: {
        title: {
          display: true,
          text: 'Tasks by month',
        },
        scales: {
          yAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
                beginAtZero: true,
                stepSize: this.stepMonth,
                max: this.maxMonth,
              },
              gridLines: {
                display: false,
              },
            },
          ],
          xAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
              },
              gridLines: {
                display: false,
              },
            },
          ],
        },
      },
    });
  }
  chartYear() {
    this.yearchart = new Chart(this.yearChart.nativeElement, {
      type: 'line',
      data: {
        labels: this.lableYear,

        datasets: [
          {
            label: 'Bugs Finished',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(75,192,192,0.4)',
            borderColor: 'rgba(75,192,192,1)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(75,192,192,1)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.finishYear,
            spanGaps: false,
          },

          {
            label: 'Bugs Added',
            fill: false,

            lineTension: 0.05,
            backgroundColor: 'red',
            borderColor: 'red',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(75,192,192,1)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.createYear,
            spanGaps: false,
          },
        ],
      },
      options: {
        title: {
          display: true,
          text: 'Tasks by year',
        },
        scales: {
          yAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
                beginAtZero: true,
                stepSize: this.stepYear,
                max: this.maxYear,
              },
              gridLines: {
                display: false,
              },
            },
          ],
          xAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
              },
              gridLines: {
                display: false,
              },
            },
          ],
        },
      },
    });
  }
}
