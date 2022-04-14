import { Component,OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NzButtonSize } from 'ng-zorro-antd/button';
import { CategoryService } from '../Service/Category/category.service';
import { IssueService } from '../Service/Issue/issue.service';
import { MemberService } from '../Service/Member/member.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';
import { UserService } from '../Service/User/user.service';
import { LoaderService } from '../Service/Loader/loader.service';
import { SignalRService } from '../Service/SignalR/signal-r.service';
import {
  trigger,
  style,
  animate,
  transition,
  // ...
} from '@angular/animations';
import { HistoryService } from '../Service/History/history.service';
@Component({
  selector: 'app-issues',
  templateUrl: './issues.component.html',
  styleUrls: ['./issues.component.scss'],
  animations: [
    trigger('fadeIn', [
      transition(':enter', [
        style({ opacity: '0' }),
        animate('.5s ease-out', style({ opacity: '1' })),
      ]),
    ]),
  ],
})


export class IssuesComponent implements OnInit {
  isShowFilter = false;
  loading = true;
  selectedLevel;
  pageIndex = 1;
  pageSize = 10;
  totalCount = 1;
  listOfData = [];
  searchText: string = "";
  visible = false;
  projectLogin;
  listCategory:any;
  listProject : any;
  listStatus : any;
  listIsssueByProject:any;
  listIssueByName: any;
  Issue: any;
  listUser: any;
  filterStatus="All";
  // edit issue
  dataIssue = {
    Description: '',
  };
  theader = [
    {
      key: 'projectName',
      status: false,
    },
    {
      key: 'name',
      status: true,
    },
    {
      key: 'description',
      status: false,
    },
    {
      key: 'categoryName',
      status: true,
    },
    {
      key: 'priorityValue',
      status: true,
    },
    {
      key: 'priority',
      status: false,
    },
    {
      key: 'levelValue',
      status: true,
    },
    // {
    //   key: 'issueLevel',
    //   status: false,
    // },
    {
      key: 'statusName',
      status: true,
    },
    {
      key: 'userName',
      status: true,
    },
    {
      key: 'startDate',
      status: true,
    },
    {
      key: 'dueDate',
      status: true,
    },
    {
      key: 'finishDate',
      status: true,
    },
    {
      key: 'DueTime',
      status: true,
    },
  ];

  //pgaination;
  size: NzButtonSize = 'large';
  isVisible = false;
  isVisible1 = false;
  selectedValue = '';
  selectedCategory?:string;
  selectedAssignee?:string;
  selectedIssue?:string;
  selectedUser?: string;
  selectedProject?:any;
  public connectionHub;
  idCategory:any = null;
  Filter:any=null;
  idStatus:any = null;
  isOkLoading = false;
  idIssue : any;
  userName:any = null;
  calculateProgress: any;
  strokeColor: any;
  ////history
  historyLoading = false;
  limitPageSize = 0;
  isShowMoreLoading = false;
  startDate: any = null;
  endDate: any = null;

  constructor(
    private rout:ActivatedRoute,
    public router: Router,
    private categoryService : CategoryService,
    private issueService: IssueService,
    private statusService: StatusService,
    private userService: UserService,
    private shareService: ShareServiceService,
    private memberService: MemberService,
    public loaderService:LoaderService,
    public historyService: HistoryService,
    private signalRService: SignalRService
  ) {
  }

  ngOnInit(): void {
    this.projectLogin=this.rout.snapshot.params.idProject;
    this.checkUserInProject( this.projectLogin);
    this.getListCatByIdProject();
    this.getListStatus();
    this.searchData();
    this.getListUserByIdProject();
    this.signalRService.SetConnection();
    this.connectionHub = this.signalRService.connection;
    // this.calculateProgress = this.percentDueDate({
    //   start: listEvent[index].start,
    //   due: listEvent[index].extendedProps.issueProperties.dueDate,
    //   finish: listEvent[index].extendedProps.issueProperties.finishDate,
    // });
  }
  getColor(percent, finish) {
    if (finish == true && percent != 0) {
      return {
        //textColor: 'red',
        strokeColor: '#52c41a',
        //fontWeight: 'bold',
      };
    }
    if (percent <= 25) {
      return {
        textColor: 'red',
        strokeColor: 'red',
        fontWeight: 'bold',
      };
    }
    if (percent <= 50) {
      return {
        textColor: '#ff8e23',
        strokeColor: '#ff8e23',
        fontWeight: 'bold',
      };
    }
    return {
      textColor: '#929292',
      strokeColor: '#1890ff',
      fontWeight: 'normal',
    };
  }

  parseDuration(duration) {
    let remain = duration;

    let days = Math.floor(remain / (1000 * 60 * 60 * 24));
    remain = remain % (1000 * 60 * 60 * 24);

    let hours = Math.floor(remain / (1000 * 60 * 60));
    remain = remain % (1000 * 60 * 60);

    let minutes = Math.floor(remain / (1000 * 60));
    remain = remain % (1000 * 60);

    let seconds = Math.floor(remain / 1000);
    remain = remain % 1000;

    let milliseconds = remain;

    return {
      days,
      hours,
      minutes,
      seconds,
      milliseconds,
    };
  }

  formatTime(o, useMilli = false) {
    let parts = [];
    if (o.days) {
      let ret = o.days + 'd';
      if (o.days !== 1) {
        ret += 's';
      }
      parts.push(ret);
    }
    if (o.hours) {
      let ret = o.hours + 'hr';
      if (o.hours !== 1) {
        ret += 's';
      }
      parts.push(ret);
    }
    if (o.minutes) {
      let ret = o.minutes + 'm';
      if (o.minutes !== 1) {
        ret += 's';
      }
      parts.push(ret);
    }
    if (parts.length === 0) {
      return 'instantly';
    } else {
      return parts.join(' ');
    }
  }

  formatDuration(duration, useMilli = false) {
    let time = this.parseDuration(duration);
    return this.formatTime(time, useMilli);
  }

  percentDueDate(date) {
    var dateNow = Date.now();
    var start = new Date(date.start).getTime();
    var due = new Date(date.due).getTime();
    var finish = new Date(date.finish).getTime();
    var percentLeft = 100 - Math.round(((dateNow - start) * 100) / (due - start));
    if (date.due == null && date.finish == null) {
      return {
        percent: 0,
        text: '',
        formatString: () => '',
        status: 'active',
        color: this.getColor(0, false),
      };
    }
    if (date.finish != null && date.due == null) {
      return {
        percent: 100,
        text: ``,
        status: 'success',
        color: this.getColor(100, true),
      };
    }
    if (date.finish != null && date.due != null && finish > due) {
      return {
        percent: 100,
        text: `${this.formatDuration(finish - due)} late`,
        status: 'success',
        color: this.getColor(0, true),
      };
    }
    if (date.finish != null && finish < due) {
      return {
        percent: percentLeft,
        text: ``,
        status: 'success',
        color: this.getColor(percentLeft, true),
      };
    }
    if (date.finish == null && dateNow > due) {
      return {
        percent: 100,
        text: `> ${this.formatDuration(dateNow - due)}`,
        status: 'exception',
        color: this.getColor(0, false),
      };
    }
    return {
      percent: percentLeft,
      text: `${this.formatDuration(due - dateNow)} left`,
      formatString: () => ``,
      status: 'active',
      color: this.getColor(percentLeft, false),
    };
  }

  showModal(): void {
    this.isVisible = true;
  }
  showModal1(id): void {
    this.isVisible1 = true;
    this.idIssue = id;
    this.getIssue();
  }
  onChange(result: Date): void {
  }
  //get list Status
  getListStatus(){
    this.statusService.getList().subscribe(data => {
      this.listStatus = data;
    })
  }

  //get list cate
  getListCatByIdProject(){
    this.categoryService.getListCategoryByProject(this.projectLogin).subscribe(data =>{
      this.listCategory= data;
    });
  }
  handleOk(): void {
    this.isVisible = false;
  }
  handleOk1(): void {
    this.isVisible1 = false;
  }

  handleCancel(): void {
    this.isVisible = false;
  }
  handleCancel1(): void {
    this.isVisible1 = false;
  }
  log(value: string[]): void {
  }

filterChange(Filter){
  if(Filter==""){
    Filter=null;
  }
  this.Filter=Filter;
  this.searchData();
}
CategoryChange(result){
  this.idCategory=result;
  this.searchData();
}
GetStatusId(id : string,name){
  this.idStatus = id;
  this.filterStatus=name;
  this.searchData();
};
// search muti
setNullStatusId(){
  this.idStatus=null;
  this.filterStatus="All";
  this.searchData();
}
//username
getUserName(nameuser: any)
{
  this.userName = nameuser;
  this.searchData();
}
AssignMyself() {
  this.userName = this.shareService.getUserData.id;
  this.searchData();
}
//pagination nhưng ghi nhầm tên hàm lười sua
searchData(): void {
  this.loading = true;
  let skipCount = (this.pageIndex-1) * (this.pageSize);
  this.issueService
  .getListIssueResult(skipCount, this.pageSize, this.projectLogin, this.Filter, this.idCategory , this.idStatus, this.userName)
  .subscribe((data: any) => {
    this.loading = false;
    this.totalCount = data.totalCount;
    this.listIsssueByProject = data;
    var arr = new Array<object>();
    var arr1 = new Array<string>();
    data['items'].forEach(element => {

          var x = this.percentDueDate({
          start: element.startDate,
          due: element.dueDate,
          finish: element.finishDate,
    });
    var y = x.percent > 0 && x.percent <= 25 && x.status == 'active' ? '#1890ff' : '#1890ff'

    arr.push(x);
    arr1.push(y);
    });
    this.calculateProgress = arr;
    this.strokeColor = arr1;
  });
}
  //getDescription
  getIssue() {
    this.issueService.getIssueById(this.idIssue).subscribe(data => {
      this.Issue = data;
      this.dataIssue.Description = this.Issue.description;
    });
  }
  getListUserByIdProject() {
    this.userService.getListCreaterIDByIdProject(localStorage.getItem('ProjectId')).subscribe(data => {
      if (data) {
        this.listUser = data;
      }
    });
  }
  checkUserInProject(projectId){
    if(projectId){
      this.memberService.checkUserInProject(projectId).subscribe(data=>{
        if(!data){
          this.shareService.deleteLocalData();
          this.router.navigate(['/sign-in']);
        }
      });
    }
  }
  showFilterInMobile():void{
    this.isShowFilter = !this.isShowFilter;
  }
}
