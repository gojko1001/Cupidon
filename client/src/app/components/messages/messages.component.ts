import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { Message } from 'src/app/model/message';
import { Pagination } from 'src/app/model/pagination';
import { ConfirmService } from 'src/app/services/confirm.service';
import { MessageService } from 'src/app/services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pageSize = 5;
  loading: boolean

  constructor(private messageService: MessageService,
              private confirmService: ConfirmService,
              private titleService: Title) {
                this.titleService.setTitle("Messages | Cupidon")
              }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pageSize, this.container).subscribe(response => {
      this.messages = response.result;
      this.pagination = response.pagination;
      this.loading = false;
    })
  }

  deleteMessage(id: number){
    this.confirmService.confirm("Delete message", "You are about to delete a message. This cannot be undone. Proceed?").subscribe(result => {
      if(result){
        this.messageService.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === id), 1);
        })
      }
    })
  }

  pageChanged(event: any){
    if(this.pageNumber !== event.page){
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

}
