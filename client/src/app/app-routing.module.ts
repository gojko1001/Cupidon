import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { PreventUnsavedChangesGuard } from './guards/prevent-unsaved-changes.guard';
import { HomeComponent } from './components/home/home.component';
import { ListsComponent } from './components/lists/lists.component';
import { MemberDetailComponent } from './components/member-detail/member-detail.component';
import { MemberEditComponent } from './components/member-edit/member-edit.component';
import { MemberListComponent } from './components/member-list/member-list.component';
import { MessagesComponent } from './components/messages/messages.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';
import { MemberDetailedResolver } from './resolvers/member-detailed.resolver';

const routes: Routes = [
  {path:'', component: HomeComponent},
  {
    path: '',
    runGuardsAndResolvers: 'always',
    canActivate: [AuthGuard],
    children: [
      {path:'members', component: MemberListComponent},
      {path:'members/:username', component: MemberDetailComponent, resolve: {member: MemberDetailedResolver}},
      {path:'member/edit', component: MemberEditComponent, canDeactivate: [PreventUnsavedChangesGuard]},
      {path:'lists', component: ListsComponent},
      {path:'messages', component: MessagesComponent},
    ]
  },
  {path: 'server-error', component: ServerErrorComponent},
  {path:'**', component: NotFoundComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
