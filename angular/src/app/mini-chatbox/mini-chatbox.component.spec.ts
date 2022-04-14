import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MiniChatboxComponent } from './mini-chatbox.component';

describe('MiniChatboxComponent', () => {
  let component: MiniChatboxComponent;
  let fixture: ComponentFixture<MiniChatboxComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MiniChatboxComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MiniChatboxComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
