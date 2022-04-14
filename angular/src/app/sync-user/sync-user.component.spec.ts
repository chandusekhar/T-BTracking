import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SyncUserComponent } from './sync-user.component';

describe('SyncUserComponent', () => {
  let component: SyncUserComponent;
  let fixture: ComponentFixture<SyncUserComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SyncUserComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SyncUserComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
