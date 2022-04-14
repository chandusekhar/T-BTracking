//leaves-routing.module.ts
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminComponent } from '../admin/admin.component';
import { CalendarComponent } from '../Calendar/calendar/calendar.component';
import { DashboardComponent } from '../dashboard/dashboard.component';
import { DepartmentComponent } from '../department/department.component';
import { ElsaComponent } from '../elsa/elsa.component';
import { IssueDepartmentComponent } from '../issue-department/issue-department.component';
import { MyIssuesComponent } from '../MyIssues/my-issues/my-issues.component';
import { AuthGuard } from '../Service/Auth/auth.guard';
import { TeamComponent } from '../team/team.component';
import { DepartmentManageComponent } from './department-manage.component';

const routes: Routes = [
    { path: '', component:DepartmentManageComponent,canActivate:[AuthGuard] ,
    children: [
        { path:'',component:DashboardComponent,},
      { path:'admin',component:AdminComponent,},
      { path:'dashboard',component:DashboardComponent,},
      { path:'workflow',component:ElsaComponent,},
      { path:'department',component:DepartmentComponent,},
      { path:'team',component:TeamComponent,},
      { path:'issue-department',component:IssueDepartmentComponent,},
      { path:'myFollow',component:CalendarComponent,},
      { path:'my-issue',component:MyIssuesComponent,},
      
      
      
    ]},
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DepartmentManagerRoutingModule { }
