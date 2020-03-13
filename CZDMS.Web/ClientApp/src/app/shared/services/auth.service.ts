import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

const helper = new JwtHelperService();
// const decodedToken = helper.decodeToken(myRawToken);
// const expirationDate = helper.getTokenExpirationDate(myRawToken);
// const isExpired = helper.isTokenExpired(myRawToken);

@Injectable()
export class AuthService {



  constructor(private http: HttpClient, public router: Router) { }

  public getToken(): string {
    return localStorage.getItem('token');
  }

  private setToken(token) {
    return localStorage.setItem('token', token);
  }

  public isAuthenticated(): boolean {

    const token = this.getToken();

    if (token && token.length) {
      return !helper.isTokenExpired(token);
    }
    return false;
  }

  public login(username: string, password: string): Observable<any> {
    // return this.http.post('https://localhost:44351/api/Token', { username: username, password: password }, { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) })
    return this.http.post('https://localhost:44351/api/Token', { username: username, password: password }, { responseType: 'text' as 'json' })
      .pipe(tap(response => {
        this.setToken(response);
      }));
  }

  logout() {
    localStorage.removeItem('token');
    this.router.navigate(['login']);
  }

}
