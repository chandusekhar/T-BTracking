import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ShareServiceService } from '../Service/share-service.service';
import { UserService } from '../Service/User/user.service';

@Component({
  selector: 'app-department-manage',
  templateUrl: './department-manage.component.html',
  styleUrls: ['./department-manage.component.scss']
})
export class DepartmentManageComponent implements OnInit {
  currentRouter;
  dashboard = false;
  admin = false;
  workflow = false;
  department = false;
  team = false;
  issuedepartment = false;
  myissue = false;
  myFollow = false;
  isActive;
  currentPath: string;
  url;
  isCollapsed = false;
  NameModule: any = "dashboard";
  constructor(
    public router: Router,
    // private rout: ActivatedRoute,
    private userService: UserService,
    public shareService: ShareServiceService,
  ) { }

  ngOnInit(): void {
    this.currentRouter = this.router.url;
    if (this.currentRouter.includes('admin')) this.admin = true;
    if (this.currentRouter.includes('dashboard') || this.currentRouter == '/' || this.currentRouter == '/departmentManage') this.dashboard = true;
    if (this.currentRouter.includes('workflow')) this.workflow = true;
    if (this.currentRouter.includes('departmentManage/department') || this.currentRouter == '/department') this.department = true;
    if (this.currentRouter.includes('issue-department')) this.issuedepartment = true;
    if (this.currentRouter.includes('my-issue')) this.myissue = true;
    if (this.currentRouter.includes('myFollow')) this.myFollow = true;
    if (this.currentRouter.includes('team')) this.team = true;
    // if(this.url == '/departmentManage/dashboard') this.isActive='dashboard';
  }
  // choose(active){
  //   this.isActive= active;
  // }
  checkUserAmin() {
    this.userService.CheckAdmin().subscribe(data => {
      this.shareService.admin = data;
      if (this.shareService.admin) {
        this.NameModule = "department";
      }
    });
  }
  CheckRouter(NameModule) {
    this.isActive = NameModule;
    //if(NameModule == 'admin')
    // this.router.navigate(['admin' ]);
  }
}
