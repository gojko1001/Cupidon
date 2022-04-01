import { TestBed } from '@angular/core/testing';

import { BusyService } from './busy.service';

xdescribe('BusyService', () => {
  let service: BusyService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(BusyService);
  });
});
