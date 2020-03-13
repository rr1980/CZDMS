import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DxFileManagerModule } from 'devextreme-angular/ui/file-manager';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { AuthRouteGuard } from '../guards/auth.guard';
import { AuthService } from '../services/auth.service';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';


@NgModule({
  imports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule,
    DxToolbarModule
  ],
  declarations: [],
  exports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule,
    DxToolbarModule
  ]
})
export class InjectionModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: InjectionModule,
      providers: [
        AuthService,
        AuthRouteGuard
      ]
    };
  }
}
