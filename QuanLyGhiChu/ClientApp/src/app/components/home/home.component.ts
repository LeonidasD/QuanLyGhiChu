import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { HttpParams, HttpClient } from '@angular/common/http';

@Component({
    selector: 'home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})

export class HomeComponent {
  httpClient: HttpClient;

    constructor(http: HttpClient, @Inject('BASE_URL') baseurl: string) {
      this.httpClient = http;
    }

    public postCreateNote(f: NgForm) {
      this.httpClient.post("http://127.0.0.1:3000/customers",
        {
          "name": "Customer004",
          "email": "customer004@email.com",
          "tel": "0000252525"
        })
        .subscribe(
          data => {
            console.log("POST Request is successful ", data);
          },
          error => {
            console.log("Error", error);
          }
        );  
    }
}
