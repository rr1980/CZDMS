import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HTTP_INTERCEPTORS, HttpErrorResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler) {
    
    return next.handle(request).pipe(
      // catchError((err: HttpErrorResponse) =>{
      //   const msg = err.error.message;
      //   console.debug(err);
      //   return throwError(err.error);
      //   // return throwError(new Error(msg));
      // })
    );
  }
}

export const errorInterceptProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
];