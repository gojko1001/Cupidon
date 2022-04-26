import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { RouterOutlet } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { NgxSpinnerModule } from 'ngx-spinner';
import { ToastrModule } from 'ngx-toastr';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { User } from './model/user';
import { AccountService } from './services/account.service';
import { PresenceService } from './services/presence.service';

xdescribe('AppComponent', () => {
  let fixture: ComponentFixture<AppComponent>;
  let component: AppComponent;
  let accService: AccountService;
  let presenceService: PresenceService;

  let testUser: User = {
    username: 'alice',
    profilePhotoUrl: '',
    knownAs: 'alice',
    gender: 'female',
    roles: [],
    publicActivity: true
  }

  let testToken = 'json.token.format';
  let testRefreshToken = 'testrftkn';

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        RouterTestingModule,
        HttpClientModule,
        ToastrModule.forRoot(),
        NgxSpinnerModule,
        FormsModule
      ],
      declarations: [ AppComponent, NavbarComponent ],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AppComponent);
    component = fixture.componentInstance;
    accService = fixture.debugElement.injector.get(AccountService);
    presenceService = fixture.debugElement.injector.get(PresenceService);

    fixture.detectChanges();
  })

  it('should have router outlet', () => {
    let routeroutlet = fixture.debugElement.query(By.directive(RouterOutlet))

    expect(routeroutlet).not.toBeNull();
  })
  
  it('should have navbar', () => {
    let navabar = fixture.debugElement.query(By.directive(NavbarComponent))

    expect(navabar).not.toBeNull();
  })
  

  // TODO: Mock loclastorage?
  it('should call service to set current user and start presence hub connection, if user is logged in', () => {
    spyOn(localStorage, 'getItem').withArgs('user').and.returnValue(JSON.stringify(testUser))
                                  .withArgs('ACTKN').and.returnValue(testToken)
                                  .withArgs('RFTKN').and.returnValue(testRefreshToken)
    let spy = spyOn(accService, 'setCurrentUser');
    let hubSpy = spyOn(presenceService, 'createHubConnection');

    component.ngOnInit();

    expect(spy).toHaveBeenCalledWith(testUser);
    expect(hubSpy).toHaveBeenCalled();
  })
  
  it('should NOT call service to set current user and  NOT start presence hub connection, if user is NOT logged in', () => {
    spyOn(localStorage, 'getItem').and.returnValue(null)
    let spy = spyOn(accService, 'setCurrentUser');
    let hubSpy = spyOn(presenceService, 'createHubConnection');

    component.ngOnInit();

    expect(spy).not.toHaveBeenCalled();
    expect(hubSpy).not.toHaveBeenCalled();
  })
});
