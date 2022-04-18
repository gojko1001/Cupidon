import { TestBed } from '@angular/core/testing';

import { UserRelationService } from './user-relation.service';

xdescribe('LikesService', () => {
  let service: UserRelationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UserRelationService);
  });
});
