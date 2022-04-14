import { InterceptorService } from './Service/Loader/interceptor.service';
import { ProjectComponent } from './project/project.component';
import { AccountConfigModule } from '@abp/ng.account/config';
import { CoreModule } from '@abp/ng.core';
import { registerLocale, storeLocaleData } from '@abp/ng.core/locale';
import { IdentityConfigModule } from '@abp/ng.identity/config';
import { SettingManagementConfigModule } from '@abp/ng.setting-management/config';
import { TenantManagementConfigModule } from '@abp/ng.tenant-management/config';
import { ThemeBasicModule } from '@abp/ng.theme.basic';
import { ThemeSharedModule } from '@abp/ng.theme.shared';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgxsModule } from '@ngxs/store';
import { environment } from '../environments/environment';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { APP_ROUTE_PROVIDER } from './route.provider';
import { NZ_I18N } from 'ng-zorro-antd/i18n';
import { en_US } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import vi from '@angular/common/locales/vi';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AddIssueComponent } from './add-issue/add-issue.component';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzDatePickerModule } from 'ng-zorro-antd/date-picker';
import { NzSelectModule } from 'ng-zorro-antd/select';
import { NzUploadModule } from 'ng-zorro-antd/upload';
import { IconsProviderModule } from './icons-provider.module';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { NzMenuModule } from 'ng-zorro-antd/menu';
import { NzSpinModule } from 'ng-zorro-antd/spin';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzMessageModule } from 'ng-zorro-antd/message';
import { NzPaginationModule } from 'ng-zorro-antd/pagination';
import { NzResultModule } from 'ng-zorro-antd/result';
import { NotFoundComponent } from './not-found/not-found.component';
import { NzDividerModule } from 'ng-zorro-antd/divider';
import { NzDrawerModule } from 'ng-zorro-antd/drawer';
import { SettingComponent } from './setting/setting.component';
import { IssuesComponent } from './issues/issues.component';
import { NzCheckboxModule } from 'ng-zorro-antd/checkbox';
import { NzDropDownModule } from 'ng-zorro-antd/dropdown';
import { NzTagModule } from 'ng-zorro-antd/tag';
import { NzTableModule } from 'ng-zorro-antd/table';
import { ViewComponent } from './view/view.component';
import { NzCommentModule } from 'ng-zorro-antd/comment';
import { NzListModule } from 'ng-zorro-antd/list';
import { NzAvatarModule } from 'ng-zorro-antd/avatar';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzImageModule } from 'ng-zorro-antd/image';
import { WelcomeComponent } from './welcome/welcome.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { BoardComponent } from './board/board.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NzFormModule } from 'ng-zorro-antd/form';
import { SignUpComponent } from './sign-up/sign-up.component';
import { PVLBoardComponent } from './pvlboard/pvlboard.component';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzBadgeModule } from 'ng-zorro-antd/badge';
import { SyncUserComponent } from './sync-user/sync-user.component';
import { HomeComponent } from './home/home.component';
import { NzProgressModule } from 'ng-zorro-antd/progress';
import { NzTypographyModule } from 'ng-zorro-antd/typography';
import { NzEmptyModule } from 'ng-zorro-antd/empty';
import { NzPopconfirmModule } from 'ng-zorro-antd/popconfirm';
import { NzSpaceModule } from 'ng-zorro-antd/space';
import { NzCollapseModule } from 'ng-zorro-antd/collapse';
import { NzSkeletonModule } from 'ng-zorro-antd/skeleton';
import { NzNotificationModule } from 'ng-zorro-antd/notification';
import { FomateDatePipe } from './helper/fomate-date.pipe';
import { FomateStringPipe } from './helper/formate-string';
import { MemberManagerComponent } from './member-manager/member-manager.component';
import { MyIssuesComponent } from './MyIssues/my-issues/my-issues.component';
import { StatusManagerComponent } from './status-manager/status-manager.component';
import { FomateTimePiPe } from './helper/formate-tiem.pipe';
import { NzTimelineModule } from 'ng-zorro-antd/timeline';
import { NzTabsModule } from 'ng-zorro-antd/tabs';
import { GeneralComponent } from './general/general.component';
import { CategoryComponent } from './category/category.component';
import { NzPageHeaderModule } from 'ng-zorro-antd/page-header';
import { NzDescriptionsModule } from 'ng-zorro-antd/descriptions';
import { NzStepsModule } from 'ng-zorro-antd/steps';
import { FomateNumberPipe } from './helper/formate-number.pipe';
import { FollowIssueComponent } from './follow-issue/follow-issue.component';
import { NzCalendarModule } from 'ng-zorro-antd/calendar';
import { NzSwitchModule } from 'ng-zorro-antd/switch';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { ChartsModule } from 'ng2-charts';
import { ProgressChildComponent } from './follow-issue/progress-child.component';
import { TimeLineComponent } from './follow-issue/time-line/time-line.component';
import { NzPopoverModule } from 'ng-zorro-antd/popover';
import { FullCalendarModule } from '@fullcalendar/angular';
import dayGridPlugin from '@fullcalendar/daygrid'; // a plugin!
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import bootstrapPlugin from '@fullcalendar/bootstrap';
import listPlugin from '@fullcalendar/list';
import { NzCarouselModule } from 'ng-zorro-antd/carousel';
import { FomateHistoryPipe } from './helper/formate-history';
import { AdminComponent } from './admin/admin.component';
import { StatusTagChildComponent } from './follow-issue/status-tag-child.component';
import { QuillModule } from 'ngx-quill';
import { NzRadioModule } from 'ng-zorro-antd/radio';
import { NzAlertModule } from 'ng-zorro-antd/alert';
import { MessagerComponent } from './messager/messager.component';
import { MiniChatboxComponent } from './mini-chatbox/mini-chatbox.component';
import { HashLocationStrategy, LocationStrategy } from '@angular/common';
import { NzMentionModule } from 'ng-zorro-antd/mention';
import { DepartmentComponent } from './department/department.component';
import { NzBreadCrumbModule } from 'ng-zorro-antd/breadcrumb';
import { PickerModule } from '@ctrl/ngx-emoji-mart';
import { formatMaxstring } from './helper/formate-maxString';
import { ScrollMiniComponent } from './mini-chatbox/scroll-mini/scroll-mini.component';
import { TeamComponent } from './team/team.component';
import { IssueDepartmentComponent } from './issue-department/issue-department.component';
import { MyDepartmentComponent } from './my-department/my-department.component';
import { MyTeamComponent } from './my-team/my-team.component';
import { CustomDayPipe } from './helper/Format-Time-Chat';
import { NzAutocompleteModule } from 'ng-zorro-antd/auto-complete';
import { MyFollowComponent } from './my-follow/my-follow.component';
import { CustomDayMemberPipe } from './helper/formateTime-ListMember';
import { DrawerChatComponent } from './Drawer-Chat/drawer-chat/drawer-chat.component';
import { ProfileComponent } from './profile/profile.component';
import { CalendarComponent } from './Calendar/calendar/calendar.component';
import { CustomName } from './helper/CustomNameUser';
import { ElsaComponent } from './elsa/elsa.component';
import { DateAgoPipe } from './helper/Notification';
import { DashboardComponent } from './dashboard/dashboard.component';
import { TimeOnProjectComponent } from './timeOnProject/timeOnProject/timeOnProject.component';
FullCalendarModule.registerPlugins([
  dayGridPlugin,
  timeGridPlugin,
  interactionPlugin,
  bootstrapPlugin,
  listPlugin,
]);

import(
  /* webpackChunkName: "_locale-your-locale-js"*/
  /* webpackMode: "eager" */
  '@angular/common/locales/vi'
).then(m => storeLocaleData(m.default, 'vi'));
registerLocaleData(vi);
@NgModule({
  imports: [
    NzAutocompleteModule,
    NzMentionModule,
    PickerModule,
    NzAlertModule,
    NzTypographyModule,
    NzRadioModule,
    NzCarouselModule,
    FullCalendarModule,
    NzPopoverModule,
    ChartsModule,
    NzSwitchModule,
    NzStatisticModule,
    NzCalendarModule,
    NzStepsModule,
    NzDescriptionsModule,
    NzPageHeaderModule,
    NzTimelineModule,
    NzTabsModule,
    NzNotificationModule,
    NzSkeletonModule,
    NzCollapseModule,
    NzProgressModule,
    NzEmptyModule,
    NzSpaceModule,
    NzPopconfirmModule,
    NzTypographyModule,
    NzBadgeModule,
    NzCardModule,
    ReactiveFormsModule,
    NzFormModule,
    NzAvatarModule,
    NzTableModule,
    DragDropModule,
    NzTagModule,
    NzDropDownModule,
    NzCheckboxModule,
    BrowserModule,
    NzDividerModule,
    NzDrawerModule,
    NzModalModule,
    NzAvatarModule,
    NzInputModule,
    NzCommentModule,
    NzUploadModule,
    NzResultModule,
    NzSpinModule,
    NzListModule,
    NzMessageModule,
    NzGridModule,
    NzDatePickerModule,
    NzPaginationModule,
    NzSelectModule,
    BrowserAnimationsModule,
    NzIconModule,
    AppRoutingModule,
    NzButtonModule,
    NzToolTipModule,
    NzImageModule,
    NzBreadCrumbModule,
    NzCarouselModule,
    QuillModule.forRoot(),
    CoreModule.forRoot({
      environment,
      registerLocaleFn: registerLocale(),
    }),
    ThemeSharedModule.forRoot(),
    AccountConfigModule.forRoot(),
    IdentityConfigModule.forRoot(),
    TenantManagementConfigModule.forRoot(),
    SettingManagementConfigModule.forRoot(),
    NgxsModule.forRoot([], { developmentMode: !environment.production }),
    ThemeBasicModule.forRoot(),
    FormsModule,
    HttpClientModule,
    IconsProviderModule,
    NzLayoutModule,
    NzMenuModule,
  ],
  declarations: [
    TimeOnProjectComponent,
    DateAgoPipe,
    CustomName,
    DrawerChatComponent,
    CustomDayMemberPipe,
    CustomDayPipe,
    formatMaxstring,
    StatusTagChildComponent,
    ProgressChildComponent,
    FomateHistoryPipe,
    TimeLineComponent,
    FomateNumberPipe,
    FomateTimePiPe,
    AppComponent,
    HomeComponent,
    AddIssueComponent,
    NotFoundComponent,
    SettingComponent,
    IssuesComponent,
    ViewComponent,
    WelcomeComponent,
    SignInComponent,
    PVLBoardComponent,
    SignUpComponent,
    BoardComponent,
    SyncUserComponent,
    ProjectComponent,
    FomateDatePipe,
    FomateStringPipe,
    GeneralComponent,
    MyIssuesComponent,
    MemberManagerComponent,
    StatusManagerComponent,
    CategoryComponent,
    DashboardComponent,
    FollowIssueComponent,
    AdminComponent,
    MessagerComponent,
    MiniChatboxComponent,
    DepartmentComponent,
    ScrollMiniComponent,
    TeamComponent,
    IssueDepartmentComponent,
    MyDepartmentComponent,
    MyTeamComponent,
    MyFollowComponent,
    ProfileComponent,
    CalendarComponent,
    ElsaComponent,
  ],

  providers: [
    APP_ROUTE_PROVIDER,
    { provide: NZ_I18N, useValue: en_US },
    { provide: HTTP_INTERCEPTORS, useClass: InterceptorService, multi: true },
    { provide: LocationStrategy, useClass: HashLocationStrategy },
  ],
  bootstrap: [AppComponent],
  schemas: [CUSTOM_ELEMENTS_SCHEMA, NO_ERRORS_SCHEMA],
})
export class AppModule {}
