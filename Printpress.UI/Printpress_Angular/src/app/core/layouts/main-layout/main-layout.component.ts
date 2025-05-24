import { Component } from '@angular/core';
import { SidebarComponent } from "../../component/sidebar/sidebar.component";
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [SidebarComponent, RouterModule],
  templateUrl: './main-layout.component.html'
})
export class MainLayoutComponent {
  title = 'Printpress_Angular';
}
