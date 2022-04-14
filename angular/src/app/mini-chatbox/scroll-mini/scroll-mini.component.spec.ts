import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScrollMiniComponent } from './scroll-mini.component';

describe('ScrollMiniComponent', () => {
  let component: ScrollMiniComponent;
  let fixture: ComponentFixture<ScrollMiniComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScrollMiniComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScrollMiniComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
