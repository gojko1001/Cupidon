import { TestBed } from '@angular/core/testing';

import { ErrorInterceptor } from './error.interceptor';

xdescribe('ErrorInterceptor', () => {
  let interceptor: ErrorInterceptor;

  beforeEach(() => {TestBed.configureTestingModule({
    providers: [
      ErrorInterceptor
      ]
    });
    interceptor = TestBed.inject(ErrorInterceptor);
  });

});
