import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

import { DxFileManagerModule } from 'devextreme-angular/ui/file-manager';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { AuthRouteGuard } from '../guards/auth.guard';
import { AuthService } from '../services/auth.service';


@NgModule({
  imports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule
  ],
  declarations: [],
  exports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule
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
