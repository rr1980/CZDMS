import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  imports: [
    CommonModule

  ],
  declarations: [],
  exports: [
    CommonModule
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
