import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthRouteGuard } from '../guards/auth.guard';
import { AuthService } from '../services/auth.service';

import { DxFileManagerModule } from 'devextreme-angular/ui/file-manager';
import { DxButtonModule } from 'devextreme-angular/ui/button';
import { DxToolbarModule } from 'devextreme-angular/ui/toolbar';
import { DxDataGridModule } from 'devextreme-angular/ui/data-grid';
import { DxTagBoxModule } from 'devextreme-angular/ui/tag-box';


@NgModule({
  imports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule,
    DxToolbarModule,
    DxDataGridModule,
    DxTagBoxModule
  ],
  declarations: [],
  exports: [
    CommonModule,
    DxFileManagerModule,
    DxButtonModule,
    DxToolbarModule,
    DxDataGridModule,
    DxTagBoxModule
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
