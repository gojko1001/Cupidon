import { TestBed } from '@angular/core/testing';

import { AccountService } from './account.service';

xdescribe('AccountService', () => {
  let service: AccountService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AccountService);
  });
});
