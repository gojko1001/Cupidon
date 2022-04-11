import { TestBed } from '@angular/core/testing';

import { PreventUnsavedChangesGuard } from './prevent-unsaved-changes.guard';

xdescribe('PreventUnsavedChangesGuard', () => {
  let guard: PreventUnsavedChangesGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(PreventUnsavedChangesGuard);
  });
});
