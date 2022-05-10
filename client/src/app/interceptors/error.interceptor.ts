import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, filter, finalize, switchMap, take } from 'rxjs/operators';
import { AccountService } from '../services/account.service';
import { getRefreshToken, setAccessToken, setRefreshToken } from '../services/tokenUtil';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  private refreshInProgress = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private router: Router,
              private toastr: ToastrService,
              private accountService: AccountService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors){
                const modalStateErrors = [];
                for (const key in error.error.errors){
                  if(error.error.errors[key]){
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                modalStateErrors.forEach(err => {
                  this.toastr.error(err, error.status.toString());
                });
                throw modalStateErrors.flat();
              } else if (typeof(error.error) === 'object'){
                this.toastr.error(error.error.message, error.status.toString());
              } else {
                this.toastr.error(error.error, error.status.toString())
              }
              break;
            case 401:
              if(error.headers.get('Token-Expired')){
                if(this.refreshInProgress){
                  return this.refreshTokenSubject.pipe(
                    filter((res) => res),
                    take(1),
                    switchMap(() => next.handle(request))
                  );
                } else {
                  this.refreshInProgress = true;
                  this.refreshTokenSubject.next(null);

                  return this.accountService.refreshToken(getRefreshToken()).pipe(
                    switchMap((token: any) => {
                      setRefreshToken(token.refreshToken);
                      setAccessToken(token.accessToken);
                      this.refreshTokenSubject.next(token.accessToken);
                      return next.handle(request);
                    }),
                    catchError(err => {
                      this.accountService.logout();
                      this.router.navigateByUrl('/');
                      return throwError(() => new Error(err.message));
                    }),
                    finalize(() => (this.refreshInProgress = false))
                  );
                }
              } else {
                if(!error.error){       // Error is null if token is invalid
                  this.accountService.logout();
                  this.router.navigateByUrl('/');
                  this.toastr.error("Invalid credentials");
                }else{
                  this.toastr.error(error.error.message, error.status.toString());
                }
              }
              break;
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(() => new Error(error.message));
      })
    );
  }
}
