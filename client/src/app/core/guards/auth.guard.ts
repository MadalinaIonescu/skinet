import { Route } from '@angular/compiler/src/core';
import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from 'src/app/account/account.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {


  constructor(private accountService: AccountService, private router: Router){}
//pagina de checkout nu era persistenta la refresh pt ca userul devenea null, asa ca schimbam BehaviorSubject cu ReplaySubject
//acum cand se cere currentUser$ nu avem valoare mereu (pt ca am dat logout) si se astepta valaore ca sa ai acces la checkout
//asa ca modificam in app.component.ts ca la refresh sa se apeleaza LoadCurrentUser, ca atunci cand nu avem user si nu avem token 
//sa avem valuarea null in currentUser
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(auth => {
        if(auth){
          return true;
        }
        this.router.navigate(['account/login'], {queryParams: {returnUrl: state.url}})
        return false;
      })
    );
  }
  
}
