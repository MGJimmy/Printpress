import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable({ providedIn: 'root' })
export class TranslationService {

  constructor(private translate: TranslateService) {
    const savedLang = localStorage.getItem('lang') ?? 'ar';
    this.translate.setDefaultLang('ar');
    this.translate.use(savedLang);
  }

  /** Translate a key synchronously (key must be already loaded). */
  t(key: string, params?: object): string {
    return this.translate.instant(key, params);
  }

  setLanguage(lang: 'ar' | 'en') {
    localStorage.setItem('lang', lang);
    this.translate.use(lang);
  }

  get currentLang(): string {
    return this.translate.currentLang ?? 'ar';
  }
}
