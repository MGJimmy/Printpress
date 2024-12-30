import { Component, OnInit } from '@angular/core';
import { TableModule } from 'primeng/table';
import { HttpService } from '../../../core/services/http.service';
import { CommonModule } from '@angular/common';
import { apiUrlResource } from '../../../core/resources/api-urls.resource';

@Component({
  selector: 'app-test-component',
  standalone: true,
  imports: [TableModule, CommonModule],
  templateUrl: './test-component.component.html',
  styleUrl: './test-component.component.css',
})
export class TestComponentComponent implements OnInit {
  public data: any[] = [];

  constructor(private httpService: HttpService) {}
  ngOnInit(): void {
    this.httpService
      .get<any[]>(apiUrlResource.ClientAPI.GetClientById, { id: 1 })
      .subscribe((res) => {
        console.log(res);
      });
  }
}
