import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HTTP_INTERCEPTORS } from '@angular/common/http';
import { tap } from 'rxjs/operators';

@Injectable()
export class LogInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler) {
    console.debug('request', request);
    return next.handle(request).pipe(
      tap((response) => {
        console.debug('response', response);
      })
    );
  }
}

export const logInterceptProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: LogInterceptor, multi: true }
];