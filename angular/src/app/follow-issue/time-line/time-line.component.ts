import { Component, OnInit, Input, Output, EventEmitter, AfterViewInit } from '@angular/core';
@Component({
  selector: 'time-line',
  templateUrl: './time-line.component.html',
  styleUrls: ['./time-line.component.scss'],
})
export class TimeLineComponent implements OnInit, AfterViewInit {
  isVisible = true;
  row: number;
  column: number;
  detailShow: boolean = false;
  detailData: any;
  @Input() listIssueFollow: any;
  @Output() click = new EventEmitter<boolean>();
  constructor() {}

  GetCharAt(name) {
    return name.charAt(0);
  }

  handleCancel(): void {
    this.isVisible = false;
    this.click.emit(this.isVisible);
  }

  showModal(data) {
    this.detailData = data;
    this.detailShow = true;
    // this.isVisible = false;
  }

  handleCancel_detail(): void {
    this.detailData = null;
    this.detailShow = false;
    // this.isVisible = true;
  }

  // //#endregion

  forLoop(number): any {
    let arr = [];
    for (let i = 1; i <= number; i++) {
      arr.push(i);
    }
    return arr;
  }

  handleData(data) {
    class Table {
      noColumn: number;
      column: any[];
      toPlainObj(): { noColumn; column } {
        return Object.assign({}, this);
      }
    }

    var cart = new Table();
    var listHour = [];

    data.map(element => {
      let date = new Date(element.excutitonTime).getHours();
      listHour.push(date);
      if (listHour.length == 1) {
        cart.noColumn = 1;
        var dataList = new Array(element);
        let newElementOfColumn = new Array({ index: date, id: element.issueID, data: dataList });
        cart.column = new Array();
        cart.column.push(newElementOfColumn);
      } else {
        for (let i = 0; i < cart.column.length; i++) {
          for (let j = 0; j < cart.column[i].length; j++) {
            if (cart.column[i][j].index == date && cart.column[i][j].id == element.issueID) {
              cart.column[i][j].data.push(element);
              return;
            }
            else if (cart.column[i].filter(e => e.index == date).length == 0) {
              var dataList = new Array(element);
              var newElementOfColumn = { index: date, id: element.issueID, data: dataList };
              cart.column[i].push(newElementOfColumn);
              return;
            }
          }
        }
        var dataList = new Array(element);
        var newElementOfColumn = { index: date, id: element.issueID, data: dataList };
        cart.column[cart.column.length] = new Array(newElementOfColumn);
        cart.noColumn++;
      }
    });
    return cart.toPlainObj();
  }

  get setColor(): {
    color: string;
    background: string;
  } {
    const rgb = [255, 0, 0];
    rgb[0] = Math.round(Math.random() * 255);
    rgb[1] = Math.round(Math.random() * 255);
    rgb[2] = Math.round(Math.random() * 255);
    // http://www.w3.org/TR/AERT#color-contrast
    const brightness = Math.round((rgb[0] * 299 + rgb[1] * 587 + rgb[2] * 114) / 1000);
    const textColour = brightness > 125 ? 'black' : 'white';
    const backgroundColour = 'rgb(' + rgb[0] + ',' + rgb[1] + ',' + rgb[2] + ')';
    return { color: textColour, background: backgroundColour };
  }

  sortByHour(data) {
    var rows = 0;
    var columns = 0;
    var sortHours = data
      .reduce((result, element) => {
        var time = new Date(element.excutitonTime);
        var key = time.getHours();
        (result[key] = result[key] || []).push(element);
        return result;
      }, [])
      .filter(e => e);
    var sortElementId = sortHours.map((e, i) => {
      rows = i > rows ? i : rows;
      var time = new Date(e[0].excutitonTime);
      var sort = e.reduce((result, element) => {
        (result[element.issueID] = result[element.issueID] || []).push(element);
        return result;
      }, []);
      var data = Object.values(sort).map((el, ind) => {
        columns = ind > columns ? ind : columns;
        return el;
      });
      var getColor = this.setColor;
      var result = {
        time: time.getHours(),
        data: data,
        bg: getColor.background,
        color: getColor.color,
      };
      return result;
    });
    rows++;
    columns++;
    return { rows: rows, columns: columns, data: sortElementId };
  }

  ngOnInit() {
    var listIssueId = [];
    var listBackgroundColor = [];
    var listTextColor = [];
    this.listIssueFollow.map(e => {
      if (listIssueId.indexOf(e.issueID) != null) {
        listIssueId.push(e.issueID);

        const rgb = [255, 0, 0];
        rgb[0] = Math.round(Math.random() * 255);
        rgb[1] = Math.round(Math.random() * 255);
        rgb[2] = Math.round(Math.random() * 255);
        // http://www.w3.org/TR/AERT#color-contrast
        const brightness = Math.round((rgb[0] * 299 + rgb[1] * 587 + rgb[2] * 114) / 1000);
        const textColour = brightness > 125 ? 'black' : 'white';
        const backgroundColour = 'rgb(' + rgb[0] + ',' + rgb[1] + ',' + rgb[2] + ')';
        listTextColor.push(textColour);
        listBackgroundColor.push(backgroundColour);
      }
      var date = new Date(e.excutitonTime);
      e.mmHH = `${date.getHours()}:${
        date.getMinutes() < 10 ? '0' + date.getMinutes() : date.getMinutes()
      }`;
      e.backgroundColor = listBackgroundColor[listIssueId.indexOf(e.issueID)];
      e.textColor = listTextColor[listIssueId.indexOf(e.issueID)];
      return e;
    });
    this.listIssueFollow = this.sortByHour(this.listIssueFollow);
  }
  ngAfterViewInit() {}
}
