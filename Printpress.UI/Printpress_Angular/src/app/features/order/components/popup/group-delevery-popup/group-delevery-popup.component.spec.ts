import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GroupDeleveryPopupComponent } from './group-delevery-popup.component';

describe('GroupDeleveryPopupComponent', () => {
  let component: GroupDeleveryPopupComponent;
  let fixture: ComponentFixture<GroupDeleveryPopupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GroupDeleveryPopupComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GroupDeleveryPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
