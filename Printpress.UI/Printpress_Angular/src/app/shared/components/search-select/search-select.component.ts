import {
  Component,
  Input,
  OnDestroy,
  OnInit,
  forwardRef,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  FormControl,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
} from '@angular/forms';
import { MatAutocompleteModule, MatAutocompleteSelectedEvent, MatAutocompleteTrigger } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { Subject, takeUntil } from 'rxjs';

export interface SearchSelectItem {
  id: string;
  name: string;
}

@Component({
  selector: 'app-search-select',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
  ],
  templateUrl: './search-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => SearchSelectComponent),
      multi: true,
    },
  ],
})
export class SearchSelectComponent implements ControlValueAccessor, OnInit, OnDestroy {
  private _items: SearchSelectItem[] = [];
  private _pendingValue: string | null = null;

  @Input() set items(value: SearchSelectItem[]) {
    this._items = value ?? [];
    // Re-apply current query so an in-progress search is not wiped out
    const query = this.searchCtrl?.value ?? '';
    this.filteredItems = (query && !this.selectedItem)
      ? this.filter(query)
      : [...this._items];
    // Resolve a value that arrived before the items list was populated
    if (this._pendingValue) {
      this.writeValue(this._pendingValue);
    }
  }
  get items(): SearchSelectItem[] {
    return this._items;
  }

  @Input() label: string = '';
  @Input() placeholder: string = 'ابحث...';
  @Input() required: boolean = false;
  @Input() allowClear: boolean = true;

  @ViewChild(MatAutocompleteTrigger) autoTrigger!: MatAutocompleteTrigger;

  searchCtrl = new FormControl<string>('');
  filteredItems: SearchSelectItem[] = [];
  selectedItem: SearchSelectItem | null = null;

  private onChange: (value: string | null) => void = () => {};
  onTouched: () => void = () => {};
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.searchCtrl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => {
      if (typeof value === 'string') {
        this.filteredItems = this.filter(value);
        if (this.selectedItem) {
          this.selectedItem = null;
          this.onChange(null);
        }
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private filter(query: string): SearchSelectItem[] {
    const lower = query.toLowerCase().trim();
    if (!lower) return [...this.items];
    return this.items.filter((item) => item.name.toLowerCase().includes(lower));
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const id = event.option.value as string;
    const found = this.items.find((i) => i.id === id) ?? null;
    this.selectedItem = found;
    this.searchCtrl.setValue(found?.name ?? '', { emitEvent: false });
    this.filteredItems = [...this.items];
    this.onChange(id);
    this.onTouched();
  }

  onSearchClick(): void {
    if (this.searchCtrl.disabled) return;
    const query = this.searchCtrl.value ?? '';
    this.filteredItems = this.filter(query);
    this.autoTrigger?.openPanel();
  }

  onEnterKey(event: Event): void {
    // If autocomplete already has an active option, let it handle the selection
    if (this.autoTrigger?.activeOption) return;
    event.preventDefault();
    this.onSearchClick();
  }

  onClear(): void {
    this.selectedItem = null;
    this.searchCtrl.setValue('', { emitEvent: false });
    this.filteredItems = [...this.items];
    this.onChange(null);
    this.onTouched();
  }

  // ControlValueAccessor

  writeValue(id: string | null): void {
    if (id) {
      const found = this._items.find((i) => i.id === id) ?? null;
      if (found) {
        this.selectedItem = found;
        this.searchCtrl.setValue(found.name, { emitEvent: false });
        this._pendingValue = null;
      } else {
        // Items not loaded yet — store and resolve later in the items setter
        this._pendingValue = id;
      }
    } else {
      this.selectedItem = null;
      this.searchCtrl.setValue('', { emitEvent: false });
      this._pendingValue = null;
    }
    this.filteredItems = [...this._items];
  }

  registerOnChange(fn: (value: string | null) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    isDisabled ? this.searchCtrl.disable() : this.searchCtrl.enable();
  }
}
