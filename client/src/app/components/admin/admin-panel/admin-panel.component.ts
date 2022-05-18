import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {

  constructor(private titleService: Title) {
    this.titleService.setTitle("Admin panel | Cupidon")
  }

  ngOnInit(): void {
  }

}
