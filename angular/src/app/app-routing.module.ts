import { TimeOnProjectComponent } from './timeOnProject/timeOnProject/timeOnProject.component';
import { FollowIssueComponent } from './follow-issue/follow-issue.component';
// import { MyIssuesComponent } from './MyIssues/my-issues/my-issues.component';
import { ViewComponent } from './view/view.component';
import { SettingComponent } from './setting/setting.component';
import { NotFoundComponent } from './not-found/not-found.component';
import { AddIssueComponent } from './add-issue/add-issue.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { IssuesComponent } from './issues/issues.component';
import { HomeComponent } from './home/home.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { AuthGuard } from './Service/Auth/auth.guard';
import { PVLBoardComponent } from './pvlboard/pvlboard.component';
import { MemberManagerComponent } from './member-manager/member-manager.component';
import { StatusManagerComponent } from './status-manager/status-manager.component';
import { MyIssuesComponent } from './MyIssues/my-issues/my-issues.component';
import { GeneralComponent } from './general/general.component';
import { CategoryComponent } from './category/category.component';
import { MessagerComponent } from './messager/messager.component';
import { MyDepartmentComponent } from './my-department/my-department.component';
import { MyTeamComponent } from './my-team/my-team.component';
import { ProfileComponent } from './profile/profile.component';

const routes: Routes = [

  { path: '',  loadChildren: () => import(`./department-manage/department-manager.module`).then(m => m.DepartmentManagerModule) },
  { path: 'sign-in/:projectId/:Email', component: SignInComponent},
  { path: 'sign-in', component: SignInComponent},
  {
    path: 'sign-up',
    component: SignUpComponent,
    children: [
      {
        path: 'enter-otp',
        component: SignUpComponent,
      },
      {
        path: 'submit',
        component: SignUpComponent,
      },

      {
        path: 'sync',
        component: SignUpComponent,
      },
    ],
  },
  {
    path: 'sign-up/:projectId/:Email',
    component: SignUpComponent,
    children: [
      {
        path: 'enter-otp',
        component: SignUpComponent,
      },
      {
        path: 'submit',
        component: SignUpComponent,
      },

      {
        path: 'sync',
        component: SignUpComponent,
      },
    ],
  },
  {
    path: 'sync-user',
    component: SignUpComponent,
    children: [
      {
        path: 'enter-otp',
        component: SignUpComponent,
      },
      {
        path: 'submit',
        component: SignUpComponent,
      },
      {
        path: 'sync',
        component: SignUpComponent,
      },
    ],
  },
  {
    path:'project/:idProject/add-issue',component:AddIssueComponent, canActivate: [AuthGuard]
  },
    {
    path:'my-profile',component:MyIssuesComponent,canActivate: [AuthGuard]
  },
  {
    path:'project/:idProject/my-follow-issues',component:FollowIssueComponent,canActivate: [AuthGuard]
  },
  {
    path:'profile',component:ProfileComponent,canActivate: [AuthGuard]
  },
  {
    path:'project/:idProject/home',component:HomeComponent,canActivate: [AuthGuard]
  } ,
  {
    path:'project/:idProject/issues',component:IssuesComponent,canActivate: [AuthGuard]
  } ,
  {
    path:'project/:idProject/board',component:PVLBoardComponent, canActivate: [AuthGuard]
  } ,
  {
    path:'project/:idProject/timeOnProject',component:TimeOnProjectComponent, canActivate: [AuthGuard]
  } ,
  {
    path:'project/:idProject/settings', canActivate: [AuthGuard],
    children: [
      {
        path: 'statistic',
        component: GeneralComponent,
      },
      {
        path: 'user',
        component: MemberManagerComponent,
      },

      {
        path: 'status',
        component: StatusManagerComponent,
      },
      {
        path: 'category',
        component: CategoryComponent,
      },
      {
        path: 'config',
        component: SettingComponent,
      },
    ],
  } ,
  { path: 'project/:idProject/issues/view/:id', component: ViewComponent,canActivate: [AuthGuard] },
  { path: 'issues/view/:id/edit', component: AddIssueComponent,canActivate: [AuthGuard] },
  { path: 'messenger',component:MessagerComponent,canActivate: [AuthGuard] },
  { path: 'departmentManage', loadChildren: () => import(`./department-manage/department-manager.module`).then(m => m.DepartmentManagerModule) },
  { path: 'my-department',  component:MyDepartmentComponent,canActivate:[AuthGuard] },
  { path: 'my-team',  component:MyTeamComponent,canActivate:[AuthGuard] },
  { path: 'profile',component:ProfileComponent,canActivate:[AuthGuard]  },
  { path: '**', component: NotFoundComponent,canActivate: [AuthGuard] },

];

@NgModule({
  imports: [RouterModule.forRoot(routes, {useHash: true})],
  exports: [RouterModule],
})
export class AppRoutingModule {}
