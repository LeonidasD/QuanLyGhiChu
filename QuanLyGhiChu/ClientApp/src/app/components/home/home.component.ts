import { Component, Inject } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Http} from '@angular/http';
import { Router } from '@angular/router';

@Component({
  selector: 'home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})

export class HomeComponent {
  http: Http;
  router: Router;
  noteURL: string;
  textareaValue: string = "";


  constructor(http: Http, router: Router) {
    this.http = http;
    this.router = router;
  }

  public postCreateNote(f: NgForm) {
    
  this.http.post("/api/ghichu/create", f.value)
      .subscribe(
        data => {
          if (data.status === 200) {
            console.log(data.text());
            this.noteURL = document.getElementsByTagName('base')[0].href + data.json()['code'];
            let alert_string = "Tiêu đề ghi chú: " + f.controls["title"].value
              + "\nID ghi chú: " + data.json()['code']
              + "\nHãy vào đường dẫn chứa ID để chỉnh sửa ghi chú";
            alert(alert_string);
          }
        },
        error => {
          console.log("Error", error);
        }
      );  
  }
}

interface Note {
  title: string,
  context: string,
  token: string,
  hashcode: string
}
