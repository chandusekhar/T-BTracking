/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { ElsaService } from './elsa.service';

describe('Service: Elsa', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ElsaService]
    });
  });

  it('should ...', inject([ElsaService], (service: ElsaService) => {
    expect(service).toBeTruthy();
  }));
});
