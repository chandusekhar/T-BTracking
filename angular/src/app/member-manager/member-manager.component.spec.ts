import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MemberManagerComponent } from './member-manager.component';

describe('MemberManagerComponent', () => {
  let component: MemberManagerComponent;
  let fixture: ComponentFixture<MemberManagerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberManagerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberManagerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
