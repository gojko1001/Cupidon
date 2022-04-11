import { HttpClientModule } from '@angular/common/http';
import { DebugElement } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { AppRoutingModule } from 'src/app/app-routing.module';
import { MessageService } from 'src/app/services/message.service';

import { MemberMessagesComponent } from './member-messages.component';

describe('MemberMessagesComponent', () => {
  let component: MemberMessagesComponent;
  let fixture: ComponentFixture<MemberMessagesComponent>;
  let messageService: MessageService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ MemberMessagesComponent ],
      imports: [
        HttpClientModule,
        FormsModule,
        AppRoutingModule
      ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberMessagesComponent);
    component = fixture.componentInstance;
    messageService = fixture.debugElement.injector.get(MessageService);
    fixture.detectChanges();
  });

  describe('when i click on send Message button', () => {
    let messageForm: DebugElement;
    let messageContent = "Test message"

    beforeEach(() => {
      messageForm = fixture.debugElement.query(By.css("#messageForm"));
      messageForm.nativeElement["messageContent"].value = messageContent;
    })
    
    it('should call server to send message', () => {
      let messageSpy = spyOn(messageService, 'sendMessage').and.returnValue(Promise.resolve())
  
      messageForm.triggerEventHandler('submit', null);
  
      expect(messageSpy).toHaveBeenCalled();
    })
    
    it('should set messageform input to empty string', (done) => {
      spyOn(messageService, 'sendMessage').and.returnValue(Promise.resolve())
  
      messageForm.triggerEventHandler('submit', null);

      fixture.whenStable().then(() => {
        expect(messageForm.nativeElement["messageContent"].value).toEqual('');
      });
      done();
    })

  })


});
