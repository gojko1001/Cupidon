import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ToastrModule } from 'ngx-toastr';
import { of } from 'rxjs';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { UserParams } from 'src/app/model/user';
import { MembersService } from 'src/app/services/members.service';

import { MemberListComponent } from './member-list.component';

describe('MemberListComponent', () => {
  let component: MemberListComponent;
  let fixture: ComponentFixture<MemberListComponent>;
  let memberService: MembersService;

  let defaultUserParams: UserParams = {
    gender: 'male',
    minAge: 18,
    maxAge: 99,
    pageNumber: 1,
    pageSize: 5,
    orederBy: 'lastActive',
  }

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberListComponent ],
      imports: [
        HttpClientModule,
        ToastrModule.forRoot(),
        AppRoutingModule,
        PaginationModule.forRoot(),
        FormsModule,
        ReactiveFormsModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberListComponent);
    component = fixture.componentInstance;
    component.userParams = defaultUserParams;
    memberService = fixture.debugElement.injector.get(MembersService)
    fixture.detectChanges();

    spyOn(memberService, 'setUserParams').and.returnValue()
  });

  it('should load members', () => {
    spyOn(memberService, 'getMembers').and.returnValue(of({result: [{id: 1}]}))
    
    component.ngOnInit();

    expect(component.members).toBeDefined();
  })
  
  it('should set pagination parameters', () => {
    component.pagination = undefined;
    spyOn(memberService, 'getMembers').and.returnValue(of({pagination: [{id: 1}]}))

    component.ngOnInit();

    expect(component.pagination).toBeDefined();
  })

  it('should reset user parameters to default when i click on reset filter button', () => {
    component.userParams = {
      gender: 'female',
      minAge: 25,
      maxAge: 33,
      pageNumber: 3,
      pageSize: 3,
      orederBy: 'lastActive',
    };
    let resetFiltersBtn = fixture.debugElement.query(By.css('#resetFiltersBtn'))
    spyOn(memberService, 'getMembers').and.returnValue(of({result: [{id: 1}]}))
    spyOn(memberService, 'resetUserParams').and.returnValue(defaultUserParams)

    resetFiltersBtn.triggerEventHandler('click', null);

    expect(component.userParams).toEqual(defaultUserParams)
  })

  it('should change page number if user click on different page', () => {
    component.pagination = {
      currentPage: 1,
      itemsPerPage: 5,
      totalItems: 25,
      totalPages: 5
    };
    fixture.detectChanges();
    let pagination = fixture.debugElement.query(By.css('pagination'));
    spyOn(memberService, 'getMembers').and.returnValue(of({result: [{id: 1}]}))

    pagination.triggerEventHandler('pageChanged', { page: 2});

    expect(component.userParams.pageNumber).toBe(2);
  })
});
