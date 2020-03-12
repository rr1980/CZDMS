import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DxFileManagerModule } from 'devextreme-angular/ui/file-manager';
@NgModule({
  imports: [
    CommonModule,
    DxFileManagerModule
  ],
  declarations: [],
  exports: [
    CommonModule,
    DxFileManagerModule
  ]
})
export class InjectionModule {
  static forRoot(): ModuleWithProviders {
    return {
      ngModule: InjectionModule,
      providers: [

      ]
    };
  }
}
