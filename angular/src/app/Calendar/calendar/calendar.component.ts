import { AfterViewChecked, Component, forwardRef, OnInit, ViewChild } from '@angular/core';
import { FullCalendarComponent } from '@fullcalendar/angular';
import { Calendar, CalendarOptions, EventInput } from '@fullcalendar/core';
import dayGridPlugin from '@fullcalendar/daygrid';
import interactionPlugin from '@fullcalendar/interaction';
import { CalendarService } from 'src/app/Service/Calendar/calendar.service';
import { NzDrawerPlacement } from 'ng-zorro-antd/drawer';
import { ProjectService } from 'src/app/Service/Project/project.service';
import { ShareServiceService } from 'src/app/Service/share-service.service';
import { CreateMessageService } from 'src/app/Service/Message/create-message.service';
import { SignalRService } from 'src/app/Service/SignalR/signal-r.service';
@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit,AfterViewChecked {
  @ViewChild('fullcalendar') fullcalendar: FullCalendarComponent;
  @ViewChild('fullcalendarMobile') fullcalendarMobile: FullCalendarComponent;
  gridStyle = {
    width: '25%',
    textAlign: 'center'
  };
  calendarEvents: EventInput[] = [];
  calendarOptions: CalendarOptions;
  eventsModel: any;
  calendarApi: Calendar;
  calendarApiMobile: Calendar;
  initialized = false;
  options: any;
  visible = false;
  issueName = '';
  idProject = '';
  idStatus ='';
  listProject: any;
  listIdproject:any
  listIssue:any
  listStatus:any
  listTeam : any
  idTeam =''
  isVisibleAddIssue: boolean = false;
  isVisible = false;
  isOkLoading = false;
  idProjectSelect =''
  isShowFilter = false
  isVisibleCalendarModal = false
  issueDetail : any
  calculateProgress: any;
  strokeColor: any;
  array: any;
  connectionHub: any;
  isVisibleFilter=false;
  isVisibleProfile=false;
  idProfileShowing;
  action ='follow';
  constructor(
    private calendarService:CalendarService,
    private projectService:ProjectService,
    public shareService: ShareServiceService,
    private messageService : CreateMessageService,
    private signalRService: SignalRService,
  ) { }
  ngAfterViewChecked() {
    this.calendarApi = this.fullcalendar.getApi();

    if (this.calendarApi && !this.initialized) {
      this.initialized = true;
      this.getCalendarFollow();
    }
    this.calendarApiMobile = this.fullcalendarMobile.getApi();

    if (this.calendarApiMobile && !this.initialized) {
      this.initialized = true;
      this.getCalendarFollow();
    }
  }
  ngOnInit() {
    this.getListProject()
    this.getListIssueFollow()
    this.getListStatus()
    this.getListTeam()
    this.signalRService.SetConnection();
    this.connectionHub = this.signalRService.connection;
  forwardRef(() => Calendar);
    this.calendarOptions = {
      weekNumbers : true,
      nowIndicator : true,
      handleWindowResize: true,
      windowResizeDelay:100,
      plugins: [dayGridPlugin, interactionPlugin],
      editable: true,
      themeSystem: 'bootstrap',

      customButtons: {
        myCustomButton: {
          text: 'New Bug/Task',
          click: () => this.showModal()
        },buttonFilter: {
          text: 'Show Filter',
          click: () => this.showModalFilter()
        },Assignee: {
          text: 'Assigned To Me',
          click: () => this.assigneeModalFilter()
        },Creater: {
          text: 'Created By Me',
          click: () => this.createrModalFilter()
        },Follow: {
          text: 'Follows By Me',
          click: () => this.followModalFilter()
        }
        

      },
      headerToolbar: {
        left: 'myCustomButton buttonFilter Assignee Creater Follow prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay,listYear'
      },
      bootstrapFontAwesome: { redo: 'redo' },
      dateClick: this.handleDateClick.bind(this),
      eventClick:
      this.handleEventClick.bind(this),
      eventDragStop: function(event) {
      }
    };
    this.shareService.reloadRouter();
    this.calendarUpdate();
  }
  calendarUpdate() {
    this.connectionHub.on('calendar', (userId) => {
      if (this.shareService.getUserData.id == userId) {
        console.log(1);
        this.getCalendarFollow();
        setTimeout(() => {
          this.isVisibleCalendarModal=false;
        }, 1000);
      }
    });
  }
  getCalendarFollow(){
      this.calendarService.getCalendarFollow(this.issueName,this.idProject, this.idStatus, this.action).subscribe(data=>{
        this.calendarApi.removeAllEventSources();
        this.calendarApi.addEventSource(data);
        this.calendarApiMobile.removeAllEventSources();
        this.calendarApiMobile.addEventSource(data)
      })
  }
  handleDateClick(arg) {
  }
  handleEventClick(arg) {
    this.calendarService.getIssueDetail(arg.event._def.extendedProps.issueId).subscribe((res:any)=>{
      this.issueDetail = res
      this.calendarService.IssueIdCalendar = res.id;
      this.calendarService.ProjectIdCalendar = res.projectID
      this.isVisibleCalendarModal = true;
      var arr = new Array<object>();
      var arr1 = new Array<string>();
      var x = this.percentDueDate({
            start: res?.startDate,
            due: res?.dueDate,
            finish: res?.finishDate,
      });
      var y = x.percent > 0 && x.percent <= 25 && x.status == 'active' ? '#1890ff' : '#1890ff'
      arr.push(x);
      arr1.push(y);
      this.array = arr;
      this.strokeColor = arr1;
      this.calculateProgress=this.array[0];
    })
  }

  handleEventDragStop(arg) {
    // console.log(arg)
  }

  updateHeader() {
    this.calendarOptions.headerToolbar = {
      left: 'prev,next myCustomButton',
      center: 'title',
      right: ''
    };
  }
  placement: NzDrawerPlacement = 'left';
  open(): void {
    this.isVisibleAddIssue = true;
  }

  close(): void {
    this.visible = false;
  }
  GetIdProject(event){
    this.idProject = event
    this.getListIssueFollow()
    this.getCalendarFollow()
  }
  GetIssueName(event){
    this.issueName = event
    this.getCalendarFollow()
  }
  getListIssueFollow(){
    this.calendarService.getIssueFollow(this.idProject).subscribe(data=>
      {
        this.listIssue = data
      }
      )
  }
  getListStatus(){
    this.calendarService.getStatus().subscribe(data=>
      {
        this.listStatus = data
      })
  }
  getIdStatus(event){
    this.idStatus = event
    this.getCalendarFollow()
  }
  getListTeam()
  {
    this.calendarService.getTeamByUserId().subscribe(data=>{
      this.listTeam = data
    })
  }
  getIdTeam(event)
  {
    this.idTeam = event
    this.getCalendarFollow()
  }
  ///getListProject
  getListProject(){
    this.projectService.getListByUserId().subscribe(data=>
      {
        this.listProject = data.items
      })
  }
  handleCancelAddIssue(): void {
    this.isVisibleAddIssue = false;
  }
  ///////////////chose project
  showModal(): void {
    this.isVisible = true;
  }
  getIdProject(event){
    this.idProjectSelect = event
  }
  handleOk(): void {
    if(this.idProjectSelect != '')
    {
      this.isOkLoading = true;
      setTimeout(() => {
        this.isVisible = false;
        this.isOkLoading = false;
         
      }, 200);
    }
    else
    {
      this.messageService.createMessage('error', 'Please check a Project');
    }
  }

  handleCancel(): void {
    this.isVisible = false;
  }
  showFilterInMobile():void{
    this.isShowFilter = !this.isShowFilter;
  }
  /////calendar
  handleCancelCalendatModal(): void {
    this.isVisibleCalendarModal = false
  }
  handleOkCalendarModal(){
    this.isVisibleCalendarModal = false;
  }
  ////////////////////////////////get due time
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
  showModalFilter(): void {
    this.isVisibleFilter = true;
  }

  handleOkFilter(): void {
    this.isVisibleFilter = false;
  }

  handleCancelFilter(): void {
    this.isVisibleFilter = false;
  }

  ///profile
  handleCancelProfile(){
    this.isVisibleProfile=false;
  }
  showUserProfile(id){
    this.idProfileShowing = id;
    this.isVisibleProfile =true;
  }
  CreateConversition() {
    this.messageService.CheckConversation(this.idProfileShowing).subscribe(data => {
      if (data['hasConversation']) {
        this.signalRService.connection.invoke(
          'DrawerChat',
          data['conversationId'],
          this.shareService.getUserData.id,
        );
      }
      else {
        this.messageService.CreateConversition(this.idProfileShowing).subscribe(data => {
          this.signalRService.connection.invoke(
            'DrawerChat',
            data.conversationId,
            this.shareService.getUserData.id,
          );
        });
      }
      this.isVisibleProfile=false
    });
  }
  assigneeModalFilter() {
    this.action = 'assignee'
    this.getCalendarFollow()
  }
  createrModalFilter() {
    this.action = 'creater'
    this.getCalendarFollow()
  }
  followModalFilter() {
    this.action = 'follow'
    this.getCalendarFollow()
  }
}
