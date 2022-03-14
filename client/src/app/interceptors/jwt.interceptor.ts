import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { getAccessToken } from '../services/tokenUtil';

@Injectable()
export class JwtInterceptor implements HttpInterceptor {

  constructor() {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    let accessToken = getAccessToken()
    if(accessToken){
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`
        }
      })
    }
    
    return next.handle(request);
  }
}
