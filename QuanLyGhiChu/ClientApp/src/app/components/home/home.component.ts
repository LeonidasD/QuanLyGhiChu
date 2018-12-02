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
      this.httpClient.post("/api/ghichu/create", f.value)
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
