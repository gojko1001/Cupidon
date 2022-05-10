import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private router: Router,
              private toastr: ToastrService) {}

  canActivate(): Observable<boolean> {
    if(localStorage.getItem('user')) return of(true);
    
    this.router.navigateByUrl('');
    this.toastr.error('You need to be logged in to access this source!', 'Not authorized!');
  }
  
}
