import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { GoogleLoginProvider, SocialAuthServiceConfig, SocialLoginModule } from 'angularx-social-login';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { MemberListComponent } from './components/member/member-list/member-list.component';
import { MemberDetailComponent } from './components/member/member-detail/member-detail.component';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { MemberCardComponent } from './components/member/member-card/member-card.component';
import { MemberEditComponent } from './components/member/member-edit/member-edit.component';
import { MemberMessagesComponent } from './components/member/member-messages/member-messages.component';
import { PasswordChangeComponent } from './components/member/password-change/password-change.component';
import { PhotoEditorComponent } from './components/member/photo-editor/photo-editor.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';

import { ErrorInterceptor } from './interceptors/error.interceptor';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { SharedModule } from './modules/shared.module';
import { HasRoleDirective } from './directives/has-role.directive';
import { TextInputComponent } from './components/forms/text-input/text-input.component';
import { DateInputComponent } from './components/forms/date-input/date-input.component';
import { AdminPanelComponent } from './components/admin/admin-panel/admin-panel.component';
import { UserManagementComponent } from './components/admin/user-management/user-management.component';
import { PhotoManagementComponent } from './components/admin/photo-management/photo-management.component';
import { RolesModalComponent } from './components/modals/roles-modal/roles-modal.component';
import { ConfirmDialogComponent } from './components/modals/confirm-dialog/confirm-dialog.component';
import { environment } from 'src/environments/environment';


@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MemberDetailComponent,
    ListsComponent,
    MessagesComponent,
    NotFoundComponent,
    MemberCardComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    ServerErrorComponent,
    TextInputComponent,
    DateInputComponent,
    MemberMessagesComponent,
    AdminPanelComponent,
    HasRoleDirective,
    UserManagementComponent,
    PhotoManagementComponent,
    RolesModalComponent,
    ConfirmDialogComponent,
    PasswordChangeComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    SocialLoginModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},    
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},    
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    {
      provide: 'SocialAuthServiceConfig',
      useValue: {
        autoLogin: false,
        providers: [
          {
            id: GoogleLoginProvider.PROVIDER_ID,
            provider: new GoogleLoginProvider(
              environment.googleClientId
            )
          },
        ],
      } as SocialAuthServiceConfig
    }   
  ],
  bootstrap: [AppComponent],
  schemas:[
    NO_ERRORS_SCHEMA,
    CUSTOM_ELEMENTS_SCHEMA  // Added to suppress ngx gallery warning
  ]
})
export class AppModule { }
