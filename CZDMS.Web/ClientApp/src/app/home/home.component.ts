import { Component } from '@angular/core';

import { AuthService } from '../shared/services/auth.service';
import { Router } from '@angular/router';

export class Tab {
  text: string;
  icon: string;
  path: string;
}

const tabs: Tab[] = [
  {
    text: "Files",
    icon: "activefolder",
    path: "/home/files"
  },
  {
    text: "find",
    icon: "find",
    path: "/home/find"
  }
];

@Component({
  selector: 'czdms-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {

  tabOptions: any;
  logoutButtonOptions: any;

  constructor(private authService: AuthService, public router: Router) {
    const currentRoute = tabs.find(x=> x.path === this.router.url);
    const activeTabIndex = tabs.indexOf(currentRoute);

    this.tabOptions = {
      dataSource: tabs,
      selectedIndex: activeTabIndex,
      onItemClick: (e) => {
        this.router.navigate([e.itemData.path]);
      }
    };

    this.logoutButtonOptions = {
      text: "Logout",
      onClick: () => {
        this.authService.logout();
      }
    };
  }
}
