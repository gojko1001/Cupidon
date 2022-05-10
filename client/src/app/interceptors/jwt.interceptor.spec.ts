import { TestBed } from '@angular/core/testing';

import { JwtInterceptor } from './jwt.interceptor';

xdescribe('JwtInterceptor', () => {
  let interceptor: JwtInterceptor;

  beforeEach(() => {TestBed.configureTestingModule({
    providers: [
      JwtInterceptor
      ]
    });
    interceptor = TestBed.inject(JwtInterceptor);
  });
});
