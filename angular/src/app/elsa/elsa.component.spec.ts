/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { ElsaComponent } from './elsa.component';

describe('ElsaComponent', () => {
  let component: ElsaComponent;
  let fixture: ComponentFixture<ElsaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ElsaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ElsaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
