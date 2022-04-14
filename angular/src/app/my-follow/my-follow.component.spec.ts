import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MyFollowComponent } from './my-follow.component';

describe('MyFollowComponent', () => {
  let component: MyFollowComponent;
  let fixture: ComponentFixture<MyFollowComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MyFollowComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MyFollowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
