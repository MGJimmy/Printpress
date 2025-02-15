import { Router } from "@angular/router";

export class ComponentMode {
  public isEditMode: boolean;
  public isViewMode: boolean;
  public isaddMode: boolean;
  constructor(private router: Router) {
    this.isViewMode = this.router.url.includes('view');
    this.isEditMode = this.router.url.includes('edit');
    this.isaddMode = this.router.url.includes('add');
  }
}