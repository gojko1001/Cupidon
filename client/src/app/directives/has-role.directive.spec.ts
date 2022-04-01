import { HasRoleDirective } from './has-role.directive';

xdescribe('HasRoleDirective', () => {
  it('should create an instance', () => {
    const directive = new HasRoleDirective(null, null, null);
    expect(directive).toBeTruthy();
  });
});
