import { Component, OnInit } from '@angular/core';
import { AuthService } from '../shared/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'czdms-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(private authService: AuthService, public router: Router) { }

  ngOnInit() {
  }

  onClickLogin(l: string) {
    this.authService.login('rr1980' + l, '12003').subscribe((r) => {
      if (r) {
        console.debug('Login', true);
        this.router.navigate(['home']);
      }
      else {
        console.debug('Login', false);
      }
    }, err => {
      console.debug('Login', false, err.error);
    });
  }

}
