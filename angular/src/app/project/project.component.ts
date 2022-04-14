import { Component, OnInit } from '@angular/core';
import { ProjectService } from '../Service/Project/project.service';

@Component({
  selector: 'app-project',
  templateUrl: './project.component.html',
  styleUrls: ['./project.component.scss']
})
export class ProjectComponent implements OnInit {
  loading = false;
  listProject:any;
  currentQueryUrl;
  constructor(
    private projectService: ProjectService,
  ) { }

  ngOnInit(): void {
    this.getListProject();

  }
getListProject(){
  this.projectService.getListProject().subscribe(data=>{
    this.listProject=data;
  })
}
}
