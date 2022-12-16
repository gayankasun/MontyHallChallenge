import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { playHallComponent } from './views/pages/playHall/playhall.component';

const routes: Routes = [
  {
    path: '',
    component: playHallComponent,
    data: {
      title: 'Play' 
    },
    children: [
      {
        path: 'pages',
        loadChildren: () =>
          import('./views/pages/pages.module').then((m) => m.PagesModule)
      },  
    ]
  },
  {
    path: 'playHall',
    component: playHallComponent,
    data: {
      title: 'Register Page'
    }
  },
  {path: '**', redirectTo: 'dashboard'}
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      scrollPositionRestoration: 'top',
      anchorScrolling: 'enabled',
      initialNavigation: 'enabledBlocking'
      // relativeLinkResolution: 'legacy'
    })
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
