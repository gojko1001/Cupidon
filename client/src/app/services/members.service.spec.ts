import { TestBed } from '@angular/core/testing';

import { MembersService } from './members.service';

xdescribe('MembersService', () => {
  let service: MembersService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MembersService);
  });
});
