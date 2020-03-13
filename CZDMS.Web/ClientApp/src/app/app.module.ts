import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule, routedComponents } from './app-routing.module';
import { AppComponent } from './app.component';
import { InjectionModule } from './shared/injection/injection.module';
import { logInterceptProviders } from './shared/interceptors/log.interceptor';

import { HttpClientModule } from '@angular/common/http';

@NgModule({
   declarations: [
      AppComponent,
      routedComponents
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule,
      InjectionModule.forRoot(),
   ],
   providers: [
      logInterceptProviders
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
