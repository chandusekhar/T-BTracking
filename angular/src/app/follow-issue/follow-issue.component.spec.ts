import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FollowIssueComponent } from './follow-issue.component';

describe('FollowIssueComponent', () => {
  let component: FollowIssueComponent;
  let fixture: ComponentFixture<FollowIssueComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FollowIssueComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FollowIssueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
