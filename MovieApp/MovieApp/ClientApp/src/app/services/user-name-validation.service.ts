import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors } from '@angular/forms';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  map,
  Observable,
  of,
} from 'rxjs';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root',
})
export class UserNameValidationService {
  private emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  constructor(private readonly userService: UserService) {}

  validate(control: AbstractControl): Observable<ValidationErrors | null> {
    const username = control.value;

    // Check if username matches the email pattern
    if (!this.emailPattern.test(username)) {
      return of({ invalidEmail: true });
    }

    return this.userService.validateUserName(username).pipe(
      debounceTime(1000),
      distinctUntilChanged(),
      map((isUserNameAvailable) => {
        if (isUserNameAvailable) {
          return null;
        } else {
          return { userNameNotAvailable: true };
        }
      }),
      catchError(() => of(null))
    );
  }
}
