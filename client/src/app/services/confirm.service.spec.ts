import { TestBed } from '@angular/core/testing';

import { ConfirmService } from './confirm.service';

xdescribe('ConfirmService', () => {
  let service: ConfirmService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConfirmService);
  });
});
