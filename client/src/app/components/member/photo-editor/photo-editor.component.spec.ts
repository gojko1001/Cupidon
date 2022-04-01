import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PhotoEditorComponent } from './photo-editor.component';

xdescribe('PhotoEditorComponent', () => {
  let component: PhotoEditorComponent;
  let fixture: ComponentFixture<PhotoEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PhotoEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotoEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });
});
