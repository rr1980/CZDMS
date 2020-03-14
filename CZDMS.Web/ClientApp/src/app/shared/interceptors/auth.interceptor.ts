import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HTTP_INTERCEPTORS } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authService: AuthService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler) {

    const token = this.authService.getToken();

    if (token) {
      const cloned = request.clone({
        headers: request.headers.set("Authorization",
          "Bearer " + token)
      });

      return next.handle(cloned);
    }
    else {
      return next.handle(request);
    }
  }
}

export const authInterceptProviders = [
  { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true }
];