import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { ActivatedRouteSnapshot, RouterStateSnapshot, } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import decode from 'jwt-decode';

@Injectable()
export class AuthRouteGuard implements CanActivate {

    constructor(public auth: AuthService, public router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | Observable<boolean> | Promise<boolean> {

        const expectedRole = route.data.expectedRole;
        const token = this.auth.getToken();
        // decode the token to get its payload
        // const tokenPayload = decode(token);
        // console.debug(tokenPayload);

        if (this.auth.isAuthenticated()) {
            // console.debug('canActivate', true);
            return true;
        }

        console.debug('canActivate', false);
        this.router.navigate(['login']);
        return false;
    }
}

