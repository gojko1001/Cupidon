import { TestBed } from '@angular/core/testing';

import { LoadingInterceptor } from './loading.interceptor';

xdescribe('LoadingInterceptor', () => {
  let interceptor: LoadingInterceptor;

  beforeEach(() => {
    TestBed.configureTestingModule({
    providers: [
      LoadingInterceptor
      ]
    });
    interceptor = TestBed.inject(LoadingInterceptor); 
  });
});
