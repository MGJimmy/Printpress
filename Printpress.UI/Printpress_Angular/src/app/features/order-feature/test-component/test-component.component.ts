import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { HttpService } from '../../../core/services/http.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-test-component',
  standalone: true,
  imports: [TableModule,CommonModule],
  templateUrl: './test-component.component.html',
  styleUrl: './test-component.component.css'
})
export class TestComponentComponent implements OnInit {

  public data:any[] = [];

  constructor(private httpService:HttpService) { }
ngOnInit(): void {

  this.httpService.get('https://jsonplaceholder.typicode.com/posts').subscribe((res:any) => {
    this.data = res;
  });

}




}
