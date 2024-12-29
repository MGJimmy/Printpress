import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HttpService } from './core/services/http.service';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from './core/component/loader/loader.component';
import { LoaderService } from './core/services/loader.service';
import {ProgressSpinnerMode, MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TableModule } from 'primeng/table';



@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, CommonModule,LoaderComponent,MatProgressBarModule,TableModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  title = 'Printpress_Angular';
  public isLoading: boolean = true;

  constructor(private httpService: HttpService,private loaderService:LoaderService) {

  }

  ngOnInit(): void {
  
    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });

    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });

    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });

    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });

    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });
    
    this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res) => {
      console.log(res);
    });


    this.loaderService.isLoading.subscribe((res) => {
      this.isLoading = res;
    });
    
  }
}