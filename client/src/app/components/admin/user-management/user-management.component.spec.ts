import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { of } from 'rxjs';
import { User } from 'src/app/model/user';
import { AdminService } from 'src/app/services/admin.service';

import { UserManagementComponent } from './user-management.component';

describe('UserManagementComponent', () => {
  let component: UserManagementComponent;
  let fixture: ComponentFixture<UserManagementComponent>;
  let adminService: AdminService;
  let modalService: BsModalService

  let user1: User = {
    username: 'alice',
    roles: ["Member"],
    gender: '',
    knownAs: '',
    profilePhotoUrl: '',
    publicActivity: null
  }

  let user2: User = {
    username: 'bob',
    roles: ["Moderator"],
    gender: '',
    knownAs: '',
    profilePhotoUrl: '',
    publicActivity: null
  }

  let user3: User = {
    username: 'john',
    roles: ["Member", "Moderator"],
    gender: '',
    knownAs: '',
    profilePhotoUrl: '',
    publicActivity: null
  }

  let testUsers: User[] = []

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UserManagementComponent ],
      imports: [
        HttpClientModule,
        ModalModule.forRoot(),
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    testUsers = [user1, user2, user3];
    fixture = TestBed.createComponent(UserManagementComponent);
    component = fixture.componentInstance;
    adminService = fixture.debugElement.injector.get(AdminService);
    modalService = fixture.debugElement.injector.get(BsModalService);
    fixture.detectChanges();
  });

  it('should call server to load users', () => {
    let spy = spyOn(adminService, 'getUsersWithRoles').and.returnValue(of(testUsers))

    component.ngOnInit();

    expect(spy).toHaveBeenCalled();
  })

  it('should load users from server', () => {
    spyOn(adminService, 'getUsersWithRoles').and.returnValue(of(testUsers))

    component.ngOnInit();

    expect(component.users.length).toBe(3);
  });
});
