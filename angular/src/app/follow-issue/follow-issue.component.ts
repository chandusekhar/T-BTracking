import {
  Component,
  OnInit,
  AfterViewInit,
  ViewChild,
  ViewContainerRef,
  ComponentFactoryResolver,
  Injector,
  ApplicationRef,
  EmbeddedViewRef,
} from '@angular/core';
import { Router } from '@angular/router';
import { FullCalendarComponent, CalendarOptions } from '@fullcalendar/angular';
import { Calendar } from '@fullcalendar/core';
import { fromEvent, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ShareServiceService } from '../Service/share-service.service';
import { ProgressChildComponent } from './progress-child.component';
import { StatusTagChildComponent } from './status-tag-child.component';
// import { UserService } from '../Service/User/user.service';
declare var $: any;
@Component({
  selector: 'app-follow-issue',
  templateUrl: './follow-issue.component.html',
  styleUrls: ['./follow-issue.component.scss'],
})
export class FollowIssueComponent implements OnInit, AfterViewInit {
  //Date on date picker

  @ViewChild('calendar') calendarComponent: FullCalendarComponent;
  datePicked = null;
  calendarApi: Calendar;
  calendarOptions: CalendarOptions = {
    themeSystem: 'bootstrap',
    // validRange: {
    //   start: '2017-05-01',
    //   end: '2017-06-01'
    // }
    eventsSet: function (dateInfo) {
      var renderedEvents = $('.fc-list-table  tr');
      var reorderedEvents = [];
      var blockEvents = null;
      renderedEvents.map(function (key, event) {
        if ($(event).hasClass('fc-list-day')) {
          if (blockEvents) {
            reorderedEvents.unshift(blockEvents.children());
          }
          blockEvents = $('<tbody></tbody>');
        }
        blockEvents.append(event);
      });
      if (blockEvents) {
        reorderedEvents.unshift(blockEvents.children());
        $('.fc-list-table tbody').html(reorderedEvents);
      }
    },
    headerToolbar: {
      // left: 'dayGridMonth,timeGridWeek,timeGridDay,today',\
      left: '',
      center: '',
      right: 'title',
    },
    eventOrder: '-start',
    eventOrderStrict: true,
    height: '100%',
    initialView: 'listYear',
    eventContent: function (info) {
      if (info.view.type == 'dayGridMonth') {
        return {
          html: `${info.event.title}`,
        };
      }
      if (info.view.type == 'listMonth') {
        return {
          html: `${info.event.title}<div>${info.event.extendedProps.data}</div>`,
        };
      }
      if (info.view.type == 'listYear') {
        //${info.event.title}
        // <div style="font-size: 12px;box-sizing: border-box;margin-right: 20px;float: right;border: 1px solid #cecece;padding: 5px;border-radius: 5px;background: #f5f5f5;">
        //       <div>Recently update by UserName</div>
        //       <div>Status {{xxx => yyy}}</div>
        //     </div>
        return {
          html: `
            <div class="myEventContent"></div>
            `,
        };
      }
    },
    eventClick: this.onClickIssue.bind(this),
  };

  index: number = 0;
  pageSize: number = 20;

  projectList: any;
  projectSelect: any;
  filter = {
    name: 'default',
    value: 'default'
  };
  constructor(
    private componentFactoryResolver: ComponentFactoryResolver,
    private injector: Injector,
    private appRef: ApplicationRef,
    public viewContainerRef: ViewContainerRef,
    public router: Router,
    public shareService: ShareServiceService
  ) {}
  ngOnInit(): void {}
  changeProjectSelect() {
    this.filter = {
      name: "project",
      value: this.projectSelect
    }
    this.index = 0;
    this.pageSize = 20;
    this.calendarApi.removeAllEvents();
    this.addData()
  }
  addData() {
    this.api_getListIssue().subscribe(
      res => {
        if (res.length > 0) {
          this.AddEventsFromSource(res);

          document.querySelector(".fc-header-toolbar").remove();

          var listEvent = this.calendarApi.getEvents();
          var getEventsContent = document.querySelectorAll<HTMLElement>('.myEventContent');

          for (var index = 0; index < getEventsContent.length; index++) {
            //Due date percent
            var calculateProgress = this.percentDueDate({
              start: listEvent[index].start,
              due: listEvent[index].extendedProps.issueProperties.dueDate,
              finish: listEvent[index].extendedProps.issueProperties.finishDate,
            });
            //Time
            var componentFactory =
              this.componentFactoryResolver.resolveComponentFactory(ProgressChildComponent); // Your dynamic component will replace DynamicComponent
            var componentRef = componentFactory.create(this.injector);
            componentRef.instance['calculateProgress'] = calculateProgress;
            this.appRef.attachView(componentRef.hostView);
            var domElem = (componentRef.hostView as EmbeddedViewRef<any>)
              .rootNodes[0] as HTMLElement;
            //Status
            var componentFactory_Status =
              this.componentFactoryResolver.resolveComponentFactory(StatusTagChildComponent); // Your dynamic component will replace DynamicComponent
            var componentRef_Status = componentFactory_Status.create(this.injector);
            componentRef_Status.instance['color'] = listEvent[index].extendedProps.issueProperties.statusColor;
            componentRef_Status.instance['name'] = listEvent[index].extendedProps.issueProperties.statusName;
            this.appRef.attachView(componentRef_Status.hostView);
            var _domElem = (componentRef_Status.hostView as EmbeddedViewRef<any>)
              .rootNodes[0] as HTMLElement;

            const issueName = new DOMParser().parseFromString(
              `<div class="issueName">Name: ${listEvent[index].title}<div class="projectName">${listEvent[index].extendedProps.issueProperties.projectName}</div></div>`,
              'text/html'
            ).body.childNodes[0];

            getEventsContent[index].appendChild(issueName);
            getEventsContent[index].appendChild(_domElem);
            //getEventsContent[index].appendChild(lastModification);
            getEventsContent[index].appendChild(domElem);

            //Styles
            //getEventsContent[index].style.display = "flex";
          }
        }
      },
      error => {}
    );
    this.index++;
  }

  addEvent() {
    this.addData();
  }

  ngAfterViewInit() {
    //Get more data at the end of scroll
    const content = document.querySelector('.fc-scroller');
    const scroll$ = fromEvent(content, 'scroll').pipe(map(() => content));
    scroll$.subscribe(element => {
      if (element.scrollTop + element.clientHeight >= element.scrollHeight) {
        //console.log('reached');
        this.addData();
      }
    });

    //After view init, initialize the api and manipulate with it
    this.calendarApi = this.calendarComponent.getApi();
    this.addData();
  }

  /////////////API services/////////////
  //Get: List issue based month
  api_getListIssue(): Observable<any> {
    //var idProject = this.router.url.split('/')[2]; //get Project
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/issue-follow?index=${this.index}&pageSize=${this.pageSize}&filterName=${this.filter.name}&filterValue=${this.filter.value}`;
    return this.shareService.returnHttpClient(url);
  }
  api_getUpdatesOfIssue(id): Observable<any> {
    const url = `${this.shareService.REST_API_SERVER}/api/app/follow/issue-following-detail/${id}`;
    return this.shareService.returnHttpClient(url);
  }

  //Parse a month to yyyy-DD
  parseMonth(date) {
    var month = date.getMonth() + 1 < 10 ? '0' + (date.getMonth() + 1) : date.getMonth() + 1;
    var year = date.getFullYear();
    return `${year}-${month}`;
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
        percent: 100,
        text: 'Not Set',
        formatString: () => '',
        status: 'active',
        color: this.getColor(100, false),
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
        percent: 0,
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
        percent: 0,
        text: '',
        status: 'exception',
        color: this.getColor(0, false),
      };
    }

    //console.log('percentLeft', percentLeft, dateNow, start, due);
    return {
      percent: percentLeft,
      text: `${this.formatDuration(due - dateNow)} left`,
      formatString: () => ``,
      status: 'active',
      color: this.getColor(percentLeft, false),
    };
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

  //Get issues receive from source t and goto date
  AddEventsFromSource(source) {
    source.map(e => {
      var data = {
        title: e.name,
        //start: e.issueProperties.lastModificationTime ?? e.issueProperties.startDate,
        start: e.creationTime,
        //backgroundColor: e.backgroundColor,
        extendedProps: {
          issueProperties: {
            statusName: e.statusName,
            statusColor: e.statusColor,
            projectName: e.projectName,
            projectId: e.projectID,
            dueDate: e.dueDate,
            finishDate: e.finishDate
          },
          // lastUpdate: e.lastUpdate,
          //backgroundColor: e.backgroundColor,
          //status: e.issueProperties.statusName,
          //timeLeft: e.timeLeft?.ticks
        },
      };
      this.calendarApi.addEvent(data);
      //Delete table td
      document.querySelectorAll<HTMLElement>('.fc-list-event-time').forEach(e => {
        e.remove();
      });
      document.querySelectorAll<HTMLElement>('.fc-list-event-graphic').forEach(e => {
        e.remove();
      });
    });
  }

  //Change view on date picked
  datePicker_onChange(result: Date): void {
    this.calendarApi.removeAllEvents();
    this.api_getListIssue().subscribe(
      res => {
        this.calendarApi.changeView('dayGridMonth');
        this.calendarApi.gotoDate(result);
        this.AddEventsFromSource(res);
      },
      error => {}
    );
  }
  //Quick to month now
  todayClick() {
    this.calendarApi.removeAllEvents();
    this.api_getListIssue().subscribe(
      res => {
        this.calendarApi.changeView('dayGridMonth');
        this.calendarApi.today();
        this.AddEventsFromSource(res);
      },
      error => {}
    );
  }
  //Go to list day view of this event
  onClickIssue(info) {
    this.calendarApi.changeView('listMonth');
    this.calendarApi.removeAllEvents();
    this.api_getUpdatesOfIssue(info.event.extendedProps.issueId).subscribe(
      res => {
        res.map(e => {
          var data = {
            title: e.action,
            start: e.time,
            extendedProps: {
              data: e.data.reduce((total, current) => {
                return (total += `<div>${current.propertyName}: ${current.originalValue} => ${current.newValue}</div>`);
              }, ''),
            },
          };
          this.calendarApi.addEvent(data);
        });

        var test = document.querySelectorAll<HTMLElement>('.fc-list-event-dot');

        const componentFactory =
          this.componentFactoryResolver.resolveComponentFactory(ProgressChildComponent); // Your dynamic component will replace DynamicComponent

        const componentRef = componentFactory.create(this.injector);

        this.appRef.attachView(componentRef.hostView);

        const domElem = (componentRef.hostView as EmbeddedViewRef<any>).rootNodes[0] as HTMLElement;

        // const element: HTMLElement = document.createElement('div');
        // element.style.width = '100px'
        // element.appendChild(domElem); // Component needs to be added here
        test[0].replaceWith(domElem);

        // for ( var index = 0; index < test.length; index++) {
        //   var div = document.createElement('div');
        //   const toNodes = new DOMParser().parseFromString('<div progress-child></div>', 'text/html').body.childNodes[0]
        //   //div.innerHTML = "OK"
        //   test[index].replaceWith(toNodes)
        //   test[index].style.marginTop = '10px'
        // }
      },
      error => {}
    );
  }
}
