import { Component } from '@angular/core';
import config from 'devextreme/core/config';
import Button from "devextreme/ui/button";

@Component({
  selector: 'czdms-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  constructor() {

    config({
      editorStylingMode: 'underlined' // or 'filled' | 'outlined' | 'underlined'
    });

    Button.defaultOptions({
      options:{
        stylingMode: 'text' //  'text' | 'outlined' | 'contained';  
      }
    });

  }
}
