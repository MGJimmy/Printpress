import { APP_INITIALIZER, ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { loaderInterceptor } from './core/interceptors/loader.interceptor';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { ConfigurationService } from './core/services/configuration.service';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { getArabicPaginatorIntl } from './shared/components/shared-pagination/arabic-paginator-intl';
import { provideTranslateService, provideTranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader, provideTranslateHttpLoader } from '@ngx-translate/http-loader';


export const appConfig: ApplicationConfig = {
  providers: [
     provideZoneChangeDetection({ eventCoalescing: true }),
     provideRouter(routes),
     provideHttpClient(withInterceptors([authInterceptor, loaderInterceptor, errorInterceptor])),
     {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [ConfigurationService],
      multi: true,
    },
    provideAnimations(),
    provideToastr({
      positionClass: 'toast-top-right',
      preventDuplicates: true
    }),
 
      { provide: MatPaginatorIntl,
       useValue: getArabicPaginatorIntl()
      },
    ...provideTranslateService({ defaultLanguage: 'ar' }),
    provideTranslateLoader(TranslateHttpLoader),
    ...provideTranslateHttpLoader(),
    ],

};

function initializeApp(configService: ConfigurationService): () => Promise<void> {
  return () => configService.loadConfiguration();
}






