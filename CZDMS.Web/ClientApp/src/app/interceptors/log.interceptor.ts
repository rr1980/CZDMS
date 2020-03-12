import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HTTP_INTERCEPTORS } from '@angular/common/http';

@Injectable()
export class LogInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler) {
    console.log('Auth interceptor');
    console.log(request);
    return next.handle(request);
  }
}

export const logInterceptProviders = [
    { provide: HTTP_INTERCEPTORS, useClass: LogInterceptor, multi: true }
  ];