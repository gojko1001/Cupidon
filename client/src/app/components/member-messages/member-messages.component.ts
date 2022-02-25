import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Message } from 'src/app/model/message';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm: NgForm
  @Input() messages: Message[];

  username = this.route.snapshot.paramMap.get('username');
  messageContent: string;

  constructor(private messageService: MessageService,
              private route: ActivatedRoute) { }

  ngOnInit(): void {
  }

  sendMessage(){
    this.messageService.sendMessage(this.username, this.messageContent).subscribe(message => {
      this.messages.push(message);
      this.messageForm.reset();
    });
  }

}
