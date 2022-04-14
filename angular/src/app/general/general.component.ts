import { MemberService } from './../Service/Member/member.service';
import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { Router } from '@angular/router';
import * as Chart from 'chart.js';
import { ProjectService } from '../Service/Project/project.service';
import { ShareServiceService } from '../Service/share-service.service';
import { StatusService } from '../Service/Status/status.service';

@Component({
  selector: 'app-general',
  templateUrl: './general.component.html',
  styleUrls: ['./general.component.scss'],
})
export class GeneralComponent implements OnInit, AfterViewInit {
  @ViewChild('lineCanvas') lineCanvas: ElementRef;
  @ViewChild('doughnut') doughnut: ElementRef;
  @ViewChild('monthChart') monthChart: ElementRef;
  @ViewChild('yearChart') yearChart: ElementRef;
  @Output() projectNameChild: EventEmitter<string> = new EventEmitter();
  @Output() projectNameChildIn: string;
  @ViewChild('radar') radar: ElementRef;
  monthchart: any;
  yearchart: any;
  lineChart: any;
  doughnutChart: any;
  stepDay: number;
  stepMonth: number;
  stepYear: number;
  maxDay: number;
  maxMonth: number;
  maxYear: number;
  Radar;
  totalCreateDate = 0;
  totalFinishDate = 0;
  totalCreateMonth = 0;
  totalFinishMonth = 0;
  totalCreateYear = 0;
  totalFinishYear = 0;
  totalCreateD = [];
  totalFinishD = [];
  totalCreateM = [];
  totalFinishM = [];
  totalCreateY = [];
  totalFinishY = [];
  projectName;
  project;
  listProject;
  userLogin;
  countItem: any;
  countIssue = [];
  countAssignee = 0;
  percenNotClosed = 0;
  percentAdd = 0;
  notClosed = 0;
  countStatus = [];
  projectColor = '#363636';
  projectColorChange;
  lable = [];
  create = [];
  finish = [];
  countColor = [];
  ColorName = [];
  lableDate = [];
  creatDate = [];
  finishDate = [];
  lableYear = [];
  countCreatYear = [];
  coutFinishYear = [];
  countIssueProject = 0;
  categoryData;
  memberStatistics;

  constructor(
    public router: Router,
    private projectService: ProjectService,
    public shareService: ShareServiceService,
    private statusService: StatusService,
    private memberService: MemberService
  ) {}

  ngOnInit(): void {
    this.getProject();
    this.getItemProject();
    this.userLogin = this.shareService.getUserData.id;
    this.getIssueByDay();
    this.getListStatusByIdProject();
    this.getCategoryStatistic();
  }
  ngAfterViewInit(): void {
    this.lineChartMethod();
    this.chartMont();
    this.chartYear();
    this.getMemberStatistics();
  }

  getMemberStatistics(){
    this.memberService.getMemberStatistics(localStorage.getItem('ProjectId')).subscribe(data=>{
      this.memberStatistics = data;
    })
  }
  getProject() {
    this.projectService.getProjectByID(localStorage.getItem('ProjectId')).subscribe(data => {
      this.projectName = data.name;
      this.project = data;
      this.projectColor = data.nzColor;
      this.projectColorChange = data.nzColor;
    });
  }
  getItemProject() {
    this.projectService.getItemProject(localStorage.getItem('ProjectId')).subscribe(data => {
      this.countIssueProject = data.issueCount;
      this.countAssignee = data.assigneeCount;
      this.percenNotClosed = data.percentIssue;
      this.percentAdd = data.percentAdd;
      this.notClosed = data.notClosed;
    });
  }
  getChartAt(name){
    var rs = '';
    name.split(' ').forEach(char => rs += char.charAt(0))
    return rs;
  }

  //
  getIssueByDay() {
    this.projectService.getIssueBydate(localStorage.getItem('ProjectId')).subscribe(data => {
      data.forEach(t => {
        this.lableYear.push(t.year);
        this.countCreatYear.push(t.countCreate);
        this.coutFinishYear.push(t.countFinish);
        this.totalCreateYear += t.countCreate;
        this.totalCreateY.push(this.totalCreateYear);
        this.totalFinishYear += t.countFinish;
        this.totalFinishY.push(this.totalFinishYear);
        t.data.forEach(element => {
          this.lable.push(element.month);
          this.create.push(element.countCreate);
          this.finish.push(element.countFinish);
          this.totalCreateMonth += element.countCreate;
          this.totalCreateM.push(this.totalCreateMonth);
          this.totalFinishMonth += element.countFinish;
          this.totalFinishM.push(this.totalFinishMonth);
          element.data.forEach(date => {
            this.lableDate.push(date.date);
            this.creatDate.push(date.countCreate);
            this.finishDate.push(date.countFinish);
            this.totalCreateDate += date.countCreate;
            this.totalCreateD.push(this.totalCreateDate);
            this.totalFinishDate += date.countFinish;
            this.totalFinishD.push(this.totalFinishDate);
          });
        });
      });
      if (
        Math.max.apply(Math, this.totalCreateY) < 10 &&
        Math.max.apply(Math, this.totalFinishY) < 10
      ) {
        this.stepYear = 1;
        this.maxYear = 10;
      } else {
        if (Math.max.apply(Math, this.totalCreateY) > Math.max.apply(Math, this.totalFinishY)) {
          this.maxYear = Math.max.apply(Math, this.totalCreateY);
          this.stepYear = Math.round(this.maxYear / 10);
        } else {
          this.maxYear = Math.max.apply(Math, this.totalFinishY);
          this.stepYear = Math.round(this.maxYear / 10);
        }
      }
      if (Math.max.apply(Math, this.totalCreateM) < 10 && Math.max.apply(Math, this.totalFinishM) < 10) {
        this.stepMonth = 1;
        this.maxMonth = 10;
      } else {
        if (Math.max.apply(Math, this.totalCreateM) > Math.max.apply(Math, this.totalFinishM)) {
          this.maxMonth = Math.max.apply(Math, this.totalCreateM);
          this.stepMonth = Math.round(this.maxMonth / 10);
        } else {
          this.maxMonth = Math.max.apply(Math, this.totalFinishM);
          this.stepMonth = Math.round(this.maxMonth / 10);
        }
      }
      if (Math.max.apply(Math, this.totalCreateD) < 10 && Math.max.apply(Math, this.totalFinishD) < 10) {
        this.stepDay = 1;
        this.maxDay = 10;
      } else {
        if (Math.max.apply(Math, this.totalCreateD) > Math.max.apply(Math, this.totalFinishD)) {
          this.maxMonth = Math.max.apply(Math, this.totalCreateD);
          this.stepMonth = Math.round(this.maxMonth / 10);
        } else {
          this.maxMonth = Math.max.apply(Math, this.totalFinishD);
          this.stepMonth = Math.round(this.maxMonth / 10);
        }
      }
      this.lineChart.destroy();
      this.monthchart.destroy();
      this.yearchart.destroy();
      this.lineChartMethod();
      this.chartMont();
      this.chartYear();
    });
  }
  getListStatusByIdProject() {
    this.statusService
      .getListStatusByIdProject(localStorage.getItem('ProjectId'), 999)
      .subscribe(data => {
        data.forEach(element => {
          this.countStatus.push(element.countIssue);
          this.countColor.push(element.nzColor);
          this.ColorName.push(element.name);
        });
        this.lineChartStatusMethod();
      });
  }

  //

  lineChartStatusMethod() {
    this.doughnutChart = new Chart(this.doughnut.nativeElement, {
      type: 'doughnut',

      data: {
        labels: this.ColorName,
        datasets: [
          {
            data: this.countStatus,
            backgroundColor: this.countColor,
          },
        ],
      },
      options: {
        title: {
          display: true,
          text: 'Tasks by status',
        },
        animation: {
          animateScale: true,
          animateRotate: true,
        },
      },
    });
  }
  getCategoryStatistic() {
    this.projectService
      .getCategoryStatistic(localStorage.getItem('ProjectId'))
      .subscribe(data => {
        this.categoryData = {
          labels: data.categories,
          datasets: [
            {
              label: 'Bugs Count',
              data: data.issues,
              backgroundColor: 'rgb(255, 85, 0,0.8)'
            }
          ],
        };
        this.Radar = new Chart(this.radar.nativeElement, {
          type: 'bar',
          data: this.categoryData,
          options: {
            title: {
              display: true,
              text: 'Tasks by category',
            },
            scales: {
              yAxes: [{
              ticks: {
                fontColor: "#091F3e",
               // steps: 10,
                stepSize: 1,
                beginAtZero: true
                }
              }],
            }
            }
        });
      });
  }
  lineChartMethod() {
    this.lineChart = new Chart(this.lineCanvas.nativeElement, {
      type: 'line',
      data: {
        labels: this.lableDate,
        datasets: [
          {
            label: 'Issue Finished',
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
            pointHoverRadius: 1,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.finishDate,
            spanGaps: false,
          },
          {
            label: 'Total Issue Finished',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(8, 223, 44, 0.8)',
            borderColor: 'rgba(8, 223, 44, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(8, 223, 44, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(8, 223, 44, 0.8)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalFinishD,
            spanGaps: false,
          },
          {
            label: 'Issue Added',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'red',
            borderColor: 'red',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'red',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'red',
            pointHoverBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.creatDate,
            spanGaps: false,
          },
          {
            label: 'Total Issue Added',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(255, 85, 0, 0.8)',
            borderColor: 'rgba(255, 85, 0, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderColor: 'rgb(75, 192, 110)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalCreateD,
            spanGaps: false,
          },
        ],
      },
      options: {
        title: {
          display: true,
          text: 'Tasks by day',
        },
        animation: {
          animateScale: false,
          animateRotate: false,
        },
        scales: {
          yAxes: [
            {
              ticks: {
                fontColor: '#091F3e',
                beginAtZero: true,
                // steps: 10,
                stepSize: this.stepDay,
                max: this.maxDay,
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
                //  fontSize: "10",
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

  chartMont() {
    this.monthchart = new Chart(this.monthChart.nativeElement, {
      type: 'line',
      data: {
        labels: this.lable,
        datasets: [
          {
            label: 'Issue Finished',
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
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.finish,
            spanGaps: false,
          },
          {
            label: 'Total Issue Finished',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(8, 223, 44, 0.8)',
            borderColor: 'rgba(8, 223, 44, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(8, 223, 44, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(8, 223, 44, 0.8)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalFinishM,
            spanGaps: false,
          },
          {
            label: 'Issue Added',
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
            pointHoverBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.create,
            spanGaps: false,
          },
          {
            label: 'Total Issue Added',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(255, 85, 0, 0.8)',
            borderColor: 'rgba(255, 85, 0, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderColor: 'rgb(75, 192, 110)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalCreateM,
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
                // steps: 10,

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
                //  fontSize: "10",
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
            label: 'Issue Finished',
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
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(75,192,192,1)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.coutFinishYear,
            spanGaps: false,
          },
          {
            label: 'Total Issue Finished',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(8, 223, 44, 0.8)',
            borderColor: 'rgba(8, 223, 44, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(8, 223, 44, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(8, 223, 44, 0.8)',
            pointHoverBorderColor: 'rgba(220,220,220,1)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalFinishY,
            spanGaps: false,
          },
          {
            label: 'Issue Added',
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
            pointHoverBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.countCreatYear,
            spanGaps: false,
          },
          {
            label: 'Total Issue Added',
            fill: false,
            lineTension: 0.05,
            backgroundColor: 'rgba(255, 85, 0, 0.8)',
            borderColor: 'rgba(255, 85, 0, 0.8)',
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: 'rgba(255, 85, 0, 0.8)',
            pointBackgroundColor: '#fff',
            pointBorderWidth: 5,
            pointHoverRadius: 5,
            pointHoverBackgroundColor: 'rgba(255, 85, 0, 0.8)',
            pointHoverBorderColor: 'rgb(75, 192, 110)',
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.totalCreateY,
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
                // steps: 10,

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
                //  fontSize: "10",
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
