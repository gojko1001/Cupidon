import { TestBed } from '@angular/core/testing';

import { MessageService } from './message.service';

xdescribe('MessageService', () => {
  let service: MessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MessageService);
  });
});
