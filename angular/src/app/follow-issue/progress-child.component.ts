import { Component, OnInit } from '@angular/core';

@Component({
  //<nz-progress [nzWidth]='50' [nzPercent]=calculateProgress.percent nzType="circle" [nzFormat]=formatString [nzStatus]=status></nz-progress>
  selector: 'progress-child',
  template: `<div class="progressChild"><span *ngIf="calculateProgress.text != ''"
  [style.color] = calculateProgress.color.textColor
  [style.fontWeight] = calculateProgress.color.fontWeight
  style="
    margin-bottom: -10px;
    display: flex;
    font-size: smaller;
  ">
  {{calculateProgress.text}}</span><nz-progress
    [nzPercent]=calculateProgress.percent
    [nzSteps]="5"
    [nzStrokeColor] = calculateProgress.color.strokeColor
    [nzFormat]=calculateProgress.formatString
    [nzStatus]=calculateProgress.status
  ></nz-progress></div>`,
})
export class ProgressChildComponent implements OnInit {
    calculateProgress: any;
    strokeColor: string;
//   percentReceived: any;
//   percent: number;
//   formatString: any;
//   status: string;
  // percentReceived = () => {
  //     if(this.percent == 'notSet') {
  //         this.percent = 'Not set';
  //         return 'Not set';
  //     }
  //     if(this.percent > 0) {
  //         return `${this.percent}%`;
  //     }
  //     if(this.exception == 'exception') {
  //         return 'Out date';
  //     }
  // };
  constructor() {}
  ngOnInit() {
    this.strokeColor = this.calculateProgress.percent > 0 && this.calculateProgress.percent <= 25 && this.calculateProgress.status == 'active' ? '#1890ff' : '#1890ff'
    // if (this.percentReceived == 'notSet') {
    //     this.percent = 100
    //   this.formatString = () => 'Not set';
    // }
    // if (this.percentReceived > 0) {
    //     this.percent = this.percentReceived;
    // }
    // if (this.percentReceived < 0) {
    //     this.percent = 0;
    //     this.formatString = () => 'Out of date';
    //     this.status = 'exception'
    // }
  }
}
