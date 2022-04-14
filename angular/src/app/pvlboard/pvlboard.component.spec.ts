import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PVLBoardComponent } from './pvlboard.component';

describe('PVLBoardComponent', () => {
  let component: PVLBoardComponent;
  let fixture: ComponentFixture<PVLBoardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PVLBoardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PVLBoardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
