import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ProjectService } from 'src/app/Service/Project/project.service';

@Component({
  selector: 'app-timeOnProject',
  templateUrl: './timeOnProject.component.html',
  template: `{{ now | date:'HH:mm:ss'}}`,
  styleUrls: ['./timeOnProject.component.scss'],
})
export class TimeOnProjectComponent implements OnInit {
  values = [1, 2, 3, 4];
  isVisibleDetail = false;
  projectLogin;
  isMonth:boolean =false;
  dueDate: any = new Date();
  buttonText = 'This Month';
  idTimeOn;
  listTimeOn;
  constructor(
    private projectService: ProjectService,
    private rout: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.projectLogin = this.rout.snapshot.params.idProject;
    this.onChangeDueDate(this.dueDate);
  }
  showModalDetail(id): void {
    this.isVisibleDetail = true;
    this.idTimeOn = id;
  }
  getListTimeOnProject(){
    this.projectService.getTimeOnProject(this.projectLogin,this.dueDate,this.isMonth).subscribe(data=>{
      this.listTimeOn = data;
    })
  }
  handleCancel(): void {
    this.isVisibleDetail = false;
  }
  changTypeView() {
    this.buttonText == 'This Month'
      ? (this.buttonText = 'Today')
      : (this.buttonText = 'This Month');
      this.isMonth = !this.isMonth;
      this.getListTimeOnProject();
  }
  onChangeDueDate(result: Date): void {
    var newDate;
    var newMonth;
    if (result == null) {
      this.dueDate = null;
    } else {
      var resultDate = result.getDate();
      if (resultDate < 10) {
        newDate = '0' + resultDate.toString();
      } else newDate = resultDate.toString();
      var resultMonth = result.getMonth() + 1;
      if (resultMonth < 10) {
        newMonth = '0' + resultMonth.toString();
      } else newMonth = resultMonth.toString();
      this.dueDate = result.getFullYear() + '-' + newMonth + '-' + newDate;
    }
    this.getListTimeOnProject();
  }
  GetCharAt(name) {
    return name
      .split(' ')
      .map(char => char.charAt(0))
      .join('');
  }
  getNumber(number){
    number = number.toFixed(2);
    return number;
  }
}
