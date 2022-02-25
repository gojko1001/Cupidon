import { NgModule } from '@angular/core';
import { CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http'
import { FormsModule, ReactiveFormsModule } from '@angular/forms'

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { HomeComponent } from './components/home/home.component';
import { RegisterComponent } from './components/register/register.component';
import { MemberListComponent } from './components/member-list/member-list.component';
import { MemberDetailComponent } from './components/member-detail/member-detail.component';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { MemberCardComponent } from './components/member-card/member-card.component';
import { MemberEditComponent } from './components/member-edit/member-edit.component';
import { PhotoEditorComponent } from './components/photo-editor/photo-editor.component';
import { ServerErrorComponent } from './components/server-error/server-error.component';

import { ErrorInterceptor } from './interceptors/error.interceptor';
import { JwtInterceptor } from './interceptors/jwt.interceptor';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { SharedModule } from './modules/shared.module';
import { TextInputComponent } from './forms/text-input/text-input.component';
import { DateInputComponent } from './forms/date-input/date-input.component';
import { MemberMessagesComponent } from './components/member-messages/member-messages.component';

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
    MemberMessagesComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  providers: [
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},    
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},    
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true}    
  ],
  bootstrap: [AppComponent],
  schemas:[
    CUSTOM_ELEMENTS_SCHEMA  // Added to suppress ngx gallery warning
  ]
})
export class AppModule { }
