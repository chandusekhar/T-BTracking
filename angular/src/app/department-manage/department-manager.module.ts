//leaves.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DepartmentManageComponent } from './department-manage.component';
import { DepartmentManagerRoutingModule } from './department-manager-routing.module';
import { DemoNgZorroAntdModule } from '../shared/ng-zorro-antd.module';


@NgModule({
  declarations: [
      DepartmentManageComponent
  ],
  imports: [
    CommonModule,
    DepartmentManagerRoutingModule,
    DemoNgZorroAntdModule
  ]
})
export class DepartmentManagerModule { }
