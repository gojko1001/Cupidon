import { TestBed } from '@angular/core/testing';

import { PresenceService } from './presence.service';

xdescribe('PresenceService', () => {
  let service: PresenceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(PresenceService);
  });
});
