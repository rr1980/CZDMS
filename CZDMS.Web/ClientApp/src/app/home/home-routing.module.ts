import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home.component';
import { FilesComponent } from './files/files.component';
import { FindComponent } from './find/find.component';


const routes: Routes = [
  {
    path: '', component: HomeComponent, children: [
      { path: '', pathMatch: 'full' , redirectTo: 'files' },
      {
        path: 'files', component: FilesComponent
      },
      {
        path: 'find', component: FindComponent
      },
      { path: '**', redirectTo: '' }
    ]
  },
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class HomeRoutingModule { }


export const routedComponents = [HomeComponent, FilesComponent, FindComponent];