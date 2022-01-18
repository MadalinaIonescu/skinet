import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, of, ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { IAddress } from '../shared/models/address';
import { IUser } from '../shared/models/user';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  private currentUserSoruce = new ReplaySubject<IUser>(1);
  currentUser$ = this.currentUserSoruce.asObservable();
  private isAdminSource = new ReplaySubject<boolean>(1);
  isAdmin$ = this.isAdminSource.asObservable();

  constructor(private http: HttpClient, private router: Router) { }

 loadCurrentUser(token : string) {
   if(token === null){
     this.currentUserSoruce.next(null);
     return of (null);
   }
   let headers = new HttpHeaders();
   headers = headers.set('Authorization', `Bearer ${token}`);

   return this.http.get(this.baseUrl + 'account', {headers}).pipe(
     map((user: IUser) => {
       if(user){
         localStorage.setItem('token', user.token);
         this.currentUserSoruce.next(user);
         this.isAdminSource.next(this.isAdmin(user.token));
       }
     })
   );
 }

 isAdmin(token : string) : boolean{
   if(token){
     const decodedToken = JSON.parse(atob(token.split('.')[1]));
     if(decodedToken.role.indexOf('Admin') > -1){
       return true;
     }
   }
   return false;
 }

  login(values: any){
    return this.http.post(this.baseUrl + 'account/login', values).pipe(
      map((user: IUser)=>{
        if(user){
          localStorage.setItem('token', user.token);
          this.currentUserSoruce.next(user);
          this.isAdminSource.next(this.isAdmin(user.token));
        }
      })
    );
  }

  register(values : any){
    return this.http.post(this.baseUrl + 'account/register', values).pipe(
      map((user: IUser)=>{
        if(user){
          localStorage.setItem('token', user.token);
          this.currentUserSoruce.next(user);
        }
      })
    );
  }

  logout(){
    localStorage.removeItem('token');
    this.currentUserSoruce.next(null);
    this.router.navigateByUrl('/');
  }

  checkEmailExists(email: string){
    return this.http.get(this.baseUrl + 'account/emailexists?email=' +email);
  }

  getUserAddress(){
    return this.http.get<IAddress>(this.baseUrl + 'account/address');
  }

  updateUserAddress(address: IAddress){
    return this.http.put<IAddress>(this.baseUrl + 'account/address', address);
  }

}
