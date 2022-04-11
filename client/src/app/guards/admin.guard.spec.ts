import { TestBed } from '@angular/core/testing';

import { AdminGuard } from './admin.guard';

xdescribe('AdminGuard', () => {
  let guard: AdminGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(AdminGuard);
  });
});
