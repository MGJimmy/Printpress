import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
  forwardRef,
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

export type SearchSelectValue = string | number | boolean | null;

export interface SearchSelectItem {
  value: SearchSelectValue;
  label: string;
  disabled?: boolean;
}

export function normalizeSearchText(value: string): string {
  return (value ?? '')
    .trim()
    .toLowerCase()
    .replace(/[\u064B-\u065F\u0670\u06D6-\u06ED]/g, '')
    .replace(/[أإآ]/g, 'ا')
    .replace(/ى/g, 'ي')
    .replace(/ة/g, 'ه');
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
  styles: [':host { display: block; }'],
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
  private currentValue: SearchSelectValue = null;

  @Input() set items(value: SearchSelectItem[]) {
    this._items = value ?? [];
    const query = this.searchCtrl.value ?? '';
    const typing = typeof query === 'string' && query.length > 0 && !this.selectedItem;
    this.showValue(this.currentValue);
    if (typing) {
      this.selectedItem = null;
      this.searchCtrl.setValue(query, { emitEvent: false });
      this.filteredItems = this.filter(query);
    }
  }
  get items(): SearchSelectItem[] {
    return this._items;
  }

  @Input() label = '';
  @Input() placeholder = 'ابحث...';
  @Input() required = false;
  @Input() allowClear = true;
  @Input() set disabled(value: boolean) {
    this.setDisabledState(!!value);
  }

  @Output() selectionChange = new EventEmitter<SearchSelectValue>();

  @ViewChild(MatAutocompleteTrigger) autoTrigger!: MatAutocompleteTrigger;

  searchCtrl = new FormControl<string>('');
  filteredItems: SearchSelectItem[] = [];
  selectedItem: SearchSelectItem | null = null;

  private onChange: (value: SearchSelectValue) => void = () => {};
  onTouched: () => void = () => {};
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.searchCtrl.valueChanges.pipe(takeUntil(this.destroy$)).subscribe((value) => {
      if (typeof value !== 'string') return;
      this.filteredItems = this.filter(value);
      if (this.selectedItem && value !== this.selectedItem.label) {
        this.selectedItem = null;
        this.currentValue = null;
        this.onChange(null);
        this.selectionChange.emit(null);
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private filter(query: string): SearchSelectItem[] {
    const text = normalizeSearchText(query);
    if (!text) return [...this.items];
    return this.items.filter((item) => normalizeSearchText(item.label).includes(text));
  }

  onOptionSelected(event: MatAutocompleteSelectedEvent): void {
    const found = event.option.value as SearchSelectItem;
    if (found?.disabled) return;
    this.selectedItem = found;
    this.currentValue = found.value;
    this.searchCtrl.setValue(found.label, { emitEvent: false });
    this.filteredItems = [...this.items];
    this.onChange(found.value);
    this.selectionChange.emit(found.value);
    this.onTouched();
  }

  onSearchClick(): void {
    if (this.searchCtrl.disabled) return;
    this.filteredItems = this.filter(this.searchCtrl.value ?? '');
    this.autoTrigger?.openPanel();
  }

  onEnterKey(event: Event): void {
    if (this.autoTrigger?.activeOption) return;
    event.preventDefault();
    this.onSearchClick();
  }

  onClear(): void {
    this.selectedItem = null;
    this.currentValue = null;
    this.searchCtrl.setValue('', { emitEvent: false });
    this.filteredItems = [...this.items];
    this.onChange(null);
    this.selectionChange.emit(null);
    this.onTouched();
  }

  writeValue(value: SearchSelectValue): void {
    this.currentValue = value ?? null;
    this.showValue(this.currentValue);
  }

  registerOnChange(fn: (value: SearchSelectValue) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    if (isDisabled === this.searchCtrl.disabled) return;
    if (isDisabled) this.searchCtrl.disable({ emitEvent: false });
    else this.searchCtrl.enable({ emitEvent: false });
  }

  private showValue(value: SearchSelectValue): void {
    const found = this._items.find((item) => item.value === value) ?? null;
    if (found) {
      this.selectedItem = found;
      this.searchCtrl.setValue(found.label, { emitEvent: false });
    } else if (this._items.length > 0 || value === null || value === undefined) {
      this.selectedItem = null;
      this.searchCtrl.setValue('', { emitEvent: false });
    }
    this.filteredItems = [...this._items];
  }
}
