import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { AuthRouteGuard } from './shared/guards/auth.guard';


const routes: Routes = [
  { path: '', redirectTo: 'home', pathMatch: 'full' },
  {
    path: 'home', loadChildren: () => import('./home/home.module').then(m => m.HomeModule),
    canActivate: [AuthRouteGuard]
  },
  {
    path: 'login', component: LoginComponent
  },
  {
    path: '**', redirectTo: ''
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { useHash: true })],
  exports: [RouterModule]
})
export class AppRoutingModule { }


export const routedComponents = [LoginComponent]