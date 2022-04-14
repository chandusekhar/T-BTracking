import { TestBed } from '@angular/core/testing';

import { CreateMessageService } from './create-message.service';

describe('CreateMessageService', () => {
  let service: CreateMessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(CreateMessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
