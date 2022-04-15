import { HttpClientModule } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { of } from 'rxjs';
import { PaginatedResult, Pagination } from 'src/app/model/pagination';
import { Member } from 'src/app/model/user';
import { UserRelationService } from 'src/app/services/user-relation.service';

import { ListsComponent } from './lists.component';

describe('ListsComponent', () => {
  let component: ListsComponent;
  let fixture: ComponentFixture<ListsComponent>;
  let userRelationService: UserRelationService;

  let testMember1: Member = { 
    id: 1,
    username: 'alice', 
    knownAs: 'alice', 
    photoUrl: '', 
    age: 18, 
    city: '',
    country: '',
    created: new Date(),
    lastActive: new Date(),
    gender: '',
    interests: '',
    introduction: '',
    lookingFor: '',
    photos: []
  }

  let testMember2: Member = { 
    id: 2,
    username: 'bob', 
    knownAs: 'bob', 
    photoUrl: '', 
    age: 18, 
    city: '',
    country: '',
    created: new Date(),
    lastActive: new Date(),
    gender: '',
    interests: '',
    introduction: '',
    lookingFor: '',
    photos: []
  }
  
  let testMember3: Member = { 
    id: 3,
    username: 'john', 
    knownAs: 'john', 
    photoUrl: '', 
    age: 18, 
    city: '',
    country: '',
    created: new Date(),
    lastActive: new Date(),
    gender: '',
    interests: '',
    introduction: '',
    lookingFor: '',
    photos: []
  }

  let members: Member[] = [];
  let pagination: Pagination = { currentPage: 1, itemsPerPage: 5, totalItems: 25, totalPages: 5};
  let paginatedResult: PaginatedResult<Member[]> = { result: [], pagination: pagination};

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ListsComponent ],
      imports: [ 
        HttpClientModule,
        PaginationModule.forRoot(),
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    members = [testMember1, testMember2, testMember3];
    paginatedResult.result = members;
    fixture = TestBed.createComponent(ListsComponent);
    component = fixture.componentInstance;
    component.predicate = 'liked'
    component.pageNumber = 1;
    component.pageSize = 5;
    userRelationService = fixture.debugElement.injector.get(UserRelationService);
    fixture.detectChanges();
  });

  it("should call a server to load user's like lists", () => {
    let spy = spyOn(userRelationService, 'getRelations').and.returnValue(of(paginatedResult))

    component.ngOnInit();

    expect(spy).toHaveBeenCalled();
  });
  
  it("should load user's like lists and set pagination", () => {
    spyOn(userRelationService, 'getRelations').and.returnValue(of(paginatedResult))

    component.ngOnInit();

    expect(component.members.length).toEqual(3);
    expect(component.pagination).toEqual(pagination);
  });

  it('should change page number if user click on different page', () => {
    component.pagination = pagination;
    fixture.detectChanges();
    let paginationEl = fixture.debugElement.query(By.css('pagination'));
    spyOn(userRelationService, 'getRelations').and.returnValue(of(paginatedResult))

    paginationEl.triggerEventHandler('pageChanged', { page: 2});

    expect(component.pageNumber).toBe(2);
  })
});
