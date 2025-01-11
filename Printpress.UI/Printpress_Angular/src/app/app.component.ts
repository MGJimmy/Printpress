import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from './core/component/loader/loader.component';
import { AuthService } from './core/services/auth.service';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';



@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    CommonModule,
    LoaderComponent,
    SidebarComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {

  title = 'Printpress_Angular';

  isSidebarToggled: boolean = false;
  isAuthenticated = true;
  isLoggedIn: boolean = false;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.isAuthenticated = this.authService.isLoggedIn();
  }

  toggleSidebar() {
    this.isSidebarToggled = !this.isSidebarToggled;
  }

  onLogout(): void {
    this.authService.logout();
    this.isAuthenticated = false;
    this.isLoggedIn = false;
  }

  onLoginSuccess(): void {
    this.isAuthenticated = true;
    this.isLoggedIn = true;
  }
}
