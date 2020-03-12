import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule, routedComponents } from './app-routing.module';
import { AppComponent } from './app.component';
import { InjectionModule } from './shared/injection/injection.module';
import { logInterceptProviders } from './interceptors/log.interceptor';

@NgModule({
  declarations: [
    AppComponent,
    routedComponents
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    InjectionModule.forRoot(),
  ],
  providers: [
    logInterceptProviders
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
