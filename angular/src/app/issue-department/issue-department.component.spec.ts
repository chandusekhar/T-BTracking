import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IssueDepartmentComponent } from './issue-department.component';

describe('IssueDepartmentComponent', () => {
  let component: IssueDepartmentComponent;
  let fixture: ComponentFixture<IssueDepartmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ IssueDepartmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(IssueDepartmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
