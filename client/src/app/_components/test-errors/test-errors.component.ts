import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-test-errors',
  templateUrl: './test-errors.component.html',
  styleUrls: ['./test-errors.component.css']
})
export class TestErrorsComponent implements OnInit {
  baseUrl = 'https://localhost:5001/api/';
  bugyUrl = 'buggy/';
  validationErrors: string[] = [];
  
  constructor(private http: HttpClient) { }

  ngOnInit(): void {
  }

  get404Error(){
    this.http.get(this.bugyUrl + 'not-found').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get400Error(){
    this.http.get(this.bugyUrl + 'bad-request').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get401Error(){
    this.http.get(this.bugyUrl + 'auth').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get500Error(){
    this.http.get(this.bugyUrl + 'server-error').subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
    })
  }

  get400Validation(){
    this.http.post(this.baseUrl + 'account/register', {}).subscribe(response => {
      console.log(response);
    }, error => {
      console.log(error);
      this.validationErrors = error;
    })
  }

}
