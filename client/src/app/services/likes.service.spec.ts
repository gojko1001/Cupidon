import { TestBed } from '@angular/core/testing';

import { LikesService } from './likes.service';

xdescribe('LikesService', () => {
  let service: LikesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LikesService);
  });
});
