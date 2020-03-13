import { NgModule } from '@angular/core';
import { HomeRoutingModule, routedComponents } from './home-routing.module';
import { InjectionModule } from '../shared/injection/injection.module';

@NgModule({
  imports: [
    HomeRoutingModule,
    InjectionModule
  ],
  declarations: [
    routedComponents
  ]
})
export class HomeModule { }
